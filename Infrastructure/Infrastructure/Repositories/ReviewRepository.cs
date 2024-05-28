using Infrastructure.Data.Contexts;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories;

public class ReviewRepository(FeedbackItemsDataContext dataContext) : Repository<ReviewEntity, FeedbackItemsDataContext>(dataContext)
{
    public override IQueryable<ReviewEntity> GetSet(bool includeRelations)
    {
        if (includeRelations)
        {
            return base.GetSet(includeRelations)
                .Include(x => x.ReviewText);
        }

        return base.GetSet(includeRelations);
    }
}
