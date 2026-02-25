using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;

namespace CartSyncBackend.Database.Objects;

public class Error(int status, string message,  Dictionary<string, object>? details = null)
{
    private const string BadRequest = "Bad Request: ";
    private const string NotFound = "Not Found: ";
    
    [UsedImplicitly]
    public int Status { get; init; } = status;
    
    [UsedImplicitly] 
    public string Message { get; init; } = message;

    [UsedImplicitly]
    public Dictionary<string, object>? Details { get; init; } = details;

    public static BadRequestObjectResult BadRequestInvalidStoreId =>
        new(new Error(
            StatusCodes.Status400BadRequest,
            BadRequest + "StoreId is invalid"));

    public static BadRequestObjectResult BadRequestStoreNameInvalid =>
        new(new Error(
            StatusCodes.Status400BadRequest,
            BadRequest + "Store name is invalid"));
    
    public static BadRequestObjectResult BadRequestAisleNameInvalid =>
        new(new Error(
            StatusCodes.Status400BadRequest,
            BadRequest + "Aisle name is invalid"));

    public static BadRequestObjectResult BadRequestAisleNotUnderStore =>
        new(new Error(
            StatusCodes.Status400BadRequest,
            BadRequest + "Aisle does not belong to the selected store"));
    
    public static BadRequestObjectResult BadRequestPrepNameInvalid =>
        new(new Error(
            StatusCodes.Status400BadRequest,
            BadRequest + "Prep name is invalid"));

    public static BadRequestObjectResult BadRequestInvalidRecipeId =>
        new(new Error(
            StatusCodes.Status400BadRequest,
            BadRequest + "RecipeId is invalid"));
    
    public static BadRequestObjectResult BadRequestPrepDeleteFailed(Dictionary<string, object> details)
    {
        return new BadRequestObjectResult(
            new Error(
                StatusCodes.Status400BadRequest,
                BadRequest + 
                "Prep is in use. " +
                "Use override parameter to force a cascading delete. " +
                "Related entities listed in details",
                details
            ));
    }
    
    public static BadRequestObjectResult BadRequestItemAddRequestInvalid(List<string> errorList)
    {
        Dictionary<string, object> validationErrors = new()
        {
            ["errors"] = errorList
        };

        return new BadRequestObjectResult(
            new Error(
                StatusCodes.Status400BadRequest,
                BadRequest + "Invalid model object provided to create new Item",
                validationErrors
            ));
    }
    
    public static BadRequestObjectResult BadRequestItemEditRequestInvalid(List<string> errorList)
    {
        Dictionary<string, object> validationErrors = new()
        {
            ["errors"] = errorList
        };

        return new BadRequestObjectResult(
            new Error(
                StatusCodes.Status400BadRequest,
                BadRequest + "Invalid model object provided to edit Item",
                validationErrors
            ));
    }
    
    public static NotFoundObjectResult NotFoundStore => 
        new(new Error(
            StatusCodes.Status404NotFound,
            NotFound + "Store matching given ID not found"));
    
    public static NotFoundObjectResult NotFoundAisle => 
        new(new Error(
            StatusCodes.Status404NotFound,
            NotFound + "Aisle matching given ID not found"));
    
    public static NotFoundObjectResult NotFoundItem => 
        new(new Error(
            StatusCodes.Status404NotFound,
            NotFound + "Item matching given ID not found"));

    public static NotFoundObjectResult NotFoundPrep =>
        new(new Error(
            StatusCodes.Status404NotFound,
            NotFound + "Prep matching given ID not found"));
    
    public static NotFoundObjectResult NotFoundRecipe => 
        new(new Error(
            StatusCodes.Status404NotFound,
            NotFound + "Recipe matching given ID not found"));
}