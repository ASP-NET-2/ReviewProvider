using Infrastructure.Data.Contexts;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProductFeedbackRepository(FeedbackItemsDataContext dataContext) : Repository<ProductFeedbackEntity, FeedbackItemsDataContext>(dataContext)
{
    public override IQueryable<ProductFeedbackEntity> GetSet(bool includeRelations)
    {
        if (includeRelations)
        {
            return base.GetSet(includeRelations)
                .Include(x => x.UserFeedbacks);
        }

        return base.GetSet(includeRelations);
    }
}

