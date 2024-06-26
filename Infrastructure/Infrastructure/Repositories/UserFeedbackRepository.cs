﻿using Infrastructure.Data.Contexts;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories;

public class UserFeedbackRepository(FeedbackItemsDataContext dataContext) : Repository<UserFeedbackEntity, FeedbackItemsDataContext>(/*dataContext*/)
{
    public override IQueryable<UserFeedbackEntity> GetSet(FeedbackItemsDataContext context, bool includeRelations)
    {
        if (includeRelations)
        {
            return base.GetSet(context, includeRelations)
                .Include(x => x.Review)
                .Include(x => x.Rating);
        }

        return base.GetSet(context, includeRelations);
    }
}
