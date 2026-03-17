using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using CartSyncBackend.Database.Interfaces;
using CartSyncBackend.Database.Objects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CartSyncBackend.Database.Models;

[PrimaryKey(nameof(PrepId))]
public class Prep : IEditable<PrepEditRequest>
{
    public Ulid PrepId { get; init; } = Ulid.NewUlid();
    
    [StringLength(256, MinimumLength = 1)] 
    public string PrepName { get; set; } = string.Empty;

    // Navigation
    public List<Item> Items { get; set; } = null!;
    public List<ItemPrep> ItemPreps { get; set; } = null!;
    
    // Projections
    public static Expression<Func<Prep, PrepResponse>> ToResponse =>
        prep => new PrepResponse
        {
            PrepId = prep.PrepId,
            PrepName = prep.PrepName
        };

    // Conversion and Validation
    public PrepEditRequest ToEditRequest()
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
    public static NotFoundObjectResult NotFound(Ulid prepId) => 
        Error.NotFound(prepId, "Prep");
}

public class PrepResponse
{
    public Ulid PrepId { get; init; }
    public string PrepName { get; init; } = string.Empty;
    
    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj is not PrepResponse other)
        {
            return false;
        }

        return PrepId.Equals(other.PrepId) && PrepName == other.PrepName;
    }

    public override int GetHashCode()
    {
        return PrepId.GetHashCode();
    }
}

public class PrepAddRequest
{
    [StringLength(256, MinimumLength = 1)] 
    public required string PrepName { get; set; }
}

public class PrepEditRequest
{
    [StringLength(256, MinimumLength = 1)] 
    public required string PrepName { get; set; }
}