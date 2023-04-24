using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearnJwtAuth.DTO
{
    public class PagedResponseDTO<T>
    {
        public int TotalCount { get; set; } // TOTAL DATA

        public int PageNumber { get; set; } // CURRENT PAGE

        public int PageSize { get; set; } // DATA PERPAGE

        public int TotalPage { get; set; } // TOTAL PAGE

        public List<T> Items { get; set; }

        public PagedResponseDTO()
        {
        }

        // TODO: Lanjut pagination
        public PagedResponseDTO(List<T> items, int totalCount, int pageNumber, int pageSize, int totalPage)
        {
            Items = items;
            TotalCount = totalCount;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalPage = totalPage;
        }
    }
}