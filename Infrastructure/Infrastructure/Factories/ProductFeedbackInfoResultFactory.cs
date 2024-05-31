using Infrastructure.Entities;
using Infrastructure.Models.ResultModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Factories;

public static class ProductFeedbackInfoResultFactory
{
    public static ProductFeedbackInfoResult Create(ProductFeedbackEntity entity)
    {
        return new ProductFeedbackInfoResult
        {
            AverageRating = entity.AverageRating,
            RatingCount = entity.RatingCount,
            ReviewCount = entity.ReviewCount,
        };
    }
}
