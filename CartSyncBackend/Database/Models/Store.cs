using System.ComponentModel.DataAnnotations;

namespace CartSyncBackend.Database.Models;

public class Store
{
    [Key]
    [Required]
    public Ulid StoreId { get; init; } = Ulid.NewUlid();

    [StringLength(256)] 
    [Required]
    public string StoreName { get; set; } = null!;

    // Navigation
    public virtual ICollection<Aisle> Aisles { get; init; } = null!;
    
    public StoreResponse ToResponse()
    {
        return new StoreResponse
        {
            StoreId = StoreId,
            StoreName = StoreName
        };
    }
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