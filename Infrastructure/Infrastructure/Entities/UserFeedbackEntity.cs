﻿using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Entities;

public class UserFeedbackEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string UserId { get; set; } = null!;
    public string ProductId { get; set; } = null!;
    public RatingEntity? Rating { get; set; }
    public ReviewEntity? Review { get; set; }
}
