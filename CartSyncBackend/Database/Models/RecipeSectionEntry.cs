using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Text.Json.Serialization;
using CartSyncBackend.Database.Interfaces;
using CartSyncBackend.Database.Objects;
using CartSyncBackend.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CartSyncBackend.Database.Models;

[PrimaryKey(nameof(RecipeSectionEntryId))]
public class RecipeSectionEntry : ISortable, IEditable<RecipeSectionEntryEditRequest>
{
    public Ulid RecipeSectionEntryId { get; init; } = Ulid.NewUlid();
    
    public Ulid RecipeSectionId { get; set; }
    public int SortOrder { get; set; }
    public Ulid ItemId { get; set; }
    public Ulid? PrepId { get; set; }
    public Amount Amount { get; set; } = new();
    
    // Navigation
    [JsonIgnore]
    [ForeignKey(nameof(RecipeSectionId))]
    public RecipeSection RecipeSection
    {
        set;
        get => field ?? throw new InvalidOperationException("Uninitialized property: " + nameof(RecipeSection));
    }
    
    [JsonIgnore]
    [ForeignKey(nameof(ItemId))]
    public Item Item
    {
        set;
        get => field ?? throw new InvalidOperationException("Uninitialized property: " + nameof(Item));
    }

    [JsonIgnore]
    [ForeignKey(nameof(PrepId))]
    public Prep? Prep { get; set; }
    
    // Projections
    public RecipeSectionEntryResponse ToNewResponse =>
        new()
        {
            RecipeSectionEntryId = RecipeSectionEntryId,
            RecipeSectionId = RecipeSectionId,
            SortOrder = SortOrder,
            Amount = Amount,
            Item = Item.ToMinimalResponse.Compile()(Item),
            Prep = Prep != null ? Prep.ToResponse.Compile()(Prep) : null
        };
    
    public static Expression<Func<RecipeSectionEntry, RecipeSectionEntryResponse>> ToResponse =>
        recipeSectionEntry => new RecipeSectionEntryResponse
        {
            RecipeSectionEntryId = recipeSectionEntry.RecipeSectionEntryId,
            RecipeSectionId = recipeSectionEntry.RecipeSectionId,
            SortOrder = recipeSectionEntry.SortOrder,
            Item = Item.ToMinimalResponse.Compile()(recipeSectionEntry.Item),
            Prep = recipeSectionEntry.Prep != null 
                ? Prep.ToResponse.Compile()(recipeSectionEntry.Prep) 
                : null,
            Amount = recipeSectionEntry.Amount
        };
    
    // Conversion and Validation
    public RecipeSectionEntryEditRequest ToEditRequest()
    {
        return new RecipeSectionEntryEditRequest()
        {
            ItemId = ItemId,
            PrepId = PrepId,
            Amount = Amount,
            SortOrder = SortOrder
        };
    }
    
    /// Requires RecipeSectionEntry.RecipeSection.RecipeSectionEntries Navigation to work
    public void UpdateFromEditRequest(RecipeSectionEntryEditRequest editRequest)
    {
        ItemId = editRequest.ItemId;
        PrepId = editRequest.PrepId;
        Amount = editRequest.Amount;
        
        int oldIndex = SortOrder;
        RecipeSection.RecipeSectionEntries.Reorder(oldIndex, editRequest.SortOrder);
        //SortOrder = editRequest.SortOrder;
    }
    
    // Errors
    public static NotFoundObjectResult NotFound(Ulid recipeSectionEntryId) => 
        Error.NotFound(recipeSectionEntryId, "Recipe Section Entry");
    
    public static NotFoundObjectResult NotFoundUnderRecipeSection(Ulid recipeSectionEntryId, Ulid recipeSectionId) => 
        Error.NotFoundUnder(recipeSectionEntryId, "Recipe Section Entry", recipeSectionId, "Recipe Section");
    
    public static NotFoundObjectResult NotFoundUnderRecipe(Ulid recipeSectionEntryId, Ulid recipeId) => 
        Error.NotFoundUnder(recipeSectionEntryId, "Recipe Section Entry", recipeId, "Recipe");
}

public class RecipeSectionEntryResponse
{
    public Ulid RecipeSectionEntryId { get; init; }
    public Ulid RecipeSectionId { get; init; }
    public int SortOrder { get; set; }
    public ItemMinimalResponse? Item { get; set; }
    public PrepResponse? Prep { get; set; }
    public Amount Amount { get; init; } = new();
}

public class RecipeSectionEntryAddRequest
{
    public required Ulid RecipeSectionId { get; init; }
    public required Ulid ItemId { get; init; }
    public Ulid? PrepId { get; init; }
    public required Amount Amount { get; init; }
}

public class RecipeSectionEntryEditRequest
{
    public required int SortOrder { get; set; }
    public required Ulid ItemId { get; init; }
    public Ulid? PrepId { get; init; }
    public required Amount Amount { get; init; }
}