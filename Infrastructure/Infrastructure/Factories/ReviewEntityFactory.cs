using Infrastructure.Entities;
using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Factories;

public static class ReviewEntityFactory
{
    public static ReviewEntity Create(UserEntity user, ProductModel product, string reviewTitle, string reviewText)
    {
        ArgumentNullException.ThrowIfNull(product.Id);

        return new ReviewEntity
        {
            ProductId = product.Id,
            UserId = user.Id,
            ReviewTitle = reviewTitle,
            ReviewText = reviewText
        };
    }
}
