using CartSyncBackend.Database.Objects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace CartSyncBackendTests.Core;

public static class Extensions
{
    extension(Ulid)
    {
        public static Ulid NotFound => Ulid.Parse("40400000000000000000000404");
    }
    
    extension(Task<IActionResult> actionResult)
    { 
        public async Task<T> ValueAsync<T>() where T : class
        {
            IActionResult result = await actionResult;
            
            OkObjectResult? okObjectResult = result as OkObjectResult;
            Assert.NotNull(okObjectResult);

            T? value = okObjectResult.Value as T;
            Assert.NotNull(value);
        
            return value;
        }

        public async Task<Error> ErrorAsync()
        {
            IActionResult result = await actionResult;
            
            return result switch
            {
                BadRequestObjectResult badRequestObjectResult => badRequestObjectResult.Value as Error,
                NotFoundObjectResult notFoundObjectResult => notFoundObjectResult.Value as Error,
                _ => null
            } ?? throw new InvalidOperationException("Unexpected Status Code:  " + ((IStatusCodeActionResult)result).StatusCode);
        }
    }
}