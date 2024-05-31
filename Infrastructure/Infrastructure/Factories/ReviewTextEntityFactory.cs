using Infrastructure.Entities;
using Infrastructure.Models.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Factories;

public static class ReviewTextEntityFactory
{
    public static ReviewEntity Create(ReviewModel model)
    {
        return new ReviewEntity
        {
            ReviewTitle = model.ReviewTitle,
            ReviewText = model.ReviewText,
        };
    }

    public static ReviewEntity Update(ReviewEntity entity, ReviewModel model)
    {
        entity.ReviewTitle = model.ReviewTitle;
        entity.ReviewText = model.ReviewText;
        entity.LastUpdatedDate = DateTime.Now;
        return entity;
    }
}
