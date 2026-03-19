namespace CartSync.Models.Seeding;

// ReSharper disable StringLiteralTypo
public static partial class SeedData
{
    public static List<Prep> Preps =>
    [
        new() { PrepId = Ulid.Parse("01KJ8AQ9DK0ZJXKX98J1JWFVK9"), PrepName = "Crumbled" },
        new() { PrepId = Ulid.Parse("01KJ8AQ9DKPNW8KCR2ZDG05GE7"), PrepName = "Diced" },
        new() { PrepId = Ulid.Parse("01KJ8AQ9DKCDJ9SFX9XXBCG8H1"), PrepName = "Minced" },
        new() { PrepId = Ulid.Parse("01KJ8AQ9DKZ7VQKAC27VRAWBRW"), PrepName = "Shredded" },
        new() { PrepId = Ulid.Parse("01KJ8AQ9DKR01ZTNR87JASJPTV"), PrepName = "Sliced" },
        new() { PrepId = Ulid.Parse("01KJ8AQ9DK9BC9DTDE6KQSQV3Z"), PrepName = "Sliced (Halves)" },
        new() { PrepId = Ulid.Parse("01KJ8AQ9DM0N30S4073K3X3CQ1"), PrepName = "Cooked" }
    ];
}