using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace API.Helpers
{
    public static class PaginationExtensions
    {
        public static PaginationMetaData ToPaginationMeta(this IPagedList pagedList)
        {
            return new PaginationMetaData(
                pagedList.PageNumber,
                pagedList.PageCount,
                pagedList.PageSize,
                pagedList.TotalItemCount
            );
        }
    }
}