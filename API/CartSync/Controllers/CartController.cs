using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using CartSync.Controllers.Core;
using CartSync.Data.Entities;
using CartSync.Data.Misc;
using CartSync.Data.Requests;
using CartSync.Data.Responses;
using CartSync.Database;
using CartSync.Objects;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CartSync.Controllers;

[Tags("Cart")]
public class CartController(CartSyncContext context) : ControllerCore(context)
{
    [HttpPost]
    [Route("/api/cart/generate")]
    public async Task<NoContent> Generate()
    {
        ImmutableList<CartEntryPrototype> cartItemsExtra = Db.CartSelectItems
            .Include(cartSelectItem => cartSelectItem.Item)
            .Select(CartEntryPrototype.FromCartItem)
            .ToImmutableList();

        ImmutableList<CartEntryPrototype> cartItemsFromRecipes = Db.Recipes
            .Include(recipe => recipe.Sections)
            .ThenInclude(recipeSection => recipeSection.Entries)
            .ThenInclude(entry => entry.Item)
            .Where(recipe => recipe.CartQuantity > 0)
            .SelectMany(recipe => recipe.Sections)
            .SelectMany(section => section.Entries)
            .Select(CartEntryPrototype.FromRecipeEntry)
            .ToImmutableList();
        
        ImmutableList<CartEntryPrototype> finalCartInfo = cartItemsFromRecipes
            .Concat(cartItemsExtra)
            .GroupBy(ItemPrepPair.FromCartEntryPrototype)
            .Select(CartEntryPrototype.Aggregate)
            .ToImmutableList();

        Store store = await GetSelectedStore();
        List<ItemAisle> lookup = Db.ItemAisles
            .Include(itemAisle => itemAisle.Aisle)
            .Where(itemAisle => itemAisle.StoreId == store.StoreId)
            .ToList();

        // Reset cart
        await Db.CartEntries.ExecuteDeleteAsync();
        foreach (CartEntryPrototype cartInfo in finalCartInfo)
        {
            ItemAisle? aisleInfo = lookup.FirstOrDefault(i => i.ItemId == cartInfo.ItemId);
            Db.CartEntries.Add(new CartEntry
            {
                ItemId = cartInfo.ItemId,
                PrepId = cartInfo.PrepId,
                AisleId = aisleInfo?.AisleId,
                Bay = aisleInfo?.Bay ?? Bay.Center,
                Amounts = cartInfo.Amounts
            });
        }

        UserInfo? info = await GetUserInfo();
        info?.CartLastGeneratedTime = DateTime.UtcNow;

        await Db.SaveChangesAsync();

        return TypedResults.NoContent();
    }

    [HttpGet]
    [Route("/api/cart")]
    public async Task<Ok<CartEntryResponse>> Get()
    {
        Store store = await GetSelectedStore();

        ReadOnlyList<CartEntryAisleResponse> aisles = Db.CartEntries
            .Include(entry => entry.Aisle)
            .GroupBy(entry => entry.Aisle)
            .Select(CartEntryAisleResponsePrototype.FromAisleGroup)
            .AsEnumerable()
            .OrderBy(entry => entry.SortOrder)
            .Select(CartEntryAisleResponse.FromPrototype)
            .ToReadOnlyList();

        return TypedResults.Ok(new CartEntryResponse
        {
            StoreId = store.StoreId,
            StoreName = store.StoreName,
            Aisles = aisles
        });
    }

    [HttpPut]
    [Route("/api/cart/entries/{entryId}/edit")]
    public async Task<Results<NoContent, BadRequest<ErrorResponse>, NotFound<ErrorResponse>>> Check(Ulid entryId, [FromQuery] bool isChecked)
    {
        CartEntry? cartEntry = await Db.CartEntries.FindAsync(entryId);
        if (cartEntry is null)
        {
            return CartEntry.NotFound(entryId);
        }
        
        cartEntry.IsChecked = isChecked;
        await Db.SaveChangesAsync();

        return TypedResults.NoContent();
    }

    [HttpGet]
    [Route("/api/cart/selection")]
    public async Task<Ok<CartSelectResponse>> GetSelection()
    {
        ReadOnlyList<Recipe> allRecipes = await Db.Recipes.ToReadOnlyListAsync();
        ReadOnlyList<CartSelectRecipeResponse> cartRecipes = allRecipes
            .Where(recipe => recipe.CartQuantity > 0)
            .AsQueryable()
            .Select(CartSelectRecipeResponse.FromEntity)
            .OrderBy(recipe => recipe.Name)
            .ThenBy(recipe => recipe.Id)
            .ToReadOnlyList();

        ReadOnlyList<RecipeMinimalResponse> remainingRecipes = allRecipes.Where(recipe => recipe.CartQuantity <= 0)
            .AsQueryable()
            .Select(RecipeMinimalResponse.FromEntity)
            .ToReadOnlyList();

        ReadOnlyList<CartSelectItemResponse> cartItems = Db.CartSelectItems
            .Include(cartSelectItem => cartSelectItem.Item)
            .Select(CartSelectItemResponse.FromEntity)
            .AsEnumerable()
            .OrderBy(item => item.Item.Name)
            .ThenBy(item => item.Item.Id)
            .ThenBy(item => item.Prep?.Name ?? "$None")
            .ThenBy(item => item.Prep?.Id ?? Ulid.Empty)
            .ToReadOnlyList();

        // Determine remaining valid item-prep combinations
        Dictionary<Ulid, ReadOnlyList<PrepResponse?>> cartItemsWithPrep = cartItems
            .GroupBy(item => item.Item.Id)
            .ToDictionary(g => g.Key, g =>
                g.Select(CartSelectItemResponse.ToPrep)
                    .OrderBy(prep => prep?.Name ?? "$None")
                    .ThenBy(prep => prep?.Id ?? Ulid.Empty)
                    .ToReadOnlyList());

        List<ItemWithPrepsResponse> allItemsWithPrep = Db.Items
            .Include(item => item.Preps)
            .Select(ItemWithPrepsResponse.FromEntity)
            .ToList();

        ReadOnlyList<ItemWithPrepsResponse> remainingItems = allItemsWithPrep
            .Where(item => !item.Preps.Equals(cartItemsWithPrep.GetValueOrDefault(item.Item.Id)))
            .Select(item => item with { Preps = item.Preps.Except(cartItemsWithPrep.GetValueOrDefault(item.Item.Id) ?? []).ToReadOnlyList() })
            .ToReadOnlyList();

        UserInfo? info = await GetUserInfo();
        
        CartSelectResponse result = new()
        {
            CartLastGeneratedTime = info?.CartLastGeneratedTime ?? DateTime.UnixEpoch,
            CartSelectionLastUpdatedTime = info?.CartSelectionLastUpdatedTime ?? DateTime.UnixEpoch,
            Items = cartItems,
            Recipes = cartRecipes,
            RemainingItems = remainingItems,
            RemainingRecipes = remainingRecipes
        };

        return TypedResults.Ok(result);
    }
    
    [HttpPut]
    [Route("/api/cart/selection/items/{itemId}/edit")]
    public async Task<Results<NoContent, BadRequest<ErrorResponse>, NotFound<ErrorResponse>>> EditItem(Ulid itemId, [FromQuery] Ulid? prepId, CartSelectItemEditRequest payload)
    {
        if (payload.Amount.UnitType == UnitType.None || payload.Amount.Fraction.AsDouble <= 0.05)
        {
            return ErrorResponse.BadRequestCartAmountInvalid();
        }
        
        Item? item = prepId is not null 
            ? await Db.Items.Include(item => item.Preps)
                .FirstOrDefaultAsync(item => item.ItemId == itemId) 
            : await Db.Items.FindAsync(itemId);
        if (item is null)
        {
            return Item.NotFound(itemId);
        }

        if (prepId is not null)
        {
            Prep? prep = await Db.Preps.FindAsync(prepId);
            if (prep is null)
            {
                return Prep.NotFound(prepId.Value);
            }
        }

        CartSelectItem? cartItem = await Db.CartSelectItems
            .FirstOrDefaultAsync(cartSelectItem => cartSelectItem.ItemId == itemId && cartSelectItem.PrepId == prepId);
        if (cartItem is not null)
        {
            cartItem.Amounts = payload.Amount;
        }
        else
        {
            Db.CartSelectItems.Add(new CartSelectItem
            {
                ItemId = itemId,
                PrepId = prepId,
                Amounts = payload.Amount
            });
        }
        
        UserInfo? info = await GetUserInfo();
        info?.CartSelectionLastUpdatedTime = DateTime.UtcNow;
        
        await Db.SaveChangesAsync();
        
        return TypedResults.NoContent();
    }
    
    [HttpPut]
    [Route("/api/cart/selection/recipes/{recipeId}/edit")]
    public async Task<Results<NoContent, BadRequest<ErrorResponse>, NotFound<ErrorResponse>>> EditRecipe([Required] Ulid recipeId, CartSelectRecipeEditRequest payload)
    {
        if (payload.Quantity <= 0)
        {
            return ErrorResponse.BadRequestCartAmountInvalid();
        }
        
        Recipe? recipe = await Db.Recipes.FindAsync(recipeId);
        if (recipe is null)
        {
            return Recipe.NotFound(recipeId);
        }
        
        UserInfo? info = await GetUserInfo();
        info?.CartSelectionLastUpdatedTime = DateTime.UtcNow;
        
        recipe.CartQuantity = payload.Quantity;
        await Db.SaveChangesAsync();
        
        return TypedResults.NoContent();
    }
    
    [HttpDelete]
    [Route("/api/cart/selection/items/{itemId}/delete")]
    public async Task<Results<NoContent, BadRequest<ErrorResponse>, NotFound<ErrorResponse>>> RemoveItem(Ulid itemId, [FromQuery] Ulid? prepId)
    {
        Item? item = await Db.Items.FindAsync(itemId);
        if (item is null)
        {
            return Item.NotFound(itemId);
        }

        if (prepId is not null)
        {
            Prep? prep = await Db.Preps.FindAsync(prepId);
            if (prep is null)
            {
                return Prep.NotFound(prepId.Value);
            }
        }

        CartSelectItem? cartItem = await Db.CartSelectItems.FirstOrDefaultAsync(ci => ci.ItemId == itemId && ci.PrepId == prepId);
        if (cartItem is null)
        {
            return CartSelectItem.NotFound(itemId, prepId);
        }
        
        UserInfo? info = await GetUserInfo();
        info?.CartSelectionLastUpdatedTime = DateTime.UtcNow;
        
        Db.CartSelectItems.Remove(cartItem);
        await Db.SaveChangesAsync();
        
        return TypedResults.NoContent();
    }
    
    [HttpDelete]
    [Route("/api/cart/selection/recipes/{recipeId}/delete")]
    public async Task<Results<NoContent, BadRequest<ErrorResponse>, NotFound<ErrorResponse>>> RemoveRecipe([Required] Ulid recipeId)
    {
        Recipe? recipe = await Db.Recipes.FindAsync(recipeId);
        if (recipe is null)
        {
            return Recipe.NotFound(recipeId);
        }
        
        UserInfo? info = await GetUserInfo();
        info?.CartSelectionLastUpdatedTime = DateTime.UtcNow;
        
        recipe.CartQuantity = 0;
        await Db.SaveChangesAsync();
        
        return TypedResults.NoContent();
    }
}