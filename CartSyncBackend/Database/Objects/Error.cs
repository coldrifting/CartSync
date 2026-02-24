using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace CartSyncBackend.Database.Objects;

public class Error(string status, string message)
{
    private const string BadRequest = "400 Bad Request";
    private const string NotFound = "404 Not Found";
    
    public string Status { get; init; } = status;
    public string Message { get; init; } = message;

    public static NotFoundObjectResult NotFoundItem => 
        new(new Error(NotFound, "Item matching given ID not found"));
    
    public static NotFoundObjectResult NotFoundStore => 
        new(new Error(NotFound, "Store matching given ID not found"));
    
    public static NotFoundObjectResult NotFoundAisle => 
        new(new Error(NotFound, "Aisle matching given ID not found"));
    
    public static BadRequestObjectResult BadRequestAisleNotUnderStore => 
        new(new Error(BadRequest, "Aisle does not belong to the selected store"));
}