using Infrastructure.Entities;
using Infrastructure.Models.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Factories;

public static class UserFeedbackModelFactory
{
    public static UserFeedbackModel Create(UserFeedbackEntity entity)
    {
        return new UserFeedbackModel
        {
            UserId = entity.UserId,
            ProductId = entity.ProductId,
            Review = entity.Review != null
                    ? new ReviewModel
                    {
                        ReviewTitle = entity.Review.ReviewTitle,
                        ReviewText = entity.Review.ReviewText,
                        OriginallyPostedDate = entity.Review.OriginallyPostedDate,
                        LastUpdatedDate = entity.Review.LastUpdatedDate,

                    } : null,
            Rating = entity.Rating?.Rating,

        };
    }
}
