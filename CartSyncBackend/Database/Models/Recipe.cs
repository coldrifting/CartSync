using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Text.Json.Serialization;
using CartSyncBackend.Database.Interfaces;
using CartSyncBackend.Database.Objects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CartSyncBackend.Database.Models;

[PrimaryKey(nameof(RecipeId))]
public class Recipe : IEditable<RecipeEditRequest>
{
    public Ulid RecipeId { get; set; } = Ulid.NewUlid();
    
    [StringLength(255, MinimumLength = 1)]
    public string RecipeName { get; set; } = string.Empty;
    
    [StringLength(255)]
    public string Url { get; set; } = string.Empty;
    
    public bool IsPinned { get; set; }
    public int CartAmount { get; set; }

    // Navigation
    [JsonIgnore] 
    public List<RecipeInstruction> RecipeInstructions { get; set; } = [];

    [JsonIgnore] 
    public List<RecipeSection> RecipeSections  { get; set; } = [];
        
    // Projections
    public RecipeResponse ToNewResponse =>
        new()
        {
            RecipeId = RecipeId,
            RecipeName = RecipeName,
            Url = Url,
            IsPinned = IsPinned,
            CartAmount = CartAmount
        };
    
    public static Expression<Func<Recipe, RecipeResponse>> ToResponse =>
        recipe => new RecipeResponse
        {
            RecipeId = recipe.RecipeId,
            RecipeName = recipe.RecipeName,
            Url = recipe.Url,
            IsPinned = recipe.IsPinned,
            CartAmount = recipe.CartAmount,
            RecipeInstructionsResponse = recipe.RecipeInstructions
                .AsQueryable()
                .OrderBy(ri => ri.SortOrder)
                .Select(RecipeInstruction.ToResponse)
                .ToList(),
            RecipeSectionsResponse = recipe.RecipeSections
                .AsQueryable()
                .OrderBy(rs => rs.SortOrder)
                .Select(RecipeSection.ToResponse)
                .ToList()
        };
    
    // Conversion and Validation
    public RecipeEditRequest ToEditRequest()
    {
        return new RecipeEditRequest()
        {
            CartAmount = CartAmount,
            RecipeName = RecipeName,
            Url = Url,
            IsPinned = IsPinned
        };
    }

    public void UpdateFromEditRequest(RecipeEditRequest editRequest)
    {
        CartAmount = editRequest.CartAmount;
        RecipeName = editRequest.RecipeName;
        Url = editRequest.Url;
        IsPinned = editRequest.IsPinned;
    }
    
    // Errors
    public static NotFoundObjectResult NotFound(Ulid recipeId) => 
        Error.NotFound(recipeId, "Recipe");
}

public class RecipeResponse
{
    public Ulid RecipeId { get; set; }
    
    public string RecipeName { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public bool IsPinned { get; set; }
    public int CartAmount { get; set; }

    public List<RecipeInstructionResponse> RecipeInstructionsResponse { get; set; } = [];
    public List<RecipeSectionResponse> RecipeSectionsResponse { get; set; } = [];
}

public class RecipeEditRequest
{
    [StringLength(255, MinimumLength = 1)]
    public required string RecipeName  { get; set; }
    
    [StringLength(255)]
    public required string Url { get; set; }
    
    public required bool IsPinned { get; set; }
    public required int CartAmount { get; set; }
}