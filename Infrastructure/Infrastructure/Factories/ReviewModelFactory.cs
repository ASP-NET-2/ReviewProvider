using Infrastructure.Entities;
using Infrastructure.Models.EntityModels;
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
            UserId = entity.UserId,
            ProductId = entity.ProductId,
            ReviewText = entity.ReviewText != null
                    ? new ReviewTextModel
                    {
                        ReviewTitle = entity.ReviewText.ReviewTitle,
                        ReviewText = entity.ReviewText.ReviewText

                    } : null,
            Rating = entity.Rating,
            OriginallyPostedDate = entity.OriginallyPostedDate,
            LastUpdatedDate = entity.LastUpdatedDate,
        };
    }
}
