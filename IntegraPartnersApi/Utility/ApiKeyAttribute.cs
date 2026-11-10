using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IntegraPartnersApi.Utility
{
    public class ApiKeyAttribute : Attribute, IAsyncActionFilter
    {
        private const string APIKEY_HEADER_NAME = "X-API-KEY";

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var configuration = context.HttpContext.RequestServices.GetService(typeof(IConfiguration)) as IConfiguration;

            if (!context.HttpContext.Request.Headers.TryGetValue(APIKEY_HEADER_NAME, out var extractedApiKey))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Check against ApiClients configuration mapping
            var apiKey = configuration.GetSection("ApiKey");
            bool isValidApiKey = false;

            if (apiKey.Value.Equals(extractedApiKey.ToString(), StringComparison.Ordinal))
            {
                isValidApiKey = true;
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

