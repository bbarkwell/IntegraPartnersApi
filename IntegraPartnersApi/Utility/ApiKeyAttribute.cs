using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace IntegraPartnersApi.Utility
{
    public class ApiKeyAttribute : Attribute, IAsyncActionFilter
    {
        private const string APIKEY_HEADER_NAME = "X-API-KEY";

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var config = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();

            // Retrieve the secret apiKey
            var expected = config["ApiKey"];

            if (string.IsNullOrEmpty(expected) ||
                !context.HttpContext.Request.Headers.TryGetValue(APIKEY_HEADER_NAME, out var provided) ||
                !CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(expected), Encoding.UTF8.GetBytes(provided.ToString())))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            await next();
        }
    }
}

