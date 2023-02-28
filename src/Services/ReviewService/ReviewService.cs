using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.src.Services.ReviewService
{
    public class ReviewService : BaseService<Review, ReviewDto>, IReviewService
    {  
        public ReviewService(IMapper mapper, DatabaseContext context) : base(mapper, context)
        {
        }
    }
}