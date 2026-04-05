using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Text.Json.Serialization;
using CartSync.Controllers.Core;
using CartSync.Models.Interfaces;
using CartSync.Objects;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace CartSync.Models;

[PrimaryKey(nameof(RecipeEntryId))]
public class RecipeEntry : IEditable<RecipeEntryEditRequest>, IResponse<RecipeEntry, RecipeEntryResponse>
{
    public Ulid RecipeEntryId { get; init; } = Ulid.NewUlid();
    
    public Ulid RecipeSectionId { get; init; }
    public Ulid ItemId { get; init; }
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
    public RecipeEntryResponse ToNewResponse =>
        new()
        {
            RecipeEntryId = RecipeEntryId,
            Amount = Amount,
            Item = Item.ToMinimalResponse.Compile()(Item),
            Prep = Prep != null ? Prep.ToResponse.Compile()(Prep) : null
        };
    
    public static Expression<Func<RecipeEntry, RecipeEntryResponse>> ToResponse =>
        recipeSectionEntry => new RecipeEntryResponse
        {
            RecipeEntryId = recipeSectionEntry.RecipeEntryId,
            Item = Item.ToMinimalResponse.Compile()(recipeSectionEntry.Item),
            Prep = recipeSectionEntry.Prep != null 
                ? Prep.ToResponse.Compile()(recipeSectionEntry.Prep) 
                : null,
            Amount = recipeSectionEntry.Amount
        };
    
    // Conversion and Validation
    public RecipeEntryEditRequest ToEditRequest(Ulid? storeId = null)
    {
        return new RecipeEntryEditRequest
        {
            PrepId = PrepId,
            Amount = Amount
        };
    }
    
    /// Requires RecipeSectionEntry.RecipeSection.RecipeSectionEntries Navigation to work
    public void UpdateFromEditRequest(RecipeEntryEditRequest editRequest)
    {
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
    
    public static Conflict<Error> AlreadyExists(Ulid itemId, Ulid? prepId) => 
        Error.AlreadyExists("Recipe Section Entry", itemId, "Item", prepId, "Prep");
}

public record RecipeEntryResponse
{
    public required Ulid RecipeEntryId { get; init; }
    public required ItemMinimalResponse Item { get; init; }
    public required PrepResponse? Prep { get; init; }
    public required Amount Amount { get; init; }
}

public record RecipeEntryAddRequest
{
    public required Ulid ItemId { get; init; }
    public required Ulid? PrepId { get; init; }
    public required Amount Amount { get; init; }
}

public record RecipeEntryEditRequest
{
    public required Ulid? PrepId { get; init; }
    public required Amount Amount { get; init; }
}