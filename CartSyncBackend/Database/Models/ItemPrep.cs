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
    public Item Item { get; set; } = null!;
    
    [ForeignKey(nameof(PrepId))]
    public Prep Prep { get; set; } = null!;
}