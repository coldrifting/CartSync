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

[Tags("Locations")]
public class CartController(CartSyncContext context) : ControllerCore(context)
{
    [HttpGet]
    [Route("/api/cart/generate")]
    public async Task<NoContent> Generate()
    {
        List<CartInfo> cartInfos = Db.Recipes
            .Where(r => r.CartQuantity > 0)
            .SelectMany(r => r.Sections)
            .SelectMany(r => r.Entries)
            .Select(e => new CartInfo
            {
                ItemId = e.ItemId,
                PrepId = e.PrepId,
                Amount = e.Amount
            })
            .Concat(Db.CartItems.Select(ci => new CartInfo
            {
                ItemId = ci.ItemId,
                PrepId = ci.PrepId,
                Amount = ci.Amount
            }))
            .GroupBy(e => new {e.ItemId, e.PrepId})
            .Select(g => new CartInfo
            {
                ItemId = g.Key.ItemId,
                PrepId = g.Key.PrepId,
                Amount = g.Aggregate(Amount.None, (a, b) => a + b.Amount)
            })
            .ToList();

        Store store = await GetSelectedStore();
        List<ItemAisle> lookup = Db.ItemAisles
            .Include(ia => ia.Aisle)
            .Where(ia => ia.StoreId == store.StoreId)
            .ToList();

        // Reset cart
        await Db.CartEntries.ExecuteDeleteAsync();
        foreach (CartInfo cartInfo in cartInfos)
        {
            ItemAisle? aisleInfo = lookup.FirstOrDefault(i => i.ItemId == cartInfo.ItemId);
            Db.CartEntries.Add(new CartEntry
            {
                ItemId = cartInfo.ItemId,
                PrepId = cartInfo.PrepId,
                AisleId = aisleInfo?.AisleId,
                Bay = aisleInfo?.Bay ?? BayType.Middle,
                Amount = cartInfo.Amount
            });
        }

        return TypedResults.NoContent();
    }

    [HttpGet]
    [Route("/api/cart")]
    public async Task<Ok<CartResponse>> Get()
    {
        Store store = await GetSelectedStore();

        CartAisleResponse[] aisles = Db.CartEntries
            .Include(ce => ce.Aisle)
            .GroupBy(ce => ce.AisleId)
            .Select(g => new CartAisleResponse
            {
                AisleId = g.Key,
                AisleName = g.FirstOrDefault() != null ? g.FirstOrDefault()!.Aisle!.AisleName : null,
                Items = g.Select(ce => new CartItemResponse
                {
                    Item = new ItemMinimalResponse
                    {
                        ItemId = ce.ItemId,
                        ItemName = ce.Item.ItemName,
                        ItemTemp = ce.Item.ItemTemp
                    },
                    Prep = ce.Prep,
                    Bay = ce.Bay,
                    Amount = ce.Amount
                }).ToArray()
            })
            .ToArray();

        return TypedResults.Ok(new CartResponse()
        {
            StoreId = store.StoreId,
            StoreName = store.StoreName,
            Aisles = aisles
        });
    }

    [HttpGet]
    [Route("/api/cart/check")]
    public async Task<Results<NoContent, BadRequest<Error>, NotFound<Error>>> Check([FromQuery] Ulid itemId, [FromQuery] Ulid? prepId)
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
    public Task<Ok<CartSelectResponse>> GetSelection()
    {
        try
        {
            CartSelectResponse result = new()
            {
                Items = Db.CartItems
                    .Include(ci => ci.Item)
                    .Select(CartItem.ToResponse)
                    .ToArray(),
                Recipes = Db.Recipes.Select(r => new CartSelectRecipeResponse
                {
                    RecipeId = r.RecipeId,
                    RecipeName = r.RecipeName,
                    Quantity = r.CartQuantity
                }).ToArray()
            };
            
            return Task.FromResult(TypedResults.Ok(result));
        }
        catch (Exception exception)
        {
            return Task.FromException<Ok<CartSelectResponse>>(exception);
        }
    }
    
    [HttpPut]
    [Route("/api/cart/selection/items/{itemId}/edit")]
    public async Task<Results<NoContent, BadRequest<Error>, NotFound<Error>>> EditItem([Required] Ulid itemId, [FromQuery] Ulid? prepId, [FromBody] CartItemEditRequest payload)
    {
        if (payload.Amount.Fraction.AsDouble <= 0.05)
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
            cartItem.Amount = payload.Amount;
        }
        else
        {
            Db.CartItems.Add(new CartItem
            {
                ItemId = itemId,
                PrepId = prepId,
                Amount = payload.Amount
            });
        }
        
        await Db.SaveChangesAsync();
        
        return TypedResults.NoContent();
    }
    
    [HttpPut]
    [Route("/api/cart/selection/recipes/{recipeId}/edit")]
    public async Task<Results<NoContent, BadRequest<Error>, NotFound<Error>>> EditRecipe([Required] Ulid recipeId, [FromBody] CartRecipeEditRequest payload)
    {
        if (payload.Quantity < 0)
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
    public async Task<Results<NoContent, BadRequest<Error>, NotFound<Error>>> RemoveItem([Required] Ulid itemId, [FromQuery] Ulid? prepId)
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