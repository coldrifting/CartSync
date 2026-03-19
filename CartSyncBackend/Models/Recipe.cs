using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Text.Json.Serialization;
using CartSyncBackend.Controllers.Core;
using CartSyncBackend.Models.Interfaces;
using CartSyncBackend.Objects;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace CartSyncBackend.Models;

[PrimaryKey(nameof(RecipeId))]
public class Recipe : IEditable<RecipeEditRequest>, IResponse<Recipe, RecipeResponse>
{
    public Ulid RecipeId { get; init; } = Ulid.NewUlid();
    
    [StringLength(255, MinimumLength = 1)]
    public required string RecipeName { get; set; }
    
    [StringLength(2048)]
    public string Url { get; set; } = string.Empty;
    
    public bool IsPinned { get; set; }
    public int CartAmount { get; set; }

    // Navigation
    [JsonIgnore] 
    public List<RecipeInstruction> RecipeInstructions { get; init; } = [];

    [JsonIgnore] 
    public List<RecipeSection> RecipeSections  { get; init; } = [];
        
    // Projections
    public static Expression<Func<Recipe, RecipeResponse>> ToResponse =>
        recipe =>
            new RecipeResponse
            {
                RecipeId = recipe.RecipeId,
                RecipeName = recipe.RecipeName,
                Url = recipe.Url,
                IsPinned = recipe.IsPinned,
                RecipeInstructionsResponse = recipe.RecipeInstructions
                    .AsQueryable()
                    .OrderBy(ri => ri.SortOrder)
                    .Select(RecipeInstruction.ToResponse)
                    .ToImmutableList()
                    .WithValueSemantics(),
                RecipeSectionsResponse = recipe.RecipeSections
                    .AsQueryable()
                    .OrderBy(rs => rs.SortOrder)
                    .Select(RecipeSection.ToResponse)
                    .ToImmutableList()
                    .WithValueSemantics(),
            };
    
    // Since new objects will have no children we can skip querying the db after inserting
    public RecipeResponse ToNewResponse =>
        new()
        {
            RecipeId = RecipeId,
            RecipeName = RecipeName,
            Url = Url,
            IsPinned = IsPinned,
            RecipeInstructionsResponse = [],
            RecipeSectionsResponse = [],
        };
    
    // Conversion and Validation
    public RecipeEditRequest ToEditRequest(Ulid? storeId)
    {
        return new RecipeEditRequest
        {
            RecipeName = RecipeName,
            Url = Url,
            IsPinned = IsPinned
        };
    }

    public void UpdateFromEditRequest(RecipeEditRequest editRequest)
    {
        RecipeName = editRequest.RecipeName;
        Url = editRequest.Url;
        IsPinned = editRequest.IsPinned;
    }
    
    // Errors
    public static NotFound<Error> NotFound(Ulid recipeId) => 
        Error.NotFound(recipeId, "Recipe");
    
    public static InvalidOperationException NotLoaded => new("Recipe Navigation Property was not loaded");
}

public record RecipeResponse
{
    public required Ulid RecipeId { get; init; }
    
    public required string RecipeName { get; init; }
    public required string Url { get; init; }
    public required bool IsPinned { get; init; }

    public required ReadOnlyList<RecipeInstructionResponse> RecipeInstructionsResponse { get; init; }
    public required ReadOnlyList<RecipeSectionResponse> RecipeSectionsResponse { get; init; }

    public RecipeMinimalResponse ToMinimalResponse =>
        new()
        {
            RecipeId = RecipeId,
            RecipeName = RecipeName,
            Url = Url,
            IsPinned = IsPinned,
        };
}

public record RecipeMinimalResponse
{
    public required Ulid RecipeId { get; init; }
    
    public required string RecipeName { get; init; }
    public required string Url { get; init; }
    public required bool IsPinned { get; init; }
}

public class RecipeEditRequest
{
    [Required, StringLength(255, MinimumLength = 1)]
    public required string RecipeName  { get; init; }
    
    [Required, StringLength(2048)]
    public required string Url { get; init; }
    
    [Required] public required bool IsPinned { get; init; }
}