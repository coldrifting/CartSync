using CartSync.Models.Joins;
using CartSync.Models.Seeding;
using CartSync.Objects;
using CartSync.Objects.Enums;
using CartSync.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CartSync.Models;

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
    
    public DbSet<SelectedStore> SelectedStores { get; set; }

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
            .Properties<BayType>()
            .HaveConversion<BayTypeEx.ValueConverter>();
        
        configurationBuilder
            .Properties<UnitType>()
            .HaveConversion<UnitTypeEx.ValueConverter>();

        configurationBuilder
            .Properties<ItemTemp>()
            .HaveConversion<ItemTempEx.ItemTempConverter>();
        
        configurationBuilder
            .Properties<Ulid>()
            .HaveMaxLength(26)
            .AreUnicode(false)
            .HaveConversion<Extensions.UlidValueConverter>();
    }
    
    public void Seed()
    {
        AddRange(SeedData.Users);
        
        AddRange(SeedData.Stores);
        AddRange(SeedData.Aisles);
        AddRange(SeedData.Items);
        AddRange(SeedData.ItemAisles);
        AddRange(SeedData.Preps);
        AddRange(SeedData.ItemPreps);
        AddRange(SeedData.Recipes);
        AddRange(SeedData.RecipeSteps);
        AddRange(SeedData.RecipeSections);
        AddRange(SeedData.RecipeEntries);
        
        AddRange(SeedData.SelectedStores);
        
        SaveChanges();
    }
}