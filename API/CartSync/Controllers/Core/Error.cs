using JetBrains.Annotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CartSync.Controllers.Core;

public record Error
{
    [UsedImplicitly]
    public required int StatusCode { get; init; }
    
    [UsedImplicitly] 
    public required string Message { get; init; }

    [UsedImplicitly]
    public Dictionary<string, string?>? Errors { get; init; }

    public static BadRequest<Error> BadRequestInvalidLogin()
    {
        return TypedResults.BadRequest(new Error
        {
            StatusCode = StatusCodes.Status400BadRequest,
            Message = "BadRequest: Invalid username or password"
        });
    }

    public static BadRequest<Error> BadRequestModelInvalid(Dictionary<string, string?>? errors)
    {
        return TypedResults.BadRequest(new Error
        {
            StatusCode = StatusCodes.Status400BadRequest,
            Message = "BadRequest: Input validation failure",
            Errors = errors
        });
    }

    public static BadRequest<Error> BadRequestPatchInvalid(ModelStateDictionary modelState)
    {
        Dictionary<string, string?> errors = modelState
            .Where(kvp => kvp.Value?.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value?.Errors
                    .Select(e => e.ErrorMessage)
                    .Aggregate("", (s, s1) => s + s1)
            );

        return TypedResults.BadRequest(new Error
        {
            StatusCode = StatusCodes.Status400BadRequest,
            Message = "BadRequest: JSON Patch validation failure",
            Errors = errors
        });
    }
    
    public static Error Unauthorized =>
        new()
        {
            StatusCode = StatusCodes.Status401Unauthorized,
            Message = "Unauthorized: Invalid token"
        };

    public static NotFound<Error> NotFound(Ulid id, string itemName)
    {
        return TypedResults.NotFound(new Error
        {
            StatusCode = StatusCodes.Status404NotFound,
            Message = $"NotFound: {itemName} matching id {id} was not found"
        });
    }

    public static Conflict<Error> Conflict(string msg)
    {
        return TypedResults.Conflict(new Error
        {
            StatusCode = StatusCodes.Status409Conflict,
            Message = $"Conflict: {msg}"
        });
    }

    public static NotFound<Error> NotFoundUnder(Ulid id1, string itemName1, Ulid id2, string itemName2)
    {
        return TypedResults.NotFound(new Error
        {
            StatusCode = StatusCodes.Status404NotFound,
            Message = $"NotFound: {itemName1} matching id {id1} was not found under {itemName2} with Id {id2}"
        });
    }
}