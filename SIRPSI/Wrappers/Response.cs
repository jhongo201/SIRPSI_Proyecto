using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRPSI.Wrappers
{
    public class Response<T>
    {
        public Response()
        {

        }
        public Response(T data, string message = null, string? title = null, int? status = null)
        {
            Succeeded = true;
            Message = message;
            Data = data;
            Title = title;
            Status = status;
        }
        public Response(string message, string? title = null, int? status = null)
        {
            Succeeded = false;
            Message = message;
            Title = title;
            Status = status;
        }
        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public string Title { get; set; }
        public int? Status { get; set; }
        public List<string> Erros { get; set; }
        public T Data { get; set; }
    }
}
