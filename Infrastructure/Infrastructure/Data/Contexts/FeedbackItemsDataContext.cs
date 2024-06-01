using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Contexts;

public class FeedbackItemsDataContext(DbContextOptions<FeedbackItemsDataContext> options) : DbContext(options)
{
    public DbSet<ProductFeedbackEntity> ProductFeedbacks { get; set; }
    public DbSet<UserFeedbackEntity> UserFeedbacks { get; set; }
    public DbSet<ReviewEntity> UserReviews { get; set; }
    public DbSet<RatingEntity> UserRatings { get; set; }

}
