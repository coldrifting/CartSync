using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using CartSync.Controllers.Core;
using CartSync.Models;
using CartSync.Models.Joins;
using CartSync.Objects;
using CartSync.Objects.Enums;
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
        ImmutableList<CartInfo> cartItemsExtra = Db.CartItems
            .Include(ci => ci.Item)
            .Select(CartInfo.FromCartItem)
            .ToImmutableList();

        ImmutableList<CartInfo> cartItemsFromRecipes = Db.Recipes
            .Include(recipe => recipe.Sections)
            .ThenInclude(recipeSection => recipeSection.Entries)
            .ThenInclude(rse => rse.Item)
            .Where(r => r.CartQuantity > 0)
            .SelectMany(r => r.Sections)
            .SelectMany(s => s.Entries)
            .Select(e => new CartInfo
            {
                ItemId = e.ItemId,
                PrepId = e.PrepId,
                Amounts = e.Amount.Multiply(e.RecipeSection.Recipe.CartQuantity, e.Item.UncapCartUnits),
                UncapUnits = e.Item.UncapCartUnits
            })
            .ToImmutableList();
        
        ImmutableList<CartInfo> finalCartInfo = cartItemsFromRecipes
            .Concat(cartItemsExtra)
            .GroupBy(ci => new { ci.ItemId, ci.PrepId })
            .Select(g => new CartInfo
            {
                ItemId = g.Key.ItemId,
                PrepId = g.Key.PrepId,
                Amounts = g.Aggregate(AmountGroup.None, (totalAmount, cartInfo) => cartInfo.Amounts.Add(totalAmount, g.First().UncapUnits)),
                UncapUnits = g.First().UncapUnits
            })
            .ToImmutableList();

        Store store = await GetSelectedStore();
        List<ItemAisle> lookup = Db.ItemAisles
            .Include(ia => ia.Aisle)
            .Where(ia => ia.StoreId == store.StoreId)
            .ToList();

        // Reset cart
        await Db.CartEntries.ExecuteDeleteAsync();
        foreach (CartInfo cartInfo in finalCartInfo)
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
        await Db.SaveChangesAsync();

        return TypedResults.NoContent();
    }

    [HttpGet]
    [Route("/api/cart")]
    public async Task<Ok<CartResponse>> Get()
    {
        Store store = await GetSelectedStore();

        CartAisleResponse[] aisles = Db.CartEntries
            .Include(ce => ce.Aisle)
            .OrderBy(ce => ce.Aisle != null ? ce.Aisle.SortOrder : -1)
            .GroupBy(ce => ce.AisleId)
            .Select(g => new CartAisleResponse
            {
                AisleId = g.Key,
                AisleName = g.FirstOrDefault() != null ? g.FirstOrDefault()!.Aisle!.AisleName : null,
                Items = g.Select(ce => new CartItemResponse
                    {
                        Item = new ItemMinimalResponse
                        {
                            Id = ce.ItemId,
                            Name = ce.Item.ItemName,
                            Temp = ce.Item.Temp
                        },
                        Prep = ce.Prep,
                        Bay = ce.Bay,
                        Amounts = ce.Amounts
                    })
                    .OrderBy(cir => cir.Bay)
                    .ThenBy(cir => cir.Item.Name)
                    .ThenBy(cir => cir.Item.Id)
                    .ThenBy(cir => cir.Prep != null ? cir.Prep.PrepName : "")
                    .ThenBy(cir => cir.Prep != null ? cir.Prep.PrepId : Ulid.Empty)
                    .ToArray()
            })
            .ToArray();

        return TypedResults.Ok(new CartResponse
        {
            StoreId = store.StoreId,
            StoreName = store.StoreName,
            Aisles = aisles
        });
    }

    [HttpPost]
    [Route("/api/cart/items/{itemId}/edit")]
    public async Task<Results<NoContent, BadRequest<Error>, NotFound<Error>>> Check(Ulid itemId, [FromQuery] Ulid? prepId)
    {
        CartEntry? cartEntry = await Db.CartEntries.FirstOrDefaultAsync(ce => ce.ItemId == itemId && ce.PrepId == prepId);
        if (cartEntry is null)
        {
            return CartEntry.NotFound(itemId, prepId);
        }
        
        cartEntry.IsChecked = !cartEntry.IsChecked;

        return TypedResults.NoContent();
    }

    [HttpGet]
    [Route("/api/cart/selection")]
    public async Task<Ok<CartSelectResponse>> GetSelection()
    {
        CartSelectResponse result = new()
        {
            Items = await Db.CartItems
                .Include(ci => ci.Item)
                .Select(CartItem.ToResponse)
                .ToArrayAsync(),
            Recipes = await Db.Recipes
                .Where(r => r.CartQuantity > 0)
                .Select(r => new CartSelectRecipeResponse
            {
                RecipeId = r.RecipeId,
                RecipeName = r.RecipeName,
                Quantity = r.CartQuantity
            }).ToArrayAsync()
        };
        
        return TypedResults.Ok(result);
    }
    
    [HttpPut]
    [Route("/api/cart/selection/items/{itemId}/edit")]
    public async Task<Results<NoContent, BadRequest<Error>, NotFound<Error>>> EditItem(Ulid itemId, [FromQuery] Ulid? prepId, [FromBody] CartItemEditRequest payload)
    {
        if (payload.Amount.UnitType == UnitType.None || payload.Amount.Fraction.AsDouble <= 0.05)
        {
            return Error.BadRequestCartAmountInvalid();
        }
        
        Item? item = prepId is not null 
            ? await Db.Items.Include(i => i.Preps).FirstOrDefaultAsync(i => i.ItemId == itemId) 
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

        CartItem? cartItem = await Db.CartItems.FirstOrDefaultAsync(ci => ci.ItemId == itemId && ci.PrepId == prepId);
        if (cartItem is not null)
        {
            cartItem.Amounts = payload.Amount;
        }
        else
        {
            Db.CartItems.Add(new CartItem
            {
                ItemId = itemId,
                PrepId = prepId,
                Amounts = payload.Amount
            });
        }
        
        await Db.SaveChangesAsync();
        
        return TypedResults.NoContent();
    }
    
    [HttpPut]
    [Route("/api/cart/selection/recipes/{recipeId}/edit")]
    public async Task<Results<NoContent, BadRequest<Error>, NotFound<Error>>> EditRecipe([Required] Ulid recipeId, [FromBody] CartRecipeEditRequest payload)
    {
        if (payload.Quantity <= 0)
        {
            return Error.BadRequestCartAmountInvalid();
        }
        
        Recipe? recipe = await Db.Recipes.FindAsync(recipeId);
        if (recipe is null)
        {
            return Recipe.NotFound(recipeId);
        }
        
        recipe.CartQuantity = payload.Quantity;
        await Db.SaveChangesAsync();
        
        return TypedResults.NoContent();
    }
    
    [HttpDelete]
    [Route("/api/cart/selection/items/{itemId}/delete")]
    public async Task<Results<NoContent, BadRequest<Error>, NotFound<Error>>> RemoveItem(Ulid itemId, [FromQuery] Ulid? prepId)
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

        CartItem? cartItem = await Db.CartItems.FirstOrDefaultAsync(ci => ci.ItemId == itemId && ci.PrepId == prepId);
        if (cartItem is null)
        {
            return CartItem.NotFound(itemId, prepId);
        }
        
        Db.CartItems.Remove(cartItem);
        await Db.SaveChangesAsync();
        
        return TypedResults.NoContent();
    }
    
    [HttpDelete]
    [Route("/api/cart/selection/recipes/{recipeId}/delete")]
    public async Task<Results<NoContent, BadRequest<Error>, NotFound<Error>>> RemoveRecipe([Required] Ulid recipeId)
    {
        Recipe? recipe = await Db.Recipes.FindAsync(recipeId);
        if (recipe is null)
        {
            return Recipe.NotFound(recipeId);
        }
        
        recipe.CartQuantity = 0;
        await Db.SaveChangesAsync();
        return TypedResults.NoContent();
    }
}