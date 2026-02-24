using Microsoft.EntityFrameworkCore;

namespace CartSyncBackend.Database.Models;

[PrimaryKey(nameof(PrepId))]
public class Prep
{
    public Ulid PrepId { get; set; }
    public string PrepName { get; set; } = string.Empty;

    public List<Item>? Items { get; set; } = [];

    public PrepResponse ToResponse()
    {
        return new PrepResponse()
        {
            PrepId = PrepId,
            PrepName = PrepName
        };
    }
}

public class PrepResponse
{
    public Ulid PrepId { get; set; }
    public string PrepName { get; set; } = string.Empty;
}