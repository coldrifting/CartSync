using CartSync.Models;
using CartSync.Objects;
using CartSync.Objects.Enums;
using CartSyncTests.Base;
using Microsoft.EntityFrameworkCore;
using SeedData = CartSync.Models.Seeding.SeedData;

namespace CartSyncTests.ControllerTests;

[Collection("DatabaseTests")]
public class CartControllerTests(DatabaseSetup fixture) : DatabaseFixture(fixture)
{
    [Fact]
    public async Task TestEditItem_NoPrep_NotInCart()
    {
        Ulid itemId = SeedData.Items[37].ItemId;
        Amount cartAmount = new(5, 2, UnitType.VolumeCups);
        await CartController.EditItem(itemId, null, new CartItemEditRequest { Amount = cartAmount }).AssertIsSuccessful();

        CartItem? cartItem = await Context.CartItems.FirstOrDefaultAsync(ci => ci.ItemId == itemId && ci.PrepId == null);
        Assert.NotNull(cartItem);
        Assert.Equal(cartAmount, cartItem.Amount);
    }
    
    [Fact]
    public async Task TestEditItem_WithPrep_NotInCart()
    {
        Ulid itemId = SeedData.Items[207].ItemId;
        Ulid prepId = SeedData.Preps[2].PrepId;
        Amount cartAmount = new(1, 2, UnitType.Count);
        await CartController.EditItem(itemId, prepId, new CartItemEditRequest { Amount = cartAmount }).AssertIsSuccessful();

        CartItem? cartItem = await Context.CartItems.FirstOrDefaultAsync(ci => ci.ItemId == itemId && ci.PrepId == prepId);
        Assert.NotNull(cartItem);
        Assert.Equal(cartAmount, cartItem.Amount);
    }
    
    [Fact]
    public async Task TestEditItem_WithPrepAndNoPrep_NotInCart()
    {
        Ulid itemId = SeedData.Items[207].ItemId;
        Ulid prepId = SeedData.Preps[2].PrepId;
        Amount cartAmount1 = new(1, 2, UnitType.Count);
        Amount cartAmount2 = new(4, 1, UnitType.VolumeTablespoons);
        await CartController.EditItem(itemId, prepId, new CartItemEditRequest { Amount = cartAmount1 }).AssertIsSuccessful();
        await CartController.EditItem(itemId, null, new CartItemEditRequest { Amount = cartAmount2 }).AssertIsSuccessful();

        CartItem? cartItem1 = await Context.CartItems.FirstOrDefaultAsync(ci => ci.ItemId == itemId && ci.PrepId == prepId);
        Assert.NotNull(cartItem1);
        Assert.Equal(cartAmount1, cartItem1.Amount);
        
        CartItem? cartItem2 = await Context.CartItems.FirstOrDefaultAsync(ci => ci.ItemId == itemId && ci.PrepId == null);
        Assert.NotNull(cartItem2);
        Assert.Equal(cartAmount2, cartItem2.Amount);
    }
    
    [Fact]
    public async Task TestEditItem_NoPrep_AlreadyInCart()
    {
        Ulid itemId = SeedData.Items[114].ItemId;
        Amount cartAmount = new(5, 2, UnitType.VolumeCups);
        await CartController.EditItem(itemId, null, new CartItemEditRequest { Amount = cartAmount }).AssertIsSuccessful();

        CartItem? cartItem = await Context.CartItems.FirstOrDefaultAsync(ci => ci.ItemId == itemId && ci.PrepId == null);
        Assert.NotNull(cartItem);
        Assert.NotEqual(SeedData.CartItems[0].Amount, cartItem.Amount);
        Assert.Equal(itemId, cartItem.ItemId);
        Assert.Equal(cartAmount, cartItem.Amount);
    }
}