using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CartSync.Models.Joins;

[PrimaryKey(nameof(ItemId), nameof(PrepId))]
public class ItemPrep
{
    public Ulid ItemId { get; set; }
    public Ulid PrepId { get; set; }
    
    // Navigation
    [ForeignKey(nameof(ItemId))]
    public Item Item { set; get => field ?? throw Item.NotLoaded; }
    
    [ForeignKey(nameof(PrepId))]
    public Prep Prep { set; get => field ?? throw Prep.NotLoaded; }
}