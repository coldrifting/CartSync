using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CartSyncBackend.Database.Objects;

[UsedImplicitly]
public class UlidConverter() : ValueConverter<Ulid, string>(
    v => v.ToString(),
    v => Ulid.Parse(v)
);