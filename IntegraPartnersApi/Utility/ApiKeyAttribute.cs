using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;

namespace IntegraPartnersApi.Utility
{
    public class ApiKeyAttribute : Attribute, IAsyncActionFilter
    {
        private const string APIKEY_HEADER_NAME = "X-API-KEY";

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //var configuration = context.HttpContext.RequestServices.GetService(typeof(IConfiguration)) as IConfiguration;

            var builder = WebApplication.CreateBuilder();
            // or Host.CreateDefaultBuilder(args) for console apps

            // Retrieve the secret seamlessly
            string? apiKey = builder.Configuration["ApiKey"];

            if (!context.HttpContext.Request.Headers.TryGetValue(APIKEY_HEADER_NAME, out var extractedApiKey))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            bool isValidApiKey = false;
            if (apiKey == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            else
            {
                if (apiKey.Equals(extractedApiKey.ToString(), StringComparison.Ordinal))
                {
                    isValidApiKey = true;
                }
            }
            
            if (!isValidApiKey)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            await next();
        }
    }
}

