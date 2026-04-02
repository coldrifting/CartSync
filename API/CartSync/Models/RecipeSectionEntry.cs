using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Text.Json.Serialization;
using CartSync.Controllers.Core;
using CartSync.Models.Interfaces;
using CartSync.Objects;
using CartSync.Utils;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace CartSync.Models;

[PrimaryKey(nameof(RecipeSectionEntryId))]
public class RecipeSectionEntry : IEditable<RecipeSectionEntryEditRequest>, IResponse<RecipeSectionEntry, RecipeSectionEntryResponse>
{
    public Ulid RecipeSectionEntryId { get; init; } = Ulid.NewUlid();
    
    public Ulid RecipeSectionId { get; init; }
    public Ulid ItemId { get; set; }
    public Ulid? PrepId { get; set; }
    public Amount Amount { get; set; } = new();
    
    // Navigation
    [JsonIgnore]
    [ForeignKey(nameof(RecipeSectionId))]
    public RecipeSection RecipeSection { set; get => field ?? throw RecipeSection.NotLoaded; }
    
    [JsonIgnore]
    [ForeignKey(nameof(ItemId))]
    public Item Item { set; get => field ?? throw Item.NotLoaded; }

    [JsonIgnore]
    [ForeignKey(nameof(PrepId))]
    public Prep? Prep { get; set; }
    
    // Projections
    public RecipeSectionEntryResponse ToNewResponse =>
        new()
        {
            RecipeSectionEntryId = RecipeSectionEntryId,
            RecipeSectionId = RecipeSectionId,
            Amount = Amount,
            Item = Item.ToMinimalResponse.Compile()(Item),
            Prep = Prep != null ? Prep.ToResponse.Compile()(Prep) : null
        };
    
    public static Expression<Func<RecipeSectionEntry, RecipeSectionEntryResponse>> ToResponse =>
        recipeSectionEntry => new RecipeSectionEntryResponse
        {
            RecipeSectionEntryId = recipeSectionEntry.RecipeSectionEntryId,
            RecipeSectionId = recipeSectionEntry.RecipeSectionId,
            Item = Item.ToMinimalResponse.Compile()(recipeSectionEntry.Item),
            Prep = recipeSectionEntry.Prep != null 
                ? Prep.ToResponse.Compile()(recipeSectionEntry.Prep) 
                : null,
            Amount = recipeSectionEntry.Amount
        };
    
    // Conversion and Validation
    public RecipeSectionEntryEditRequest ToEditRequest(Ulid? storeId = null)
    {
        return new RecipeSectionEntryEditRequest
        {
            ItemId = ItemId,
            PrepId = PrepId,
            Amount = Amount
        };
    }
    
    /// Requires RecipeSectionEntry.RecipeSection.RecipeSectionEntries Navigation to work
    public void UpdateFromEditRequest(RecipeSectionEntryEditRequest editRequest)
    {
        ItemId = editRequest.ItemId;
        PrepId = editRequest.PrepId;
        Amount = editRequest.Amount;
    }
    
    // Errors
    public static NotFound<Error> NotFound(Ulid recipeSectionEntryId) => 
        Error.NotFound(recipeSectionEntryId, "Recipe Section Entry");
    
    public static NotFound<Error> NotFoundUnderRecipeSection(Ulid recipeSectionEntryId, Ulid recipeSectionId) => 
        Error.NotFoundUnder(recipeSectionEntryId, "Recipe Section Entry", recipeSectionId, "Recipe Section");
    
    public static NotFound<Error> NotFoundUnderRecipe(Ulid recipeSectionEntryId, Ulid recipeId) => 
        Error.NotFoundUnder(recipeSectionEntryId, "Recipe Section Entry", recipeId, "Recipe");
}

public record RecipeSectionEntryResponse
{
    public required Ulid RecipeSectionEntryId { get; init; }
    public required Ulid RecipeSectionId { get; init; }
    public required ItemMinimalResponse Item { get; init; }
    public required PrepResponse? Prep { get; init; }
    public required Amount Amount { get; init; }
}

public record RecipeSectionEntryAddRequest
{
    public required Ulid RecipeSectionId { get; init; }
    public required Ulid ItemId { get; init; }
    public required Ulid? PrepId { get; init; }
    public required Amount Amount { get; init; }
}

public record RecipeSectionEntryEditRequest
{
    public required Ulid ItemId { get; init; }
    public required Ulid? PrepId { get; init; }
    public required Amount Amount { get; init; }
}