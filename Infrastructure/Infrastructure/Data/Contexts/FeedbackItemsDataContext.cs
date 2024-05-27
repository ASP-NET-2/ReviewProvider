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
    public DbSet<ReviewEntity> ProductReviews { get; set; }
    public DbSet<RatingEntity> ProductRatings { get; set; }
}
