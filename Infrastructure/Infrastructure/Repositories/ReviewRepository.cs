using Infrastructure.Data.Contexts;
using Infrastructure.Entities;

namespace Infrastructure.Repositories;

public class ReviewRepository(FeedbackItemsDataContext dataContext) : Repository<ReviewEntity, FeedbackItemsDataContext>(/*dataContext*/)
{
}