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
    public DbSet<ProductReviewRatingEntity> ProductFeedbacks { get; set; }
    public DbSet<ReviewEntity> Reviews { get; set; }
    public DbSet<ReviewTextEntity> ReviewTexts { get; set; }
}
