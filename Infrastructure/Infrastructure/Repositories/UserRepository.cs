using Infrastructure.Data.Contexts;
using Infrastructure.Entities;

namespace Infrastructure.Repositories;

public class UserRepository(IdentityDataContext dataContext) : Repository<UserEntity, IdentityDataContext>(/*dataContext*/)
{
}
