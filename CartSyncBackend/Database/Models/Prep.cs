using Microsoft.EntityFrameworkCore;

namespace CartSyncBackend.Database.Models;

[PrimaryKey(nameof(PrepId))]
public class Prep
{
    public Ulid PrepId { get; set; } = Ulid.NewUlid();
    public string PrepName { get; set; } = string.Empty;

    // Navigation
    public List<Item> Items { get; set; } = null!;
    public List<ItemPrep> ItemPreps { get; set; } = null!;
}

public class PrepResponse
{
    public Ulid PrepId { get; set; }
    public string PrepName { get; set; } = string.Empty;
}