using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using CartSyncBackend.Database.Objects;
using Microsoft.EntityFrameworkCore;

namespace CartSyncBackend.Database.Models;

[PrimaryKey(nameof(RecipeSectionEntryId))]
public class RecipeSectionEntry
{
    public Ulid RecipeSectionEntryId { get; init; } = Ulid.NewUlid();
    
    public Ulid RecipeSectionId { get; set; }
    public int RecipeSectionEntryIndex { get; set; }
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
}

public class RecipeSectionEntryResponse
{
    public Ulid RecipeSectionEntryId { get; init; }
    public int RecipeSectionEntryIndex { get; set; }
    public ItemResponseNoPrep? Item { get; set; }
    public PrepResponse? Prep { get; set; }
    public Amount Amount { get; set; } = new();
}

public class RecipeSectionEntryAddRequest
{
    public Ulid RecipeSectionId { get; init; }
    public Ulid ItemId { get; init; }
    public Ulid? PrepId { get; init; }
    public Amount Amount { get; init; }
}


public class RecipeSectionEntryEditRequest
{
    public int? RecipeSectionEntryIndex { get; set; }
    public Ulid? ItemId { get; init; }
    public Ulid? PrepId { get; init; }
    public Amount? Amount { get; init; }
    
    // Required for ability to unset prep
    public bool UpdatePrep { get; init; }
}