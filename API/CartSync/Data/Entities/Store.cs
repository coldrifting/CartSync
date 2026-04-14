using System.ComponentModel.DataAnnotations;
using CartSync.Data.Requests;
using CartSync.Data.Responses;
using CartSync.Interfaces;
using CartSync.Utils;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace CartSync.Data.Entities;

[PrimaryKey(nameof(StoreId))]
public class Store: IPatchable<StoreEditRequest>
{
    public Ulid StoreId { get; init; } = Ulid.NewUlid();

    [StringLength(256, MinimumLength = 1)]
    public string StoreName { get; set; } = string.Empty;
    
    // Navigation
    public List<Aisle> Aisles { get; init; } = [];
    
    // Patch
    public bool TryGetPatch(ModelStateDictionary modelState, JsonPatchDocument<StoreEditRequest> jsonPatch, out StoreEditRequest patch)
    {
        patch = new StoreEditRequest
        {
            Name = StoreName
        };
        
        return Patch.TryPatch(modelState, this, jsonPatch, ref patch);
    }

    public void ApplyPatch(StoreEditRequest patch)
    {
        StoreName = patch.Name;
    }

    // Errors
    public static NotFound<ErrorResponse> NotFound(Ulid storeId)
    {
        return ErrorResponse.NotFound(storeId, "Store");
    }
    
    public static Conflict<ErrorResponse> ConflictSelected(Ulid storeId)
    {
        return ErrorResponse.Conflict($"Unable to delete store with id {storeId} while it is still the selected store");
    }

    public static InvalidOperationException NotLoaded => new("Store Navigation Property was not loaded");
}