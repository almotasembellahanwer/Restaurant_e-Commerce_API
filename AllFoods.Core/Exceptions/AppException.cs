using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AllFoods.Core.Exceptions
{
    public class AppException : Exception
    {
        public HttpStatusCode StatusCode { get; }
        public string? ErrorCode { get; }
        public bool IsOperational { get; }
        public object? AdditionalData { get; }

        public AppException(string message, HttpStatusCode statusCode = HttpStatusCode.InternalServerError
            ,string? errorCode = null, bool isOperational = true, object? additionalData = null,Exception? innerException = null) : base(message, innerException)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode ?? statusCode.ToString();
            IsOperational = isOperational;
            AdditionalData = additionalData;
        }
    }
}
