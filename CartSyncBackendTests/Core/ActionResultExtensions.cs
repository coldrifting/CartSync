using CartSyncBackend.Database.Objects;
using Microsoft.AspNetCore.Mvc;

namespace CartSyncBackendTests.Core;

public static class ActionResultExtensions
{
    extension(IActionResult actionResult)
    {
        public T Value<T>() where T : class
        {
            OkObjectResult? okObjectResult = actionResult as OkObjectResult;
            Assert.NotNull(okObjectResult);

            T? value = okObjectResult.Value as T;
            Assert.NotNull(value);
        
            return value;
        }

        public Error Error()
        {
            return actionResult switch
            {
                BadRequestObjectResult badRequestObjectResult => badRequestObjectResult.Value as Error,
                NotFoundObjectResult notFoundObjectResult => notFoundObjectResult.Value as Error,
                _ => null
            } ?? new Error(-1, "Unknown Error");
        }
    }
}