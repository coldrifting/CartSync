using System.Collections.Immutable;
using System.Net;
using CartSync.Data.Entities;
using CartSync.Data.Misc;
using CartSync.Data.Requests;
using CartSync.Data.Responses;
using CartSync.Objects;
using CartSyncTests.Base;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson.Operations;
using Microsoft.EntityFrameworkCore;
using SeedData = CartSync.SeedData.SeedData;

namespace CartSyncTests.ControllerTests;

[Collection("DatabaseTests")]
public class CartControllerTests(DatabaseSetup fixture) : DatabaseFixture(fixture)
{
    [Fact]
    public async Task TestCartEditItem_NoPrep_NotInCart()
    {
        Ulid itemId = SeedData.Items[37].ItemId;
        Amount cartAmount = Amount.VolumeCups(5, 2);
        await CartController.EditItem(itemId, null, new CartSelectItemEditRequest { Amount = cartAmount }).AssertIsSuccessful();

        CartSelectItem? cartItem = await Context.CartSelectItems.FirstOrDefaultAsync(ci => ci.ItemId == itemId && ci.PrepId == null, TestContext.Current.CancellationToken);
        Assert.NotNull(cartItem);
        Assert.Equal(cartAmount, cartItem.Amounts);
    }
    
    [Fact]
    public async Task TestCartEditItem_WithPrep_NotInCart()
    {
        Ulid itemId = SeedData.Items[207].ItemId;
        Ulid prepId = SeedData.Preps[2].PrepId;
        Amount cartAmount = Amount.Count(1, 2);
        await CartController.EditItem(itemId, prepId, new CartSelectItemEditRequest { Amount = cartAmount }).AssertIsSuccessful();

        CartSelectItem? cartItem = await Context.CartSelectItems.FirstOrDefaultAsync(ci => ci.ItemId == itemId && ci.PrepId == prepId, TestContext.Current.CancellationToken);
        Assert.NotNull(cartItem);
        Assert.Equal(cartAmount, cartItem.Amounts);
    }
    
    [Fact]
    public async Task TestCartEditItem_BadAmount_ShouldError()
    {
        Ulid itemId = SeedData.Items[207].ItemId;
        Ulid prepId = SeedData.Preps[2].PrepId;
        Amount cartAmount1 = Amount.Count(0);
        Amount cartAmount2 = Amount.None;
        ErrorResponse error1 = await CartController.EditItem(itemId, prepId, new CartSelectItemEditRequest { Amount = cartAmount1 }).ErrorAsync();
        error1.AssertStatus(HttpStatusCode.BadRequest);
        ErrorResponse error2 = await CartController.EditItem(itemId, prepId, new CartSelectItemEditRequest { Amount = cartAmount2 }).ErrorAsync();
        error2.AssertStatus(HttpStatusCode.BadRequest);
        ErrorResponse error3 = await CartController.EditItem(itemId, null, new CartSelectItemEditRequest { Amount = cartAmount1 }).ErrorAsync();
        error3.AssertStatus(HttpStatusCode.BadRequest);
        ErrorResponse error4 = await CartController.EditItem(itemId, null, new CartSelectItemEditRequest { Amount = cartAmount2 }).ErrorAsync();
        error4.AssertStatus(HttpStatusCode.BadRequest);
        
        CartSelectItem? cartItem = await Context.CartSelectItems.FirstOrDefaultAsync(ci => ci.ItemId == itemId && ci.PrepId == prepId, TestContext.Current.CancellationToken);
        Assert.Null(cartItem);
        CartSelectItem? cartItem2 = await Context.CartSelectItems.FirstOrDefaultAsync(ci => ci.ItemId == itemId && ci.PrepId == null, TestContext.Current.CancellationToken);
        Assert.Null(cartItem2);
    }
    
    [Fact]
    public async Task TestCartEditItem_WithPrepAndNoPrep_NotInCart()
    {
        Ulid itemId = SeedData.Items[207].ItemId;
        Ulid prepId = SeedData.Preps[2].PrepId;
        Amount cartAmount1 = Amount.Count(1, 2);
        Amount cartAmount2 = Amount.VolumeTablespoons(4);
        await CartController.EditItem(itemId, prepId, new CartSelectItemEditRequest { Amount = cartAmount1 }).AssertIsSuccessful();
        await CartController.EditItem(itemId, null, new CartSelectItemEditRequest { Amount = cartAmount2 }).AssertIsSuccessful();

        CartSelectItem? cartItem1 = await Context.CartSelectItems.FirstOrDefaultAsync(ci => ci.ItemId == itemId && ci.PrepId == prepId, TestContext.Current.CancellationToken);
        Assert.NotNull(cartItem1);
        Assert.Equal(cartAmount1, cartItem1.Amounts);
        
        CartSelectItem? cartItem2 = await Context.CartSelectItems.FirstOrDefaultAsync(ci => ci.ItemId == itemId && ci.PrepId == null, TestContext.Current.CancellationToken);
        Assert.NotNull(cartItem2);
        Assert.Equal(cartAmount2, cartItem2.Amounts);
    }
    
    [Fact]
    public async Task TestCartEditItem_NoPrep_AlreadyInCart()
    {
        Ulid itemId = SeedData.Items[114].ItemId;
        Amount cartAmount = Amount.VolumeCups(5, 2);
        await CartController.EditItem(itemId, null, new CartSelectItemEditRequest { Amount = cartAmount }).AssertIsSuccessful();

        CartSelectItem? cartItem = await Context.CartSelectItems.FirstOrDefaultAsync(ci => ci.ItemId == itemId && ci.PrepId == null, TestContext.Current.CancellationToken);
        Assert.NotNull(cartItem);
        Assert.NotEqual(SeedData.CartItems[0].Amounts, cartItem.Amounts);
        Assert.Equal(itemId, cartItem.ItemId);
        Assert.Equal(cartAmount, cartItem.Amounts);
    }
    
    [Fact]
    public async Task TestCartEditItem_ItemNotFound_ShouldError()
    {
        Ulid itemId = Ulid.NotFound;
        Amount cartAmount = Amount.VolumeCups(5, 2);
        ErrorResponse errorResponse = await CartController.EditItem(itemId, null, new CartSelectItemEditRequest { Amount = cartAmount }).ErrorAsync();
        errorResponse.AssertStatus(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task TestCartEditItem_PrepNotFound_ShouldError()
    {
        Ulid itemId = SeedData.Items[181].ItemId;
        Ulid prepId = SeedData.Preps[4].PrepId;
        Amount cartAmount = Amount.VolumeCups(5, 2);
        ErrorResponse errorResponse = await CartController.EditItem(itemId, Ulid.NotFound, new CartSelectItemEditRequest { Amount = cartAmount }).ErrorAsync();
        errorResponse.AssertStatus(HttpStatusCode.NotFound);
        
        CartSelectItem? cartItem = await Context.CartSelectItems.FirstOrDefaultAsync(ci => ci.ItemId == itemId && ci.PrepId == prepId, TestContext.Current.CancellationToken);
        Assert.NotNull(cartItem);
        Assert.NotEqual(cartAmount, cartItem.Amounts);
        Assert.Equal(SeedData.CartItems[1].Amounts, cartItem.Amounts);
    }

    [Fact]
    public async Task TestCartEditRecipe_NotInCart()
    {
        Ulid recipeId = SeedData.Recipes[2].RecipeId;
        const int quantity = 2;
        await CartController.EditRecipe(recipeId, new CartSelectRecipeEditRequest
        {
            Quantity = quantity
        }).AssertIsSuccessful();

        Recipe? recipe = await Context.Recipes.FindAsync([recipeId], TestContext.Current.CancellationToken);
        Assert.NotNull(recipe);
        Assert.Equal(recipeId, recipe.RecipeId);
        Assert.Equal(quantity, recipe.CartQuantity);
        Assert.NotEqual(0, recipe.CartQuantity);
    }

    [Fact]
    public async Task TestCartEditRecipe_RecipeNotFound_ShouldError()
    {
        Ulid recipeId = Ulid.NotFound;
        const int quantity = 2;
        ErrorResponse errorResponse = await CartController.EditRecipe(recipeId, new CartSelectRecipeEditRequest
        {
            Quantity = quantity
        }).ErrorAsync();
        errorResponse.AssertStatus(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task TestCartEditRecipe_AlreadyInCart()
    {
        Ulid recipeId = SeedData.Recipes[1].RecipeId;
        const int quantity = 4;
        await CartController.EditRecipe(recipeId, new CartSelectRecipeEditRequest
        {
            Quantity = quantity
        }).AssertIsSuccessful();

        Recipe? recipe = await Context.Recipes.FindAsync([recipeId], TestContext.Current.CancellationToken);
        Assert.NotNull(recipe);
        Assert.Equal(recipeId, recipe.RecipeId);
        Assert.Equal(quantity, recipe.CartQuantity);
        Assert.NotEqual(SeedData.Recipes[1].CartQuantity, recipe.CartQuantity);
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task TestCartEditRecipe_BadAmount_ShouldError(int quantity)
    {
        Ulid recipeId = SeedData.Recipes[1].RecipeId;
        ErrorResponse errorResponse = await CartController.EditRecipe(recipeId, new CartSelectRecipeEditRequest
        {
            Quantity = quantity
        }).ErrorAsync();
        errorResponse.AssertStatus(HttpStatusCode.BadRequest);

        Recipe? recipe = await Context.Recipes.FindAsync([recipeId], TestContext.Current.CancellationToken);
        Assert.NotNull(recipe);
        Assert.Equal(recipeId, recipe.RecipeId);
        Assert.NotEqual(quantity, recipe.CartQuantity);
        Assert.Equal(SeedData.Recipes[1].CartQuantity, recipe.CartQuantity);
    }

    [Fact]
    public async Task TestCartRemoveItem_WithPrep()
    {
        Ulid itemId = SeedData.Items[181].ItemId;
        Ulid prepId = SeedData.Preps[4].PrepId;
        await CartController.RemoveItem(itemId, prepId).AssertIsSuccessful();
        
        Assert.Null(Context.CartSelectItems.FirstOrDefault(ci => ci.ItemId == itemId && ci.PrepId == prepId));
    }

    [Fact]
    public async Task TestCartRemoveItem_WithPrep_DoesNotEffectItemWithoutPrep()
    {
        Ulid itemId = SeedData.Items[183].ItemId;
        Ulid prepId = SeedData.Preps[3].PrepId;
        await CartController.RemoveItem(itemId, prepId).AssertIsSuccessful();
        
        Assert.Null(Context.CartSelectItems.FirstOrDefault(ci => ci.ItemId == itemId && ci.PrepId == prepId));
        Assert.NotNull(Context.CartSelectItems.FirstOrDefault(ci => ci.ItemId == itemId && ci.PrepId == null));
    }

    [Fact]
    public async Task TestCartRemoveItem_WithOutPrep_DoesNotEffectItemWithPrep()
    {
        Ulid itemId = SeedData.Items[183].ItemId;
        Ulid prepId = SeedData.Preps[3].PrepId;
        await CartController.RemoveItem(itemId, null).AssertIsSuccessful();
        
        Assert.Null(Context.CartSelectItems.FirstOrDefault(ci => ci.ItemId == itemId && ci.PrepId == null));
        Assert.NotNull(Context.CartSelectItems.FirstOrDefault(ci => ci.ItemId == itemId && ci.PrepId == prepId));
    }

    [Fact]
    public async Task TestCartRemoveItem_NoItemPrepComboInCart_ShouldError()
    {
        Ulid itemId = SeedData.Items[182].ItemId;
        Ulid prepId = SeedData.Preps[4].PrepId;
        ErrorResponse errorResponse = await CartController.RemoveItem(itemId, prepId).ErrorAsync();
        errorResponse.AssertStatus(HttpStatusCode.NotFound);
        
        ErrorResponse error2 = await CartController.RemoveItem(itemId, null).ErrorAsync();
        error2.AssertStatus(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task TestCartRemoveItem_ItemNotFound_ShouldError()
    {
        Ulid itemId = Ulid.NotFound;
        ErrorResponse errorResponse = await CartController.RemoveItem(itemId, null).ErrorAsync();
        errorResponse.AssertStatus(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task TestCartRemoveItem_PrepNotFound_ShouldError()
    {
        Ulid itemId = SeedData.Items[183].ItemId;
        Ulid prepId = SeedData.Preps[3].PrepId;
        ErrorResponse errorResponse = await CartController.RemoveItem(itemId, Ulid.NotFound).ErrorAsync();
        errorResponse.AssertStatus(HttpStatusCode.NotFound);
        
        Assert.NotNull(Context.CartSelectItems.FirstOrDefault(ci => ci.ItemId == itemId && ci.PrepId == null));
        Assert.NotNull(Context.CartSelectItems.FirstOrDefault(ci => ci.ItemId == itemId && ci.PrepId == prepId));
    }

    [Fact]
    public async Task TestCartRemoveRecipe()
    {
        Ulid recipeId = SeedData.Recipes[2].RecipeId;
        await CartController.RemoveRecipe(recipeId).AssertIsSuccessful();
        
        Assert.Equal(0, Context.Recipes.FirstOrDefault(r => r.RecipeId == recipeId)?.CartQuantity);
    }

    [Fact]
    public async Task TestCartRemoveRecipe_NotFound_ShouldError()
    {
        Ulid recipeId = Ulid.NotFound;
        ErrorResponse errorResponse = await CartController.RemoveRecipe(recipeId).ErrorAsync();
        errorResponse.AssertStatus(HttpStatusCode.NotFound);
        
        Assert.Null(Context.Recipes.FirstOrDefault(r => r.RecipeId == recipeId));
    }

    [Fact]
    public async Task TestCartGetSelection()
    {
        CartSelectItemResponse expectedItem1 = IndicesToItemAndPrep(114, null, 0);
        CartSelectItemResponse expectedItem2 = IndicesToItemAndPrep(181, 4, 1);
        CartSelectItemResponse expectedItem3 = IndicesToItemAndPrep(183, null, 2);
        CartSelectItemResponse expectedItem4 = IndicesToItemAndPrep(183, 3, 3);

        CartSelectRecipeResponse expectedRecipe1 = IndicesToRecipe(1);
        CartSelectRecipeResponse expectedRecipe2 = IndicesToRecipe(3);
        
        CartSelectResponse actual = await CartController.GetSelection().ValueAsync();
        Assert.Equal(4, actual.Items.Count);
        Assert.Equal(2, actual.Recipes.Count);

        Assert.Contains(expectedItem1, actual.Items);
        Assert.Contains(expectedItem2, actual.Items);
        Assert.Contains(expectedItem3, actual.Items);
        Assert.Contains(expectedItem4, actual.Items);
        
        Assert.Contains(expectedRecipe1, actual.Recipes);
        Assert.Contains(expectedRecipe2, actual.Recipes);
    }

    [Fact]
    public async Task TestCartGetSelection_AfterItemRemoval()
    {
        Ulid itemId = SeedData.Items[183].ItemId;
        Ulid? prepId = null;
        await CartController.RemoveItem(itemId, prepId).AssertIsSuccessful();
        
        CartSelectResponse selection = await CartController.GetSelection().ValueAsync();
        
        Assert.Contains(new CartSelectItemResponse
        {
            Item = new ItemMinimalResponse
            {
                Id = SeedData.Items[183].ItemId,
                Name = SeedData.Items[183].ItemName,
                Temp = SeedData.Items[183].Temp,
                DefaultUnitType = SeedData.Items[183].DefaultUnitType,
            },
            Prep = new PrepResponse
            {
                Id = SeedData.Preps[3].PrepId,
                Name = SeedData.Preps[3].PrepName,
            },
            Amount = SeedData.CartItems[3].Amounts.Amount,
        }, selection.Items);
        Assert.DoesNotContain(new Tuple<Ulid, Ulid?>(SeedData.Items[183].ItemId, null), 
            selection.Items.Select(i => new Tuple<Ulid, Ulid?>(i.Item.Id, i.Prep?.Id)));
    }

    [Fact]
    public async Task TestCartGetSelection_AfterItemAddition()
    {
        Ulid itemId = SeedData.Items[181].ItemId;
        Ulid? prepId = null;
        Amount amount = Amount.WeightPounds(1, 2);
        await CartController.EditItem(itemId, prepId, new CartSelectItemEditRequest
        {
            Amount = amount
        }).AssertIsSuccessful();
        
        CartSelectResponse selection = await CartController.GetSelection().ValueAsync();
        
        Assert.Contains(new CartSelectItemResponse
        {
            Item = new ItemMinimalResponse
            {
                Id = SeedData.Items[181].ItemId,
                Name = SeedData.Items[181].ItemName,
                Temp = SeedData.Items[181].Temp,
                DefaultUnitType = SeedData.Items[181].DefaultUnitType
            },
            Prep = new PrepResponse
            {
                Id = SeedData.Preps[4].PrepId,
                Name = SeedData.Preps[4].PrepName,
            },
            Amount = SeedData.CartItems[1].Amounts.Amount
        }, selection.Items);
        
        Assert.Contains(new CartSelectItemResponse
        {
            Item = new ItemMinimalResponse
            {
                Id = SeedData.Items[181].ItemId,
                Name = SeedData.Items[181].ItemName,
                Temp = SeedData.Items[181].Temp,
                DefaultUnitType = SeedData.Items[181].DefaultUnitType
            },
            Prep = null,
            Amount = amount
        }, selection.Items);
    }

    [Fact]
    public async Task TestCartGenerate_UsingStore0()
    {
        await StoreController.Select(SeedData.Stores[0].StoreId);
        await CartController.Generate();

        ImmutableList<CartEntryValue> expectedEntries =
        [
            IndicesToCartEntry(00, Bay.End,    /* Olive_Oil_Extra_Virgin */ 231, /* $None */           null, Amount.VolumeTablespoons(2) ),
            IndicesToCartEntry(02, Bay.Begin,  /* Black_Beans */            030, /* $None */           null, Amount.VolumeOunces(8) ),
            IndicesToCartEntry(02, Bay.Center, /* Macaroni_Pasta */         046, /* $None */           null, Amount.WeightPounds(1) ),
            IndicesToCartEntry(04, Bay.Begin,  /* Diced_Green_Chilies */    063, /* $None */           null, Amount.VolumeOunces(16) ),
            IndicesToCartEntry(04, Bay.Begin,  /* Salsa */                  064, /* $None */           null, Amount.VolumeCups(3, 4) ),
            IndicesToCartEntry(04, Bay.Center, /* Lime_Juice */             061, /* $None */           null, Amount.VolumeTablespoons(2) ),
            IndicesToCartEntry(04, Bay.Center, /* Rice */                   056, /* $None */           null, Amount.VolumeCups(1) ),
            IndicesToCartEntry(05, Bay.Begin,  /* Black_Pepper */           066, /* $None */           null, Amount.VolumeTeaspoons(1,4) ),
            IndicesToCartEntry(05, Bay.Begin,  /* Coriander */              072, /* $None */           null, Amount.VolumeTeaspoons(1,2) ),
            IndicesToCartEntry(05, Bay.Begin,  /* Crushed_Red_Pepper */     073, /* $None */           null, Amount.VolumeTeaspoons(1,4) ),
            IndicesToCartEntry(05, Bay.Begin,  /* Cumin */                  074, /* $None */           null, Amount.VolumeTeaspoons(3,2) ),
            IndicesToCartEntry(05, Bay.Begin,  /* Oregano */                085, /* $None */           null, Amount.VolumeTeaspoons(1,2) ),
            IndicesToCartEntry(05, Bay.Begin,  /* Salt */                   088, /* $None */           null, Amount.VolumeTeaspoons(3,4) ),
            IndicesToCartEntry(05, Bay.Center, /* Brownie_Mix */            114, /* $None */           null, Amount.Count(3)),
            IndicesToCartEntry(05, Bay.Center, /* Flour */                  100, /* $None */           null, Amount.VolumeTablespoons(4) ),
            IndicesToCartEntry(15, Bay.Begin,  /* Milk */                   147, /* $None */           null, Amount.VolumeQuarts(1) ),
            IndicesToCartEntry(15, Bay.Center, /* Butter */                 149, /* $None */           null, Amount.VolumeTablespoons(4) ),
            IndicesToCartEntry(20, Bay.Center, /* Monterey_Jack_Cheese */   180, /* Shredded */        3,    Amount.VolumeCups(3) ),
            IndicesToCartEntry(20, Bay.Center, /* Mozzarella_Cheese */      181, /* Sliced */          4,    Amount.VolumeCups(2) ),
            IndicesToCartEntry(20, Bay.Center, /* Provolone_Cheese */       183, /* $None */           null, Amount.VolumeCups(8) ),
            IndicesToCartEntry(20, Bay.Center, /* Provolone_Cheese */       183, /* Shredded */        3,    Amount.VolumeCups(4) ),
            IndicesToCartEntry(21, Bay.Center, /* Garlic */                 207, /* Minced */          2,    Amount.Count(1) ),
            IndicesToCartEntry(21, Bay.Center, /* Poblano_Peppers */        215, /* Sliced (Halves) */ 5,    Amount.Count(4) ),
            IndicesToCartEntry(21, Bay.Center, /* Red_Bell_Pepper */        216, /* Sliced */          4,    Amount.Count(1, 2) ),
            IndicesToCartEntry(21, Bay.Center, /* Red_Onions */             217, /* Diced */           1,    Amount.Count(1, 3) ),
            IndicesToCartEntry(21, Bay.Center, /* Spinach */                193, /* $None */           null, Amount.VolumeCups(3) ),
        ];

        ImmutableList<CartEntryValue> actualEntries = Context.CartEntries
            .OrderBy(entry => entry.Aisle != null ? entry.Aisle.SortOrder : -1)
            .ThenBy(entry => entry.Bay)
            .ThenBy(entry => entry.Item.ItemName)
            .ThenBy(entry => entry.Item.ItemId)
            .ThenBy(entry => entry.Prep != null ? entry.Prep.PrepName : "$None")
            .ThenBy(entry => entry.PrepId)
            .Select(CartEntryValue.FromCartEntry)
            .ToImmutableList();
        
        Assert.Equal(expectedEntries.Count, actualEntries.Count);
        for (int i = 0; i < expectedEntries.Count; i++)
        {
            Assert.Equal(expectedEntries[i].AisleId, actualEntries[i].AisleId);
            Assert.Equal(expectedEntries[i].Bay, actualEntries[i].Bay);
            Assert.Equal(expectedEntries[i].ItemId, actualEntries[i].ItemId);
            Assert.Equal(expectedEntries[i].PrepId, actualEntries[i].PrepId);
            Assert.Equal(expectedEntries[i].Amounts, actualEntries[i].Amounts);
        }

        CartEntryResponse result = await CartController.Get().ValueAsync();
        Assert.Equal(SeedData.Stores[0].StoreId, result.StoreId);
        Assert.Equal(SeedData.Stores[0].StoreName, result.StoreName);
        Assert.Equal(7, result.Aisles.Count);

        Assert.Single(result.Aisles[0].Items);
        Assert.Equal(2, result.Aisles[1].Items.Count);
        Assert.Equal(2, result.Aisles[1].Items.Count);
        Assert.Equal(4, result.Aisles[2].Items.Count);
        Assert.Equal(8, result.Aisles[3].Items.Count);
        Assert.Equal(2, result.Aisles[4].Items.Count);
        Assert.Equal(4, result.Aisles[5].Items.Count);
        Assert.Equal(5, result.Aisles[6].Items.Count);
    }

    [Fact]
    public async Task TestCartGenerate_UsingStore1()
    {
        await StoreController.Select(SeedData.Stores[1].StoreId);

        await SetItemLocation(1, 114, 23, Bay.End);
        await SetItemLocation(1, 207, 23, Bay.End);
        await SetItemLocation(1, 215, 23, Bay.End);
        
        await CartController.Generate();

        ImmutableList<CartEntryValue> expectedEntries =
        [
            IndicesToCartEntry(null, Bay.Center, /* Black_Beans */            030, /* $None */           null, Amount.VolumeOunces(8) ),
            IndicesToCartEntry(null, Bay.Center, /* Black_Pepper */           066, /* $None */           null, Amount.VolumeTeaspoons(1,4) ),
            IndicesToCartEntry(null, Bay.Center, /* Coriander */              072, /* $None */           null, Amount.VolumeTeaspoons(1,2) ),
            IndicesToCartEntry(null, Bay.Center, /* Crushed_Red_Pepper */     073, /* $None */           null, Amount.VolumeTeaspoons(1,4) ),
            IndicesToCartEntry(null, Bay.Center, /* Cumin */                  074, /* $None */           null, Amount.VolumeTeaspoons(3,2) ),
            IndicesToCartEntry(null, Bay.Center, /* Diced_Green_Chilies */    063, /* $None */           null, Amount.VolumeOunces(16) ),
            IndicesToCartEntry(null, Bay.Center, /* Flour */                  100, /* $None */           null, Amount.VolumeTablespoons(4) ),
            IndicesToCartEntry(null, Bay.Center, /* Lime_Juice */             061, /* $None */           null, Amount.VolumeTablespoons(2) ),
            IndicesToCartEntry(null, Bay.Center, /* Macaroni_Pasta */         046, /* $None */           null, Amount.WeightPounds(1) ),
            IndicesToCartEntry(null, Bay.Center, /* Olive_Oil_Extra_Virgin */ 231, /* $None */           null, Amount.VolumeTablespoons(2) ),
            IndicesToCartEntry(null, Bay.Center, /* Oregano */                085, /* $None */           null, Amount.VolumeTeaspoons(1,2) ),
            IndicesToCartEntry(null, Bay.Center, /* Red_Onions */             217, /* Diced */           1,    Amount.Count(1, 3) ),
            IndicesToCartEntry(null, Bay.Center, /* Rice */                   056, /* $None */           null, Amount.VolumeCups(1) ),
            IndicesToCartEntry(null, Bay.Center, /* Salsa */                  064, /* $None */           null, Amount.VolumeCups(3, 4) ),
            IndicesToCartEntry(null, Bay.Center, /* Salt */                   088, /* $None */           null, Amount.VolumeTeaspoons(3,4) ),
            IndicesToCartEntry(null, Bay.Center, /* Butter */                 149, /* $None */           null, Amount.VolumeTablespoons(4) ),
            IndicesToCartEntry(null, Bay.Center, /* Milk */                   147, /* $None */           null, Amount.VolumeQuarts(1) ),
            IndicesToCartEntry(null, Bay.Center, /* Monterey_Jack_Cheese */   180, /* Shredded */        3,    Amount.VolumeCups(3) ),
            IndicesToCartEntry(null, Bay.Center, /* Mozzarella_Cheese */      181, /* Sliced */          4,    Amount.VolumeCups(2) ),
            IndicesToCartEntry(null, Bay.Center, /* Provolone_Cheese */       183, /* $None */           null, Amount.VolumeCups(8) ),
            IndicesToCartEntry(null, Bay.Center, /* Provolone_Cheese */       183, /* Shredded */        3,    Amount.VolumeCups(4) ),
            IndicesToCartEntry(null, Bay.Center, /* Red_Bell_Pepper */        216, /* Sliced */          4,    Amount.Count(1, 2) ),
            IndicesToCartEntry(null, Bay.Center, /* Spinach */                193, /* $None */           null, Amount.VolumeCups(3) ),
            IndicesToCartEntry(23, Bay.End, /* Brownie_Mix */            114, /* $None */           null, Amount.Count(3)),
            IndicesToCartEntry(23, Bay.End, /* Garlic */                 207, /* Minced */          2,    Amount.Count(1) ),
            IndicesToCartEntry(23, Bay.End, /* Poblano_Peppers */        215, /* Sliced (Halves) */ 5,    Amount.Count(4) )
        ];

        ReadOnlyList<CartEntryValue> actualEntries = Context.CartEntries
            .OrderBy(entry => entry.Aisle != null ? entry.Aisle.SortOrder : -1)
            .ThenBy(entry => entry.Bay)
            .ThenBy(entry => entry.Item.Temp)
            .ThenBy(entry => entry.Item.ItemName)
            .ThenBy(entry => entry.Item.ItemId)
            .ThenBy(entry => entry.Prep != null ? entry.Prep.PrepName : "$None")
            .ThenBy(entry => entry.PrepId)
            .Select(CartEntryValue.FromCartEntry)
            .ToReadOnlyList();
        
        Assert.Equal(expectedEntries.Count, actualEntries.Count);
        for (int i = 0; i < expectedEntries.Count; i++)
        {
            Assert.Equal(expectedEntries[i].AisleId, actualEntries[i].AisleId);
            Assert.Equal(expectedEntries[i].Bay, actualEntries[i].Bay);
            Assert.Equal(expectedEntries[i].ItemId, actualEntries[i].ItemId);
            Assert.Equal(expectedEntries[i].PrepId, actualEntries[i].PrepId);
            Assert.Equal(expectedEntries[i].Amounts, actualEntries[i].Amounts);
        }

        CartEntryResponse result = await CartController.Get().ValueAsync();
        Assert.Equal(SeedData.Stores[1].StoreId, result.StoreId);
        Assert.Equal(SeedData.Stores[1].StoreName, result.StoreName);
        Assert.Equal(2, result.Aisles.Count);

        Assert.Equal(23, result.Aisles[0].Items.Count);
        Assert.Equal(3, result.Aisles[1].Items.Count);
    }
    
    [Fact]
    public async Task TestCartGenerate_UsingStore0_WithConflictingUnitTypes()
    {
        await StoreController.Select(SeedData.Stores[0].StoreId);
        
        await foreach (Recipe recipe in Context.Recipes)
        {
            recipe.CartQuantity = 0;
        }

        ReadOnlyList<CartSelectItem> cartItems = await Context.CartSelectItems.ToReadOnlyListAsync(TestContext.Current.CancellationToken);
        foreach (CartSelectItem contextCartItem in cartItems)
        {
            Context.CartSelectItems.Remove(contextCartItem);
        }

        Context.CartSelectItems.Add(new CartSelectItem
        {
            ItemId = SeedData.Items[180].ItemId,
            PrepId = SeedData.Preps[3].PrepId,
            Amounts = Amount.WeightOunces(8),
        });

        await CartController.EditRecipe(SeedData.Recipes[1].RecipeId, new CartSelectRecipeEditRequest
        {
            Quantity = 2
        });
        
        await CartController.Generate();

        AmountGroup amounts = new()
        {
            Count = Amount.None,
            Volume = Amount.VolumeOunces(16),
            Weight = Amount.WeightOunces(8)
        };
        
        ImmutableList<CartEntryValue> expectedEntries =
        [
            IndicesToCartEntry(02, Bay.Center, /* Macaroni_Pasta */         046, /* $None */           null, Amount.WeightPounds(1) ),
            IndicesToCartEntry(04, Bay.Begin,  /* Diced_Green_Chilies */    063, /* $None */           null, Amount.VolumeOunces(16) ),
            IndicesToCartEntry(05, Bay.Begin,  /* Crushed_Red_Pepper */     073, /* $None */           null, Amount.VolumeTeaspoons(1,4) ),
            IndicesToCartEntry(05, Bay.Begin,  /* Cumin */                  074, /* $None */           null, Amount.VolumeTeaspoons(1) ),
            IndicesToCartEntry(05, Bay.Begin,  /* Salt */                   088, /* $None */           null, Amount.VolumeTeaspoons(1,2) ),
            IndicesToCartEntry(05, Bay.Center, /* Flour */                  100, /* $None */           null, Amount.VolumeTablespoons(4) ),
            IndicesToCartEntry(15, Bay.Begin,  /* Milk */                   147, /* $None */           null, Amount.VolumeQuarts(1) ),
            IndicesToCartEntry(15, Bay.Center, /* Butter */                 149, /* $None */           null, Amount.VolumeTablespoons(4) ),
            IndicesToCartEntry(20, Bay.Center, /* Monterey_Jack_Cheese */   180, /* Shredded */        3,    amounts)
        ];
        
        ImmutableList<CartEntryValue> actualEntries = Context.CartEntries
            .OrderBy(entry => entry.Aisle != null ? entry.Aisle.SortOrder : -1)
            .ThenBy(entry => entry.Bay)
            .ThenBy(entry => entry.Item.ItemName)
            .ThenBy(entry => entry.Item.ItemId)
            .ThenBy(entry => entry.Prep != null ? entry.Prep.PrepName : "$None")
            .ThenBy(entry => entry.PrepId)
            .Select(CartEntryValue.FromCartEntry)
            .ToImmutableList();

        Assert.Equal(expectedEntries.Count, actualEntries.Count);
        for (int i = 0; i < expectedEntries.Count; i++)
        {
            Assert.Equal(expectedEntries[i].AisleId, actualEntries[i].AisleId);
            Assert.Equal(expectedEntries[i].Bay, actualEntries[i].Bay);
            Assert.Equal(expectedEntries[i].ItemId, actualEntries[i].ItemId);
            Assert.Equal(expectedEntries[i].PrepId, actualEntries[i].PrepId);
            Assert.Equal(expectedEntries[i].Amounts, actualEntries[i].Amounts);
        }
    }

    private static CartEntryValue IndicesToCartEntry(int? aisleIndex, Bay bay, int itemIndex, int? prepIndex, AmountGroup amounts)
    {
        return new CartEntryValue
        {
            AisleId = aisleIndex is not null ? SeedData.Aisles[aisleIndex.Value].AisleId : null,
            Bay = bay,
            ItemId = SeedData.Items[itemIndex].ItemId,
            PrepId = prepIndex is not null ? SeedData.Preps[prepIndex.Value].PrepId : null,
            Amounts = amounts,
            IsChecked = false
        };
    }

    private static CartSelectItemResponse IndicesToItemAndPrep(int itemIndex, int? prepIndex, int cartItemIndex)
    {
        return new CartSelectItemResponse
        {
            Item = new ItemMinimalResponse
            {
                Id = SeedData.Items[itemIndex].ItemId,
                Name = SeedData.Items[itemIndex].ItemName,
                Temp = SeedData.Items[itemIndex].Temp,
                DefaultUnitType = SeedData.Items[itemIndex].DefaultUnitType
            },
            Prep = prepIndex != null
                ? new PrepResponse
                {
                    Id = SeedData.Preps[prepIndex.Value].PrepId,
                    Name = SeedData.Preps[prepIndex.Value].PrepName,
                }
                : null,
            Amount = SeedData.CartItems[cartItemIndex].Amounts.Amount
        };
    }

    private static CartSelectRecipeResponse IndicesToRecipe(int recipeIndex)
    {
        return new CartSelectRecipeResponse
        {
            Id = SeedData.Recipes[recipeIndex].RecipeId,
            Name = SeedData.Recipes[recipeIndex].RecipeName,
            Quantity = SeedData.Recipes[recipeIndex].CartQuantity
        };
    }

    private async Task SetItemLocation(int storeIndex, int itemIndex, int aisleIndex, Bay bay)
    {
        await StoreController.Select(SeedData.Stores[storeIndex].StoreId);
        await ItemController.Edit(SeedData.Items[itemIndex].ItemId, new JsonPatchDocument<ItemEditRequest>
        {
            Operations =
            {
                new Operation<ItemEditRequest>()
                {
                    op = "replace",
                    path = "/Location",
                    value = new LocationEditRequest()
                    {
                        AisleId = SeedData.Aisles[aisleIndex].AisleId,
                        Bay = bay
                    }
                }
            }
        }).AssertIsSuccessful();
    }
}