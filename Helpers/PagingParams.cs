using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace API.Helpers
{
    public class PagingParams
    {
        private const int MaxPageSize = 50; // Giới hạn số phần tử tối đa mỗi trang
        [FromQuery(Name = "pageNumber")]
        public int PageNumber { get; set; } = 1;

        private int _pageSize = 10;
        [FromQuery(Name = "pageSize")]
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
    }
}