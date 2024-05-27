using Infrastructure.Entities;
using Infrastructure.Factories;
using Infrastructure.Models;
using Infrastructure.Models.Query;
using Infrastructure.Models.RequestModels;
using Infrastructure.Models.ResultModels;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Json;

namespace Infrastructure.Services;

/// <summary>
/// Service that manages reviews and ratings.
/// </summary>
public class FeedbackItemService(ReviewRepository reviewRepo, RatingRepository ratingRepo, UserManager<UserEntity> userManager, 
    SignInManager<UserEntity> signInManager, HttpClient httpClient, IConfiguration config)
{
    private readonly ReviewRepository _reviewRepo = reviewRepo;
    private readonly RatingRepository _ratingRepo = ratingRepo;
    private readonly UserManager<UserEntity> _userManager = userManager;
    private readonly SignInManager<UserEntity> _signInManager = signInManager;
    private readonly HttpClient _httpClient = httpClient;
    private readonly IConfiguration _config = config;

    private static ProcessResult ProduceCatchError(Exception ex)
    {
        Debug.WriteLine(ex.Message);
        return new ProcessResult(StatusCodes.Status500InternalServerError, "ERROR: " + ex.Message);
    }

    //public async Task<UserEntity>

    public async Task<ProcessResult<int>> GetTotalReviewCount(string productId)
    {
        try
        {
            int result = await _reviewRepo.GetSet(false).CountAsync();

            return new(StatusCodes.Status200OK, "", result);
        }
        catch (Exception ex) { return ProduceCatchError(ex).ToGeneric<int>(); }
    }

    public async Task<ProcessResult> AddReviewAsync(ReviewCreationRequestModel requestModel)
    {
        try
        {
            // Get user, we need both its ID and to ensure that they're logged in.
            var entity = await _userManager.GetUserAsync(requestModel.UserClaims);
            if (entity == null || !_signInManager.IsSignedIn(requestModel.UserClaims))
            {
                return ProcessResult.ForbiddenResult("User must prove that they're logged in to leave a review.");
            }

            string productId = requestModel.ProductId;

            // Ensure that product exists.
            var product = await _httpClient.GetFromJsonAsync<ProductModel>($"{_config["SingleProductUrl"]}/{productId}");
            if (product == null)
                return ProcessResult.NotFoundResult("The product being reviewed does not/no longer exists.");

            // Exceptional case, but still here so we don't crash this
            // thing under any circumstances whatsoever
            if (product.Id == null)
                return ProcessResult.BadRequestResult("The product's ID was null.");

            var reviewEntity = ReviewEntityFactory.Create(entity, product, requestModel.ReviewTitle, 
                requestModel.ReviewText);

            // Already exists somehow? Conflict.
            if (await _reviewRepo.ExistsAsync(x => x.UserId == reviewEntity.UserId && x.ProductId == productId))
                return ProcessResult.ConflictResult("User has already reviewed this product. To edit this review, call EditReview instead.");

            var result = await _reviewRepo.CreateAsync(reviewEntity);
            if (result == null)
                return ProcessResult.InternalServerErrorResult("Review could not be posted for an unknown reason.");

            return new ProcessResult(StatusCodes.Status201Created, "Review successfully posted.");
        }
        catch (Exception ex) { return ProduceCatchError(ex); }
    }

    public async Task<ProcessResult<BaseFeedbackItemResult<ReviewModel>>> GetAllReviewsOfProductAsync(BaseFeedbackQueryModel qModel)
    {
        try
        {
            if (qModel.ProductId == null && qModel.ByUserId == null)
                return new ProcessResult<BaseFeedbackItemResult<ReviewModel>>(StatusCodes.Status200OK, "ProductId and ByUserId are both null. Returning empty list.", new());

            var query = _reviewRepo.GetSet(false);

            if (qModel.ProductId != null)
            {
                query = query.Where(x => x.ProductId == qModel.ProductId);
            }
            if (qModel.ByUserId != null)
            {
                query = query.Where(x => x.UserId ==  qModel.ByUserId);
            }

            query = query.OrderByDescending(x => x.OriginallyPostedDate);

            var result = new BaseFeedbackItemResult<ReviewModel>();

            if (qModel.PageQuery != null)
            {
                PageQuery pageQ = qModel.PageQuery.Value;
                int totalItemCount = await query.CountAsync();
                if (totalItemCount > 0)
                {
                    int takeCount = qModel.MaxItemCount != null 
                        ? Math.Min(pageQ.PageSize, qModel.MaxItemCount.Value)
                        : pageQ.PageSize;

                    query = query.Skip((pageQ.PageNumber - 1) * pageQ.PageSize).Take(pageQ.PageSize);

                    var list = await query.ToListAsync();
                    
                    var resultList = new List<ReviewModel>();
                    foreach (var item in list) 
                    {
                        resultList.Add(ReviewModelFactory.Create(item));
                    }
                }
            }
        }
        catch (Exception ex) { return ProduceCatchError(ex).ToGeneric<BaseFeedbackItemResult<ReviewModel>>(); }
    }
}
