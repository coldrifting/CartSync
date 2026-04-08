using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CartSync.Data.Entities;

[PrimaryKey(nameof(UserId))]
public class UserSelectedStore
{
    public Ulid UserId { get; init; }
    public Ulid StoreId { get; set; }
    
    // Navigation
    [ForeignKey(nameof(UserId))]
    public User User { set; get => field ?? throw User.NotLoaded; }
    
    [ForeignKey(nameof(StoreId))]
    public Store Store { set; get => field ?? throw Store.NotLoaded; }
}