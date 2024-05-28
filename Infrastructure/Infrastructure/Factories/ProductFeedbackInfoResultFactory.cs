//using Infrastructure.Entities;
//using Infrastructure.Models.ResultModels;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Infrastructure.Factories;

//public static class ProductFeedbackInfoResultFactory
//{
//    public static ProductFeedbackInfoResult Create(ProductReviewRatingEntity entity)
//    {
//        return new ProductFeedbackInfoResult
//        {
//            AverageRating = entity.AverageRating,
//            RatingCount = entity.UserRatings?.Count ?? 0,
//            ReviewCount = entity.UserReviews?.Count ?? 0,
//        };
//    }
//}
