using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using CartSyncBackend.Database.Objects;
using Microsoft.EntityFrameworkCore;

namespace CartSyncBackend.Database.Models;

[PrimaryKey(nameof(RecipeSectionEntryId))]
public class RecipeSectionEntry
{
    public Ulid RecipeSectionEntryId { get; init; }
    
    public Ulid RecipeSectionId { get; set; }
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
    public ItemResponseNoPrep? Item { get; set; }
    public PrepResponse? Prep { get; set; }
    public Amount Amount { get; set; } = new();
}