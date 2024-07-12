using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class ResultModel<T>
    {
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public ResultModel(HttpStatusCode statusCode, bool isSuccess, T data, string message = "")
        {
            StatusCode = statusCode;
            IsSuccess = isSuccess;
            Message = message;
            Data = data;
        }
    }
}
