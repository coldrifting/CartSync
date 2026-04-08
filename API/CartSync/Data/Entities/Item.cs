using System.ComponentModel.DataAnnotations;
using CartSync.Data.Requests;
using CartSync.Data.Responses;
using CartSync.Interfaces;
using CartSync.Objects;
using CartSync.Utils;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace CartSync.Data.Entities;

[PrimaryKey(nameof(ItemId))]
public class Item : IPatchableStoreSpecific<ItemEditRequest>
{
    public Ulid ItemId { get; init; } = Ulid.NewUlid();
    
    [StringLength(256, MinimumLength = 1)]
    public required string ItemName { get; set; }

    public Temp Temp { get; set; }
    public UnitType DefaultUnitType { get; set; }
    public bool UncapCartUnits { get; set; }

    // Navigation
    public List<Prep> Preps { get; init; } = [];
    public List<ItemPrep> ItemPreps { get; init; } = [];
    public List<RecipeEntry> RecipeSectionEntries { get; init; } = [];
    
    // Enforce max of 1 linked aisle per store
    public List<Aisle> Aisles { get; init; } = [];
    public List<ItemAisle> ItemAisles { get; init; } = [];
 
    // Patch
    public bool TryGetPatch(ModelStateDictionary modelState, JsonPatchDocument<ItemEditRequest> jsonPatch, Ulid storeId, out ItemEditRequest patch)
    {
        patch = new ItemEditRequest
        {
            Name = ItemName,
            Temp = Temp,
            Location = ItemAisles
                .AsQueryable()
                .Where(itemAisle => itemAisle.StoreId == storeId)
                .Select(LocationEditRequest.FromItemAisle)
                .FirstOrDefault(),
            DefaultUnitType = DefaultUnitType,
            UncapCartUnits = UncapCartUnits,
            PrepIds = Preps
                .OrderBy(p => p.PrepName)
                .ThenBy(p => p.PrepId)
                .Select(p => p.PrepId)
                .ToList()
        };
        
        return Patch.TryPatch(modelState, this, jsonPatch, ref patch);
    }

    public void ApplyPatch(ItemEditRequest editRequest, Ulid storeId)
    {
        ItemName = editRequest.Name;
        Temp = editRequest.Temp;
        DefaultUnitType = editRequest.DefaultUnitType;
        UncapCartUnits = editRequest.UncapCartUnits;

        ItemPreps.Clear();

        foreach (Ulid prepId in editRequest.PrepIds.ToHashSet())
        {
            ItemPreps.Add(new ItemPrep
            {
                ItemId = ItemId,
                PrepId = prepId
            });
        }

        ItemAisle? currentLocation = ItemAisles.FirstOrDefault(itemAisle => itemAisle.StoreId == storeId);
        if (currentLocation is not null)
        {
            if (editRequest.Location is not null)
            {
                currentLocation.AisleId = editRequest.Location.AisleId;
                currentLocation.Bay = editRequest.Location.Bay;
            }
            else
            {
                ItemAisles.Remove(currentLocation);
            }
        }
        else
        {
            if (editRequest.Location is not null)
            {
                ItemAisles.Add(new ItemAisle
                {
                    ItemId = ItemId,
                    StoreId = storeId,
                    AisleId = editRequest.Location.AisleId,
                    Bay = editRequest.Location.Bay
                });
            }
        }
    }

    // Errors
    public static NotFound<ErrorResponse> NotFound(Ulid itemId) => 
        ErrorResponse.NotFound(itemId, "Item");
    
    public static InvalidOperationException NotLoaded => new("Item Navigation Property was not loaded");
}