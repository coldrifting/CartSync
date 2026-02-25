using CartSyncBackend.Database.Models;
using CartSyncBackend.Database.Objects;
using Microsoft.EntityFrameworkCore;

namespace CartSyncBackend.Database;

public class CartSyncContext(DbContextOptions<CartSyncContext> options) : DbContext(options)
{
    public DbSet<Store> Stores { get; set; }
    public DbSet<Aisle> Aisles { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<ItemAisle> ItemAisles { get; set; }
    public DbSet<Prep> Preps { get; set; }
    public DbSet<ItemPrep> ItemPreps { get; set; }

    public DbSet<Recipe> Recipes { get; set; }
    public DbSet<RecipeStep> RecipeSteps { get; set; }
    public DbSet<RecipeSection> RecipeSections { get; set; }
    public DbSet<RecipeSectionEntry> RecipeSectionEntries { get; set; }

    public static string DefaultPath => Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CartSync.db");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Item>()
            .HasMany<Prep>(i => i.Preps)
            .WithMany(p => p.Items)
            .UsingEntity<ItemPrep>();

        modelBuilder.Entity<Aisle>()
            .HasMany<Item>(a => a.Items)
            .WithMany(i => i.Aisles)
            .UsingEntity<ItemAisle>();

        /*
        modelBuilder.Entity<Recipe>()
            .HasMany<RecipeStep>(r => r.RecipeSteps)
            .WithOne(r => r.Recipe);
        
        modelBuilder.Entity<Recipe>()
            .HasMany(r => r.RecipeSections)
            .WithOne(r => r.Recipe);
        
        modelBuilder.Entity<RecipeSection>()
            .HasMany(r => r.RecipeSectionEntries)
            .WithOne(r => r.RecipeSection);
            */
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options
            .UseSqlite($"Data Source={DefaultPath};foreign keys=true;")
            .EnableSensitiveDataLogging();
    
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<Amount>()
            .HaveConversion<AmountConverter>();
        
        configurationBuilder
            .Properties<BayType>()
            .HaveConversion<BayTypeConverter>();
        
        configurationBuilder
            .Properties<UnitType>()
            .HaveConversion<UnitTypeConverter>();

        configurationBuilder
            .Properties<ItemTemp>()
            .HaveConversion<ItemTempConverter>();
        
        configurationBuilder
            .Properties<Ulid>()
            .HaveMaxLength(26)
            .AreUnicode(false)
            .HaveConversion<UlidConverter>();
    }
    
    public void Seed()
    {
        Stores.Seed();
        Aisles.Seed();
        Items.Seed();
        ItemAisles.Seed();
        Preps.Seed();
        ItemPreps.Seed();

        Recipes.Seed();
        RecipeSteps.Seed();
        RecipeSections.Seed();
        RecipeSectionEntries.Seed();
        
        SaveChanges();
    }
}