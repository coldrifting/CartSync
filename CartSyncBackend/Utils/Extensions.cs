using CartSyncBackend.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace CartSyncBackend.Utils;

public static class Extensions
{
    public static Task<Store?> GetAsync(this IQueryable<Store> query, Ulid key) => 
        query.FirstOrDefaultAsync(s => s.StoreId == key);
    
    public static Task<Aisle?> GetAsync(this IQueryable<Aisle> query, Ulid key) => 
        query.FirstOrDefaultAsync(a => a.AisleId == key);
    
    public static Task<Item?> GetAsync(this IQueryable<Item> query, Ulid key) => 
        query.FirstOrDefaultAsync(i => i.ItemId == key);
    
    public static Task<ItemResponse?> GetAsync(this IQueryable<ItemResponse> query, Ulid key) => 
        query.FirstOrDefaultAsync(i => i.ItemId == key);
    
    public static Task<ItemAisle?> GetAsync(this IQueryable<ItemAisle> query, (Ulid itemId, Ulid storeId) key) => 
        query.FirstOrDefaultAsync(ip => ip.ItemId == key.itemId && ip.StoreId == key.storeId);
    
    public static Task<Prep?> GetAsync(this IQueryable<Prep> query, Ulid key) =>
        query.FirstOrDefaultAsync(p => p.PrepId == key);
    
    public static Task<ItemPrep?> GetAsync(this IQueryable<ItemPrep> query, (Ulid itemId, Ulid prepId) key) =>
        query.FirstOrDefaultAsync(ip => ip.ItemId == key.itemId && ip.PrepId == key.prepId);
    
    public static Task<Recipe?> GetAsync(this IQueryable<Recipe> query, Ulid key) =>
        query.FirstOrDefaultAsync(r => r.RecipeId == key);
    
    public static Task<RecipeInstruction?> GetAsync(this IQueryable<RecipeInstruction> query, Ulid key) =>
        query.FirstOrDefaultAsync(ri => ri.RecipeInstructionId == key);
    
    public static Task<RecipeSection?> GetAsync(this IQueryable<RecipeSection> query, Ulid key) =>
        query.FirstOrDefaultAsync(rs => rs.RecipeSectionId == key);
    
    public static Task<RecipeSectionEntry?> GetAsync(this IQueryable<RecipeSectionEntry> query, Ulid key) =>
        query.FirstOrDefaultAsync(rse => rse.RecipeSectionEntryId == key);

}