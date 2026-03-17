using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CartSyncBackend.Database.Objects;

public class Error(int statusCode, string message,  Dictionary<string, string?>? details = null)
{
    [UsedImplicitly]
    public int StatusCode { get; init; } = statusCode;
    
    [UsedImplicitly] 
    public string Message { get; init; } = message;

    [UsedImplicitly]
    public Dictionary<string, string?>? Errors { get; init; } = details;

    public static BadRequestObjectResult BadRequestModelInvalid(Dictionary<string, string?>? errors)
    {
        return new BadRequestObjectResult(
            new Error(
                StatusCodes.Status400BadRequest,
                 "BadRequest: Input validation failure",
                errors));
    }
    
    public static BadRequestObjectResult BadRequestPatchInvalid(ModelStateDictionary modelState)
    {
        Dictionary<string, string?> errors = modelState
            .Where(kvp => kvp.Value?.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value?.Errors
                    .Select(e => e.ErrorMessage)
                    .Aggregate("", (s, s1) => s + s1)
                );
        
        return new BadRequestObjectResult(
            new Error(
                StatusCodes.Status400BadRequest,
                 "BadRequest: JSON Patch validation failure",
                errors));
    }

    public static NotFoundObjectResult NotFound(Ulid id, string itemName)
    {
        return new NotFoundObjectResult(
            new Error(
                StatusCodes.Status404NotFound,
                $"NotFound: {itemName} matching id {id} was not found"
            ));
    }

    public static NotFoundObjectResult NotFoundUnder(Ulid id1, string itemName1, Ulid id2, string itemName2)
    {
        return new NotFoundObjectResult(
            new Error(
                StatusCodes.Status404NotFound,
                $"NotFound: {itemName1} matching id {id1} was not found under {itemName2} with Id {id2}"
            ));
    }
}