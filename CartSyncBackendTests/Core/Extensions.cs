global using UsageResponse = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<(System.Ulid, string)>>;

using CartSyncBackend.Database.Objects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace CartSyncBackendTests.Core;

public static class Extensions
{
    public static Func<object?, object?, bool> UsageResponseComparer =>
        (obj1, obj2) =>
        {
            if (obj1 is null)
            {
                if (obj2 is null)
                {
                    return true;
                }

                return false;
            }

            if (obj2 is null)
            {
                return true;
            }

            if (obj1 is not UsageResponse usageResponse1 || obj2 is not UsageResponse usageResponse2)
            {
                return false;
            }

            if (!usageResponse1.Keys.OrderBy(i => i).SequenceEqual(usageResponse2.Keys.OrderBy(i => i)))
            {
                return false;
            }

            foreach (string? key in usageResponse1.Keys)
            {
                IOrderedEnumerable<(Ulid, string)> seq1 = usageResponse1[key].OrderBy(i => i);
                IOrderedEnumerable<(Ulid, string)> seq2 = usageResponse1[key].OrderBy(i => i);

                if (!seq1.SequenceEqual(seq2))
                {
                    return false;
                }
            }
                
            return true;
        };

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
        
        public async Task<T> CreatedAsync<T>(Func<T, Ulid> idSelector) where T : class
        {
            IActionResult result = await actionResult;
            
            CreatedResult? createdAtActionResult = result as CreatedResult;
            Assert.NotNull(createdAtActionResult);
            Assert.NotNull(createdAtActionResult.Location);

            Ulid locationId = Ulid.Parse(createdAtActionResult.Location.Split('/').Last());

            T? value = createdAtActionResult.Value as T;
            Assert.NotNull(value);
            
            Ulid valueId = idSelector(value);
            
            Assert.Equal(valueId, locationId);
        
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