//using Infrastructure.Entities;
//using Infrastructure.Models.EntityModels;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Infrastructure.Factories;

//public static class ReviewEntityFactory
//{
//    public static ReviewEntity CreateReview(string userId, string productId, ReviewModel model)
//    {
//        var entity = new ReviewEntity
//        {
//            ProductId = productId,
//            UserId = userId,
//            ReviewTitle = model.ReviewTitle,
//            ReviewText = model.ReviewText,
//        };

//        return entity;
//    }

//    public static ReviewEntity UpdateReview(ReviewEntity entity, ReviewModel model)
//    {
//        if (model.ReviewText != null)
//        {
//            entity.ReviewText = new ReviewTextEntity { r};
//            entity.ReviewText.ReviewText = model.ReviewText.ReviewText;
//        }
//        entity.ReviewTitle = model.ReviewTitle;
        
//        entity.LastUpdatedDate = DateTime.UtcNow;
//        return entity;
//    }
//}
