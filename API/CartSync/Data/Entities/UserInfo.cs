using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CartSync.Data.Entities;

[PrimaryKey(nameof(UserId))]
public class UserInfo
{
    public Ulid UserId { get; init; }
    public Ulid StoreId { get; set; }

    public DateTime CartLastGeneratedTime { get; set; } = DateTime.UnixEpoch;
    public DateTime CartSelectionLastUpdatedTime { get; set; } = DateTime.UnixEpoch;

    // Navigation
    [ForeignKey(nameof(UserId))]
    public User User { set; get => field ?? throw User.NotLoaded; }
    
    [ForeignKey(nameof(StoreId))]
    public Store Store { set; get => field ?? throw Store.NotLoaded; }
}