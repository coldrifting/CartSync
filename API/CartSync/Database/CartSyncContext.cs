using CartSync.Data.Entities;
using CartSync.Objects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CartSync.Database;

public class CartSyncContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    
    public DbSet<Store> Stores { get; set; }
    public DbSet<Aisle> Aisles { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<ItemAisle> ItemAisles { get; set; }
    public DbSet<Prep> Preps { get; set; }
    public DbSet<ItemPrep> ItemPreps { get; set; }

    public DbSet<Recipe> Recipes { get; set; }
    public DbSet<RecipeStep> RecipeSteps { get; set; }
    public DbSet<RecipeSection> RecipeSections { get; set; }
    public DbSet<RecipeEntry> RecipeEntries { get; set; }
    
    public DbSet<UserInfo> UserInfo { get; set; }
    public DbSet<CartSelectItem> CartSelectItems { get; set; } 
    public DbSet<CartEntry> CartEntries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();
            
        modelBuilder.Entity<Item>()
            .HasMany<Prep>(i => i.Preps)
            .WithMany(p => p.Items)
            .UsingEntity<ItemPrep>();

        modelBuilder.Entity<Aisle>()
            .HasMany<Item>(a => a.Items)
            .WithMany(i => i.Aisles)
            .UsingEntity<ItemAisle>();
        
        modelBuilder.Entity<RecipeEntry>()
            .HasIndex(r => new {r.RecipeSectionId, r.ItemId, r.PrepId})
            .IsUnique();
        
        modelBuilder.Entity<CartSelectItem>()
            .HasIndex(ci => new {ci.ItemId, ci.PrepId})
            .IsUnique();
        
        modelBuilder.Entity<CartEntry>()
            .HasIndex(ce => new {ce.ItemId, ce.PrepId})
            .IsUnique();
        
        foreach (IMutableForeignKey relationship in modelBuilder.Model.GetEntityTypes().SelectMany(mutableEntryType => mutableEntryType.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.Cascade;
        }
    }
    
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<Amount>()
            .HaveConversion<Amount.ValueConverter>();
        
        configurationBuilder
            .Properties<AmountGroup>()
            .HaveConversion<AmountGroup.ValueConverter>();
        
        configurationBuilder
            .Properties<Bay>()
            .HaveConversion<string>();
        
        configurationBuilder
            .Properties<UnitType>()
            .HaveConversion<string>();

        configurationBuilder
            .Properties<Temp>()
            .HaveConversion<string>();
        
        configurationBuilder
            .Properties<Ulid>()
            .HaveMaxLength(26)
            .AreUnicode(false)
            .HaveConversion<UlidValueConverter>();
    }
    
    // ReSharper disable once ClassNeverInstantiated.Local
    private class UlidValueConverter() : ValueConverter<Ulid, string>(
        ulid => ulid.ToString(),
        stringValue => Ulid.Parse(stringValue)
    );
    
    public void Seed()
    {
        AddRange(SeedData.SeedData.Users);
        
        AddRange(SeedData.SeedData.Stores);
        AddRange(SeedData.SeedData.Aisles);
        AddRange(SeedData.SeedData.Items);
        AddRange(SeedData.SeedData.ItemAisles);
        AddRange(SeedData.SeedData.Preps);
        AddRange(SeedData.SeedData.ItemPreps);
        AddRange(SeedData.SeedData.Recipes);
        AddRange(SeedData.SeedData.RecipeSteps);
        AddRange(SeedData.SeedData.RecipeSections);
        AddRange(SeedData.SeedData.RecipeEntries);
        
        AddRange(SeedData.SeedData.SelectedStores);
        AddRange(SeedData.SeedData.CartItems);
        
        SaveChanges();
    }
}