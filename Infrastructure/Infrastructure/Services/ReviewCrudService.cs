using Infrastructure.Entities;
using Infrastructure.Models.EntityModels;
using Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services;

public class ReviewCrudService(ReviewRepository reviewRepo, ReviewTextRepository reviewTextRepo, RatingRepository ratingRepo)
{
    private readonly ReviewRepository _reviewRepo = reviewRepo;
    private readonly ReviewTextRepository _reviewTextRepo = reviewTextRepo;
    private readonly RatingRepository _ratingRepo = ratingRepo;

    public async Task<ReviewEntity> CreateAsync(ReviewModel model)
    {
        try
        {
            var entity = new ReviewEntity
            {
                UserId = model.UserId,
                ProductId = model.ProductId,
                OriginallyPostedDate = model.OriginallyPostedDate,
                LastUpdatedDate = model.LastUpdatedDate,
            };

            var rating = model.Rating != null
                ? await _ratingRepo.CreateAsync(new RatingEntity
                {
                    ReviewEntityId = entity.Id.ToString(),
                    Rating = model.Rating.Value,
                }) : null;

            entity.Rating = rating;

            var reviewText = model.ReviewText != null
                ? await _reviewTextRepo.CreateAsync(new ReviewTextEntity
                {
                    ReviewEntityId = entity.Id.ToString(),
                    ReviewTitle = model.ReviewText.ReviewTitle,
                    ReviewText = model.ReviewText.ReviewText,
                }) : null;

            entity.ReviewText = reviewText;

            return await _reviewRepo.CreateAsync(entity);
        }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }

        return null!;
    }

    public async Task<ReviewEntity> UpdateAsync(ReviewEntity entity, ReviewModel model)
    {
        try
        {
            entity.UserId = model.UserId;
            entity.ProductId = model.ProductId;

            if (model.Rating != null)
            {
                var ratingEntity = new RatingEntity
                {
                    ReviewEntityId = entity.Id.ToString(),
                    Rating = model.Rating.Value,
                };

                entity.Rating = entity.Rating != null
                    ? await _ratingRepo.UpdateAsync(ratingEntity)
                    : await _ratingRepo.CreateAsync(ratingEntity);
            }

            if (model.ReviewText != null)
            {
                var textEntity = new ReviewTextEntity
                {
                    ReviewEntityId = entity.Id.ToString(),
                    ReviewTitle = model.ReviewText.ReviewTitle,
                    ReviewText = model.ReviewText.ReviewText,
                };

                entity.ReviewText = entity.ReviewText != null
                    ? await _reviewTextRepo.UpdateAsync(textEntity)
                    : await _reviewTextRepo.CreateAsync(textEntity);
            }

            entity.LastUpdatedDate = DateTime.UtcNow;

            return await _reviewRepo.UpdateAsync(entity);
        }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }

        return null!;
    }

    public async Task<bool> DeleteTextAsync(ReviewTextEntity entity)
    {
        return await _reviewTextRepo.DeleteAsync(entity);
    }

    public async Task<bool> DeleteRatingAsync(RatingEntity entity)
    {
        return await _ratingRepo.DeleteAsync(entity);
    }
}
