using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CartSyncBackend.Database.Models;

[PrimaryKey(nameof(StoreId))]
public class Store
{
    public Ulid StoreId { get; init; } = Ulid.NewUlid();

    [StringLength(256)]
    public string StoreName { get; set; } = string.Empty;

    // Navigation
    public List<Aisle> Aisles { get; init; } = [];
}

public class StoreResponse
{
    public Ulid StoreId { get; init; }
    public string StoreName { get; set; } = string.Empty;
}

public class StoreAddRenameRequest
{
    [Required]
    [StringLength(256)]
    public string StoreName { get; set; } = string.Empty;
}