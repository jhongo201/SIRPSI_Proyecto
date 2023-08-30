using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRPSI.Wrappers
{
    public class PagedResponse<T> : Response<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int? TotalItems { get; set; }
        public PagedResponse(T data, int pageNumber, int pageSize, int? totalItems = null, string? title = null, int? status = null, string? message = null)
        {
            this.PageNumber = pageNumber;
            this.PageSize = pageSize;
            this.Data = data;
            this.Message = message;
            this.Succeeded = true;
            this.Erros = null;
            this.TotalItems = totalItems;
            this.Title = title;
            this.Status = status;
        }
    }
}
