using Infrastructure.Entities;
using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Factories;

public static class ReviewModelFactory
{
    public static ReviewModel Create(ReviewEntity entity)
    {
        return new ReviewModel
        {
            ProductId = entity.ProductId,
            LastUpdatedDate = entity.LastUpdatedDate,
            OriginallyPostedDate = entity.OriginallyPostedDate,
            ReviewText = entity.ReviewText,
            ReviewTitle = entity.ReviewTitle,
            UserId = entity.UserId,
        };
    }
}
