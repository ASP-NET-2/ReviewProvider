using Infrastructure.Data.Contexts;
using Infrastructure.Entities;

namespace Infrastructure.Repositories;

public class ReviewTextRepository(FeedbackItemsDataContext dataContext) : Repository<ReviewTextEntity, FeedbackItemsDataContext>(dataContext)
{
}