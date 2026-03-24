using System.Net;
using CartSync.Controllers.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CartSyncTests.Base;

public static class Extensions
{
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
    
    extension(Task<Results<NoContent, BadRequest<Error>, NotFound<Error>, Conflict<Error>>> results)
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