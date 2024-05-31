using Infrastructure.Entities;
using Infrastructure.Models.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Factories;

public static class RatingEntityFactory
{
    public static RatingEntity Create(RatingModel model)
    {
        return new RatingEntity
        {
            Rating = model.Rating,
        };
    }

    public static RatingEntity Update(RatingEntity entity, RatingModel model)
    {
        entity.Rating = model.Rating;
        return entity;
    }
}
