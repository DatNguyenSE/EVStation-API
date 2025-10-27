using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers
{
    public class PaginationMetaData
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public PaginationMetaData() { }

        // Constructor nhanh để tạo từ IPagedList
        public PaginationMetaData(X.PagedList.IPagedList pagedList)
        {
            CurrentPage = pagedList.PageNumber;
            TotalPages = pagedList.PageCount;
            PageSize = pagedList.PageSize;
            TotalCount = pagedList.TotalItemCount;
        }

        // Constructor tùy chỉnh nếu cần
        public PaginationMetaData(int currentPage, int totalPages, int pageSize, int totalCount)
        {
            CurrentPage = currentPage;
            TotalPages = totalPages;
            PageSize = pageSize;
            TotalCount = totalCount;
        }
    }
}