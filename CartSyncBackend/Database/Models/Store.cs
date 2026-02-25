using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CartSyncBackend.Database.Models;

[PrimaryKey(nameof(StoreId))]
public class Store
{
    public Ulid StoreId { get; init; } = Ulid.NewUlid();

    [StringLength(256)]
    public string StoreName { get; set; } = null!;

    // Navigation
    public List<Aisle> Aisles { get; init; } = null!;
}

public class StoreResponse
{
    public Ulid StoreId { get; init; }

    [StringLength(256)]
    public string StoreName { get; init; } = null!;
}

public class StoreAddRenameRequest
{
    [Required]
    [StringLength(256)]
    public required string StoreName { get; init; }
}