using AllFoods.Core.Domain.Entities;
using AllFoods.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace AllFoods_API.Filters.ExceptionFilter
{
    public class HandleExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<HandleExceptionFilter> _logger;
        private readonly IWebHostEnvironment _env;

        public HandleExceptionFilter(ILogger<HandleExceptionFilter> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        public void OnException(ExceptionContext context)
        {
            var exception = context.Exception;
            _logger.LogError("{FilterName}.{MethodName} : {ExceptionType}, {ExceptionMassage}",
                nameof(HandleExceptionFilter),nameof(OnException), exception.GetType(), exception.Message);
            var apiResponse = new APIResponse
            {
                IsSuccess = false,
                ErrorMessages = new List<string> { _env.IsDevelopment() ? exception.ToString() : exception.Message }
            };

            switch (exception)
            {

                case NotFoundException _:
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    apiResponse.StatusCode = HttpStatusCode.NotFound;
                    break;
                case BadRequestException badRequestEx:
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    break;
                case AppException appEx:
                    context.HttpContext.Response.StatusCode = (int)appEx.StatusCode;
                    apiResponse.StatusCode = appEx.StatusCode;
                    apiResponse.ErrorMessages.Add(appEx.ErrorCode!);
                    break;

                default:
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                    break;

            }

            context.Result = new JsonResult(apiResponse);
            context.ExceptionHandled = true;
        }
    }
}
