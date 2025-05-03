using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AllFoods.Core.Exceptions
{
    public class BadRequestException : AppException
    {
        public BadRequestException(string message)
            : base(message, HttpStatusCode.BadRequest, "BAD_REQUEST")
        {
        }

        public BadRequestException(string message, string errorCode)
            : base(message, HttpStatusCode.BadRequest, errorCode)
        {
        }

        public BadRequestException(string message, object additionalData)
            : base(message, HttpStatusCode.BadRequest, "BAD_REQUEST", true, additionalData)
        {
        }

        public BadRequestException(string message, string errorCode, object additionalData)
            : base(message, HttpStatusCode.BadRequest, errorCode, true, additionalData)
        {
        }

        public BadRequestException(string message, Exception innerException)
            : base(message, HttpStatusCode.BadRequest, "BAD_REQUEST", true, null, innerException)
        {
        }
    }
}
