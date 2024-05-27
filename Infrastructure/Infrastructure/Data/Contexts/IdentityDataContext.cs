using Infrastructure.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Contexts;

public class IdentityDataContext(DbContextOptions<IdentityDataContext> options) : IdentityDbContext<UserEntity>(options)
{

}
