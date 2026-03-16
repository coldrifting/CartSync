using System.ComponentModel.DataAnnotations;
using CartSyncBackend.Database;
using CartSyncBackend.Database.Models;
using CartSyncBackend.Database.Objects;
using CartSyncBackend.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CartSyncBackend.Controllers;


[ApiController]
[Tags("Items")]
[Route("/api/items/[action]")]
public class ItemController(CartSyncContext db) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ItemResponse>))]
    public async Task<IActionResult> All()
    {
        List<ItemResponse> allItems = await db.Items
            .Select(i => new ItemResponse
            {
                ItemId = i.ItemId,
                ItemName = i.ItemName,
                ItemTemp = i.ItemTemp,
                DefaultUnitType = i.DefaultUnitType,
                CartAmount = i.CartAmount,
                Preps = i.Preps.Select(p => new PrepResponse
                {
                    PrepId = p.PrepId,
                    PrepName = p.PrepName
                }).ToList()
            })
            .OrderBy(i => i.ItemName)
            .ToListAsync();
        
        return Ok(allItems);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ItemAisleResponse>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    [Route("/api/items/all/located")]
    public async Task<IActionResult> AllWithLocation([Required] Ulid storeId)
    {
        Store? s = await db.Stores.FindAsync(storeId);
        if (s == null)
        {
            return Error.NotFoundStore;
        }
    
        // Unlocated Items
        ItemAisleResponse unlocated = new()
        {
            Aisle = new AisleResponse
            {
                AisleId = Ulid.Empty,
                AisleName = "(No Location)"
            },
            Items = db.Items
                .Where(i => i.Aisles.All(a => a.Store != s))
                .Select(i => new ItemResponse
                {
                    ItemId = i.ItemId,
                    ItemName = i.ItemName,
                    ItemTemp = i.ItemTemp,
                    DefaultUnitType = i.DefaultUnitType,
                    CartAmount = i.CartAmount,
                    Preps = i.Preps.Select(p => new PrepResponse
                    {
                        PrepId = p.PrepId,
                        PrepName = p.PrepName
                    }).ToList()
                })
                .OrderBy(i => i.ItemName)
                .ToList()
        };
        
        // Items associated with an Aisle
        List<ItemAisleResponse> located = await db.Aisles
            .Where(a => a.Store == s)
            .Select(a => new ItemAisleResponse
            {
            Aisle = new AisleResponse()
            {
            AisleId = a.AisleId,
            AisleName = a.AisleName,
            SortOrder = a.SortOrder
        },
        Items = a.Items.Select(i => new ItemResponse
        {
            ItemId = i.ItemId,
            ItemName = i.ItemName,
            ItemTemp = i.ItemTemp,
            DefaultUnitType = i.DefaultUnitType,
            CartAmount = i.CartAmount,
            Preps = i.Preps.Select(p => new PrepResponse
            {
                PrepId = p.PrepId,
                PrepName = p.PrepName
            }).ToList()
        }).ToList()
        })
        .ToListAsync();
    
        List<ItemAisleResponse> result = unlocated.Items.Count != 0 ? located.Prepend(unlocated).ToList() : located;
        return Ok(result);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ItemResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    public async Task<IActionResult> Details([Required] Ulid itemId)
    {
        ItemResponse? itemResponse = await db.Items
            .Select(i => new ItemResponse
            {
                ItemId = i.ItemId,
                ItemName = i.ItemName,
                ItemTemp = i.ItemTemp,
                DefaultUnitType = i.DefaultUnitType,
                CartAmount = i.CartAmount,
                Preps = i.Preps.Select(p => new PrepResponse
                {
                    PrepId = p.PrepId,
                    PrepName = p.PrepName
                }).ToList()
            })
            .GetAsync(itemId);

        if (itemResponse == null)
        {
            return Error.NotFoundItem;
        }

        return Ok(itemResponse);
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    public async Task<IActionResult> Add([FromBody] ItemAddRequest itemAddRequest)
    {
        if (!ModelState.IsValid)
        {
            List<string> errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            
            return Error.BadRequestItemAddRequestInvalid(errors);
        }
        
        db.Add(new Item
        {
            ItemName = itemAddRequest.ItemName,
            ItemTemp = itemAddRequest.ItemTemp ?? ItemTemp.Ambient,
            DefaultUnitType = itemAddRequest.DefaultUnitType ?? UnitType.Count
        });
        await db.SaveChangesAsync();
        
        return NoContent();
    }
    
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    public async Task<IActionResult> Edit([Required] Ulid itemId, [FromBody] ItemEditRequest itemEditRequest)
    {
        switch (ModelState.IsValid)
        {
            case false when itemId == Ulid.Empty:
                return Error.BadRequestItemIdInvalid;
            case false:
            {
                List<string> errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
            
                return Error.BadRequestItemEditRequestInvalid(errors);
            }
        }

        Item? item = await db.Items
            .Include(p => p.Preps)
            .GetAsync(itemId);
        if (item == null)
        {
            return Error.NotFoundItem;
        }
        
        item.ItemName = itemEditRequest.ItemName ?? item.ItemName;
        item.ItemTemp = itemEditRequest.ItemTemp ?? item.ItemTemp;
        item.DefaultUnitType = itemEditRequest.DefaultUnitType ?? item.DefaultUnitType;
        item.CartAmount = itemEditRequest.CartAmount ?? item.CartAmount;

        if (itemEditRequest.PrepIds != null)
        {
            item.Preps.Clear();
            
            foreach (Ulid prepId in itemEditRequest.PrepIds)
            {
                Prep? prep = await db.Preps.FindAsync(prepId);
                if (prep != null)
                {
                    item.Preps.Add(prep);
                }
            }
        }
        
        await db.SaveChangesAsync();
        
        return NoContent();
    }
    
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    [Route("/api/items/edit/[action]")]
    public async Task<IActionResult> Location([Required] Ulid itemId, [Required] Ulid storeId, [FromBody] ItemAisleLocChangeRequest itemAisleLocChangeRequest)
    {
        if (!ModelState.IsValid)
        {
            List<string> errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            
            return Error.BadRequestItemEditRequestInvalid(errors);
        }
        
        Item? i = db.Items.FirstOrDefault(i => i.ItemId == itemId);
        if (i == null)
        {
            return Error.NotFoundItem;
        }
        
        Store? s = await db.Stores
            .Include(s => s.Aisles)
            .GetAsync(storeId);
        if (s == null)
        {
            return Error.NotFoundStore;
        }

        if (db.Aisles.All(a => a.AisleId != itemAisleLocChangeRequest.AisleId))
        {
            return Error.NotFoundAisle;
        }
        
        if (s.Aisles.All(a => a.AisleId != itemAisleLocChangeRequest.AisleId))
        {
            return Error.BadRequestAisleNotUnderStore;
        }
        
        ItemAisle? itemAisle = await db.ItemAisles.GetAsync((itemId, s.StoreId));
        if (itemAisle == null)
        {
            ItemAisle newItemAisle = new()
            {
                AisleId = itemAisleLocChangeRequest.AisleId,
                ItemId = itemId,
                Bay = itemAisleLocChangeRequest.Bay ?? BayType.Middle
            };
            db.ItemAisles.Add(newItemAisle);
        }
        else
        {
            itemAisle.AisleId = itemAisleLocChangeRequest.AisleId;
            itemAisle.Bay = itemAisleLocChangeRequest.Bay ?? BayType.Middle;
        }

        await db.SaveChangesAsync();
        return NoContent();
    }
    
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    public async Task<IActionResult> Delete([Required] Ulid itemId)
    {
        Item? i = await db.Items.FindAsync(itemId);
        if (i == null)
        {
            return Error.NotFoundItem;
        }

        db.Items.Remove(i);
        await db.SaveChangesAsync();
        
        return NoContent();
    }
}