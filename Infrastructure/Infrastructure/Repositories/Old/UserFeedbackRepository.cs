//using Infrastructure.Data.Contexts;
//using Infrastructure.Entities.Old;
//using Microsoft.EntityFrameworkCore;

//namespace Infrastructure.Repositories;

//public class UserFeedbackRepository(FeedbackItemsDataContext dataContext) : Repository<UserFeedbackEntity, FeedbackItemsDataContext>(dataContext)
//{
//    public override IQueryable<UserFeedbackEntity> GetSet(bool includeRelations)
//    {
//        if (includeRelations)
//        {
//            return base.GetSet(includeRelations)
//                .Include(x => x.Review)
//                .Include(x => x.Rating);
//        }

//        return base.GetSet(includeRelations);
//    }
//}
