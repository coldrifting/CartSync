global using UsageResponse = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<(System.Ulid, string)>>;
using System.Net;
using CartSync.Controllers.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CartSyncTests.Core;

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

    extension(Error error)
    {
        public void AssertStatus(HttpStatusCode statusCode)
        {
            Assert.Equal((int)statusCode, error.StatusCode);
        }
    }

    public static async Task<T> ValueAsync<T>(this Task<Ok<T>> results) => GetValue<T>(await results);

    extension<T>(Task<Results<Ok<T>, BadRequest<Error>>> results)
    {
        public async Task<T>     ValueAsync() => GetValue<T>((await results).Result);
        public async Task<Error> ErrorAsync() => GetValue<Error>((await results).Result);
    }

    extension<T>(Task<Results<Ok<T>, BadRequest<Error>, NotFound<Error>>> results)
    {
        public async Task<T>     ValueAsync() => GetValue<T>((await results).Result);
        public async Task<Error> ErrorAsync() => GetValue<Error>((await results).Result);
    }

    extension<T>(Task<Results<Created<T>, BadRequest<Error>>> results)
    {
        public async Task<(T, string)> ValueAsync() => GetValueCreated<T>((await results).Result);
        public async Task<Error>       ErrorAsync() => GetValue<Error>((await results).Result);
    }

    extension<T>(Task<Results<Created<T>, BadRequest<Error>, NotFound<Error>>> results)
    {
        public async Task<(T, string)> ValueAsync() => GetValueCreated<T>((await results).Result);
        public async Task<Error>       ErrorAsync() => GetValue<Error>((await results).Result);
    }

    extension(Task<Results<NoContent, BadRequest<Error>>> results)
    {
        public async Task AssertIsSuccessful() => AssertIsNoContent((await results).Result);
        public async Task<Error> ErrorAsync() => GetValue<Error>((await results).Result);
    }
    
    extension(Task<Results<NoContent, BadRequest<Error>, NotFound<Error>>> results)
    {
        public async Task AssertIsSuccessful() => AssertIsNoContent((await results).Result);
        public async Task<Error> ErrorAsync() => GetValue<Error>((await results).Result);
    }
    
    private static T GetValue<T>(IResult input)
    {
        IValueHttpResult<T>? value = input as IValueHttpResult<T>;
        Assert.NotNull(value);
        Assert.NotNull(value.Value);
        return value.Value;
    }

    private static (T, string) GetValueCreated<T>(IResult input)
    {
        Created<T>? created = input as Created<T>;
        Assert.NotNull(created);
        Assert.NotNull(created.Value);
        Assert.NotNull(created.Location);
        Assert.NotEmpty(created.Location);
        return (created.Value, created.Location);
    }
    
    private static void AssertIsNoContent(IResult input)
    {
        NoContent? value = input as NoContent;
        Assert.NotNull(value);
    }
}