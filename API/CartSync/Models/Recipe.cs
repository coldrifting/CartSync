using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Text.Json.Serialization;
using CartSync.Controllers.Core;
using CartSync.Models.Interfaces;
using CartSync.Objects;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace CartSync.Models;

[PrimaryKey(nameof(RecipeId))]
public class Recipe : IEditable<RecipeEditRequest>, IResponse<Recipe, RecipeResponse>
{
    public Ulid RecipeId { get; init; } = Ulid.NewUlid();
    
    [StringLength(255, MinimumLength = 1)]
    public required string RecipeName { get; set; }
    
    [StringLength(2048)]
    public string Url { get; set; } = string.Empty;
    
    public bool IsPinned { get; set; }
    public int CartQuantity { get; set; }

    // Navigation
    [JsonIgnore] 
    public List<RecipeStep> Steps { get; init; } = [];

    [JsonIgnore] 
    public List<RecipeSection> Sections  { get; init; } = [];
        
    // Projections
    public static Expression<Func<Recipe, RecipeResponse>> ToResponse =>
        recipe =>
            new RecipeResponse
            {
                Id = recipe.RecipeId,
                Name = recipe.RecipeName,
                Url = recipe.Url,
                IsPinned = recipe.IsPinned,
                Steps = recipe.Steps
                    .AsQueryable()
                    .OrderBy(ri => ri.SortOrder)
                    .Select(RecipeStep.ToResponse)
                    .ToImmutableList()
                    .WithValueSemantics(),
                Sections = recipe.Sections
                    .AsQueryable()
                    .OrderBy(rs => rs.SortOrder)
                    .Select(RecipeSection.ToResponse)
                    .ToImmutableList()
                    .WithValueSemantics(),
            };
    
    public static Expression<Func<Recipe, RecipeMinimalResponse>> ToMinimalResponse =>
        recipe =>
            new RecipeMinimalResponse
            {
                Id = recipe.RecipeId,
                Name = recipe.RecipeName,
                IsPinned = recipe.IsPinned
            };
    
    // Since new objects will have no children we can skip querying the db after inserting
    public RecipeResponse ToNewResponse =>
        new()
        {
            Id = RecipeId,
            Name = RecipeName,
            Url = Url,
            IsPinned = IsPinned,
            Steps = [],
            Sections = [],
        };
    
    // Conversion and Validation
    public RecipeEditRequest ToEditRequest(Ulid? storeId)
    {
        return new RecipeEditRequest
        {
            Name = RecipeName,
            Url = Url,
            IsPinned = IsPinned
        };
    }

    public void UpdateFromEditRequest(RecipeEditRequest editRequest)
    {
        RecipeName = editRequest.Name;
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
    public required Ulid Id { get; init; }
    
    public required string Name { get; init; }
    public required string Url { get; init; }
    public required bool IsPinned { get; init; }

    public required ReadOnlyList<RecipeStepResponse> Steps { get; init; }
    public required ReadOnlyList<RecipeSectionResponse> Sections { get; init; }

    [JsonIgnore] 
    public RecipeMinimalResponse ToMinimalResponse =>
        new()
        {
            Id = Id,
            Name = Name,
            IsPinned = IsPinned,
        };
}

public record RecipeMinimalResponse
{
    public required Ulid Id { get; init; }
    
    public required string Name { get; init; }
    public required bool IsPinned { get; init; }
}

public class RecipeAddRequest
{
    [Required, StringLength(255, MinimumLength = 1)]
    public required string Name  { get; init; }
}

public class RecipeEditRequest
{
    [Required, StringLength(255, MinimumLength = 1)]
    public required string Name  { get; init; }
    
    [Required, StringLength(2048)]
    public required string Url { get; init; }
    
    [Required] public required bool IsPinned { get; init; }
}