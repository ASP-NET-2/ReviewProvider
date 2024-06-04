using Infrastructure.Entities;
using Infrastructure.Models.EntityModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Factories;

public static class RatingEntityFactory
{
    public static RatingEntity Create(RatingModel model)
    {
        try
        {
            return new RatingEntity
            {
                Rating = model.Rating,
            };
        }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }

        return null!;
    }

    public static RatingEntity Update(RatingEntity entity, RatingModel model)
    {
        try
        {
            entity.Rating = model.Rating;
            return entity;
        }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }

        return entity ?? null!;
    }
}
