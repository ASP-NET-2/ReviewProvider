﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Models;

public class RatingModel : BaseFeedbackItemModel
{
    public decimal Rating { get; set; }
}
