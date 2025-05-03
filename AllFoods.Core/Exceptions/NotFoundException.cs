using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AllFoods.Core.Exceptions
{
    public class NotFoundException : AppException
    {
        public NotFoundException(string recourseName, object recourseID) : base($"{recourseName}{recourseID}"
            ,HttpStatusCode.NotFound, "RESOURCE_NOT_FOUND")
        {
            
        }

        public NotFoundException(string message) : base(message, HttpStatusCode.NotFound, "RESOURCE_NOT_FOUND")
        {

        }
        public NotFoundException(string message, Exception innerException)
            : base(message, HttpStatusCode.NotFound, "RESOURCE_NOT_FOUND", true, null, innerException)
        {
        }
    }
}
