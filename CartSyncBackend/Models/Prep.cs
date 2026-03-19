using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using CartSyncBackend.Controllers.Core;
using CartSyncBackend.Models.Interfaces;
using CartSyncBackend.Models.Joins;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace CartSyncBackend.Models;

[PrimaryKey(nameof(PrepId))]
public class Prep : IEditable<PrepEditRequest>, IResponse<Prep, PrepResponse>
{
    public Ulid PrepId { get; init; } = Ulid.NewUlid();
    
    [StringLength(256, MinimumLength = 1)] 
    public required string PrepName { get; set; }

    // Navigation
    public List<Item> Items { get; init; } = [];
    public List<ItemPrep> ItemPreps { get; init; } = [];
    public List<RecipeSectionEntry> RecipeSectionEntries { get; init; } = [];
    
    // Projections
    public static Expression<Func<Prep, PrepResponse>> ToResponse =>
        prep => new PrepResponse
        {
            PrepId = prep.PrepId,
            PrepName = prep.PrepName
        };
    
    public PrepResponse ToNewResponse =>
        ToResponse.Compile()(this);

    // Conversion and Validation
    public PrepEditRequest ToEditRequest(Ulid? storeId = null)
    {
        return new PrepEditRequest
        {
            PrepName = PrepName
        };
    }

    public void UpdateFromEditRequest(PrepEditRequest editRequest)
    {
        PrepName = editRequest.PrepName;
    }
    
    // Errors
    public static NotFound<Error> NotFound(Ulid prepId) => 
        Error.NotFound(prepId, "Prep");
    
    public static InvalidOperationException NotLoaded => new("Prep Navigation Property was not loaded");
}

public record PrepResponse
{
    public required Ulid PrepId { get; init; }
    public required string PrepName { get; init; }
}

public record PrepAddRequest
{
    [Required, StringLength(256, MinimumLength = 1)] 
    public required string PrepName { get; init; }
}

public record PrepEditRequest
{
    [Required, StringLength(256, MinimumLength = 1)] 
    public required string PrepName { get; init; }
}