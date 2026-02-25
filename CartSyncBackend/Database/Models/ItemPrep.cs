using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CartSyncBackend.Database.Models;

[PrimaryKey(nameof(ItemId), nameof(PrepId))]
public class ItemPrep
{
    public Ulid ItemId { get; set; }
    public Ulid PrepId { get; set; }
    
    // Navigation
    [ForeignKey(nameof(ItemId))]
    public Item Item
    {
        set;
        get => field ?? throw new InvalidOperationException("Uninitialized property: " + nameof(Item));
    }
    
    [ForeignKey(nameof(PrepId))]
    public Prep Prep
    {
        set;
        get => field ?? throw new InvalidOperationException("Uninitialized property: " + nameof(Prep));
    }
}