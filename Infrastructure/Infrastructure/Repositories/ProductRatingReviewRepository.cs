using Infrastructure.Data.Contexts;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProductRatingReviewRepository(FeedbackItemsDataContext dataContext) : Repository<ProductReviewRatingEntity, FeedbackItemsDataContext>(dataContext)
{
    public override IQueryable<ProductReviewRatingEntity> GetSet(bool includeRelations)
    {
        if (includeRelations)
        {
            return base.GetSet(includeRelations)
                .Include(x => x.UserReviews);
        }

        return base.GetSet(includeRelations);
    }
}

