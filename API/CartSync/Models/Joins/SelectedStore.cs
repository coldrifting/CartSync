using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CartSync.Models.Joins;

[PrimaryKey(nameof(UserId))]
public class SelectedStore
{
    public Ulid UserId { get; init; }
    public Ulid StoreId { get; set; }
    
    // Navigation
    [ForeignKey(nameof(UserId))]
    public User User { set; get => field ?? throw User.NotLoaded; }
    
    [ForeignKey(nameof(StoreId))]
    public Store Store { set; get => field ?? throw Store.NotLoaded; }
}