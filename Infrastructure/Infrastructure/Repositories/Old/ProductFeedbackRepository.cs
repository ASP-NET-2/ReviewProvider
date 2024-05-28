//using Infrastructure.Data.Contexts;
//using Infrastructure.Entities;
//using Microsoft.EntityFrameworkCore;

//namespace Infrastructure.Repositories.Old;

//public class ProductFeedbackRepository(FeedbackItemsDataContext dataContext) : Repository<ProductFeedbackEntity, FeedbackItemsDataContext>(dataContext)
//{
//    public override IQueryable<ProductFeedbackEntity> GetSet(bool includeRelations)
//    {
//        if (includeRelations)
//        {
//            return base.GetSet(includeRelations)
//                .Include(x => x.UserFeedbacks)
//                    .ThenInclude(x => x.Review)
//                .Include(x => x.UserFeedbacks)
//                    .ThenInclude(x => x.Rating);
//        }

//        return base.GetSet(includeRelations);
//    }
//}
