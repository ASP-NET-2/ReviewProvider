using Infrastructure.Data.Contexts;
using Infrastructure.Entities;

namespace Infrastructure.Repositories;

public class RatingRepository(FeedbackItemsDataContext dataContext) : Repository<RatingEntity, FeedbackItemsDataContext>(dataContext)
{

}
