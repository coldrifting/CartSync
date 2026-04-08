using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CartSync.Data.Responses;

public record ErrorResponse
{
    public required int StatusCode { get; init; }
    public required string Message { get; init; }
    public Dictionary<string, string?>? Errors { get; init; }

    public static BadRequest<ErrorResponse> BadRequestInvalidLogin()
    {
        return TypedResults.BadRequest(new ErrorResponse
        {
            StatusCode = StatusCodes.Status400BadRequest,
            Message = "BadRequest: Invalid username or password"
        });
    }

    public static BadRequest<ErrorResponse> BadRequestModelInvalid(Dictionary<string, string?>? errors)
    {
        return TypedResults.BadRequest(new ErrorResponse
        {
            StatusCode = StatusCodes.Status400BadRequest,
            Message = "BadRequest: Input validation failure",
            Errors = errors
        });
    }

    public static BadRequest<ErrorResponse> BadRequestCartAmountInvalid()
    {
        return TypedResults.BadRequest(new ErrorResponse
        {
            StatusCode = StatusCodes.Status400BadRequest,
            Message = "BadRequest: Cart amount must be greater than 0"
        });
    }

    public static BadRequest<ErrorResponse> BadRequestPatchInvalid(ModelStateDictionary modelState)
    {
        Dictionary<string, string?> errors = modelState
            .Where(kvp => kvp.Value?.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value?.Errors
                    .Select(e => e.ErrorMessage)
                    .Aggregate("", (s, s1) => s + s1)
            );

        return TypedResults.BadRequest(new ErrorResponse
        {
            StatusCode = StatusCodes.Status400BadRequest,
            Message = "BadRequest: JSON Patch validation failure",
            Errors = errors
        });
    }
    
    public static ErrorResponse Unauthorized()
    {
        return new ErrorResponse
        {
            StatusCode = StatusCodes.Status401Unauthorized,
            Message = "Unauthorized: Invalid token"
        };
    }

    public static NotFound<ErrorResponse> NotFound(Ulid id, string itemName)
    {
        return TypedResults.NotFound(new ErrorResponse
        {
            StatusCode = StatusCodes.Status404NotFound,
            Message = $"NotFound: {itemName} matching id {id} was not found"
        });
    }

    public static NotFound<ErrorResponse> NotFoundCompositeKey(string itemName, Ulid id1, string itemName1, Ulid? id2, string itemName2)
    {
        return TypedResults.NotFound(new ErrorResponse
        {
            StatusCode = StatusCodes.Status404NotFound,
            Message = $"NotFound: {itemName} with composite key containing {itemName1} with id {id1} and {itemName2} with id {id2}"
        });
    }

    public static Conflict<ErrorResponse> Conflict(string msg)
    {
        return TypedResults.Conflict(new ErrorResponse
        {
            StatusCode = StatusCodes.Status409Conflict,
            Message = $"Conflict: {msg}"
        });
    }

    public static NotFound<ErrorResponse> NotFoundUnder(Ulid id1, string itemName1, Ulid id2, string itemName2)
    {
        return TypedResults.NotFound(new ErrorResponse
        {
            StatusCode = StatusCodes.Status404NotFound,
            Message = $"NotFound: {itemName1} matching id {id1} was not found under {itemName2} with Id {id2}"
        });
    }

    public static Conflict<ErrorResponse> AlreadyExists(string parentItem, Ulid id1, string itemName1, Ulid? id2, string itemName2)
    {
        return TypedResults.Conflict(new ErrorResponse
        {
            StatusCode = StatusCodes.Status409Conflict,
            Message = $"Conflict: Can not add or edit {parentItem} with {itemName1} id of {id1} and {itemName2} id of {id2}. The composite key already exists"
        });
    }
}