using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CartSync.Data.Requests;
using CartSync.Data.Responses;
using CartSync.Interfaces;
using CartSync.Utils;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace CartSync.Data.Entities;

[PrimaryKey(nameof(AisleId))]
public class Aisle : ISortable
{
    public Ulid AisleId { get; init; } = Ulid.NewUlid();
    public Ulid StoreId { get; init; }
    
    [StringLength(256, MinimumLength = 1)]
    public string AisleName { get; set; } = "(Default)";
    public int SortOrder { get; set; } = -1;
    
    // Navigation
    [ForeignKey(nameof(StoreId))]
    public Store Store { init; get => field ?? throw Store.NotLoaded; }

    public List<Item> Items { get; init; } = [];
 
    // Patch
    public bool TryGetPatch(ModelStateDictionary modelState, JsonPatchDocument<AisleEditRequest> jsonPatch, out AisleEditRequest patch)
    {
        patch = new AisleEditRequest
        {
            Name = AisleName,
            SortOrder = SortOrder,
        };
        
        return Patch.TryPatch(modelState, this, jsonPatch, ref patch);
    }

    public void ApplyPatch(AisleEditRequest patch)
    {
        AisleName = patch.Name;
        Sort.Reorder(Store.Aisles, SortOrder, patch.SortOrder);
    }

    // Errors
    public static NotFound<ErrorResponse> NotFound(Ulid aisleId) =>
        ErrorResponse.NotFound(aisleId, "Aisle");
    
    public static NotFound<ErrorResponse> NotFoundUnderStore(Ulid aisleId, Ulid storeId) =>
        ErrorResponse.NotFoundUnder(aisleId, "Aisle", storeId, "Store");
    
    public static InvalidOperationException NotLoaded => new("Aisle Navigation Property was not loaded");
}