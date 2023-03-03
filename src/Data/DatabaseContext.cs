using System.IO.Pipes;
using System.Reflection.Emit;
using Backend.src.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql;

namespace Backend.src.Data
{
    public class DatabaseContext : DbContext
    {
        private readonly IConfiguration _config;

        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        static DatabaseContext()
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        public DatabaseContext(DbContextOptions<DatabaseContext> options, IConfiguration config) : base(options)
        {
            _config = config;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.HasPostgresEnum<Role>();
            modelBuilder.HasPostgresEnum<Paid>();
            modelBuilder.HasPostgresEnum<Rating>();

            /* Configure Review model */
            modelBuilder.Entity<Review>(
                entity =>
                {
                    entity
                        .HasOne(r => r.User)
                        .WithMany()
                        .HasForeignKey(r => r.UserId)
                        .OnDelete(DeleteBehavior.SetNull);
                    entity
                        .HasOne(r => r.Product)
                        .WithMany()
                        .HasForeignKey(r => r.ProductId)
                        .OnDelete(DeleteBehavior.Cascade);
                    entity
                        .Property(e => e.Rating)
                        .HasColumnType("rating");
                }
            );

            /*  Configure Product Model*/
            modelBuilder.Entity<Product>(
                entity =>
                {
                    entity
                        .HasOne(p => p.User)
                        .WithMany()
                        .HasForeignKey(p => p.SellerId)
                        .OnDelete(DeleteBehavior.Cascade);
                    entity
                        .HasOne(p => p.Category)
                        .WithMany()
                        .HasForeignKey(p => p.CategoryID)
                        .OnDelete(DeleteBehavior.Cascade);
                }
            );

            modelBuilder.Entity<User>(
                entity =>
                {
                    entity
                        .Property(e => e.Role)
                        .HasColumnType("role");
                }
            );

            /* Configure Order model */
            modelBuilder.Entity<Order>(
                entity =>
                {
                    entity
                        .HasOne(o => o.User)
                        .WithMany()
                        .HasForeignKey(r => r.UserId)
                        .OnDelete(DeleteBehavior.Cascade);
                });

            /* Configure OrderItem model*/
            modelBuilder.Entity<OrderItem>(
                entity =>
                {
                    entity
                        .HasOne(i => i.Order)
                        .WithMany()
                        .HasForeignKey(i => i.OrderId)
                        .OnDelete(DeleteBehavior.Cascade);
                    entity
                       .HasOne(i => i.Product)
                       .WithMany()
                       .HasForeignKey(i => i.ProductId)
                       .OnDelete(DeleteBehavior.Cascade);
                    entity
                        .HasKey(i => new { i.OrderId, i.ProductId });
                }
            );

            /* Configure Category model*/
            modelBuilder.Entity<Category>()
                .HasIndex(c => c.Name)
                .IsUnique();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(_config.GetConnectionString("DefaultConnection"));
            dataSourceBuilder.MapEnum<Rating>();
            dataSourceBuilder.MapEnum<Role>();
            dataSourceBuilder.MapEnum<Paid>();
            optionsBuilder
            .UseNpgsql(dataSourceBuilder.ConnectionString)
            .UseSnakeCaseNamingConvention();
        }
    }
}
