using System.ComponentModel.DataAnnotations;
using CartSyncBackend.Database;
using CartSyncBackend.Database.Models;
using CartSyncBackend.Database.Objects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CartSyncBackend.Controllers;

[ApiController]
[Tags("Recipes - Section Entries")]
[Route("/api/recipes/sections/entries/[action]")]
public class RecipeSectionEntryController(CartSyncContext db) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    public async Task<IActionResult> Add([Required] Ulid recipeSectionId, [Required] RecipeSectionEntryAddRequest recipeSectionEntryAddRequest)
    {
        switch (ModelState.IsValid)
        {
            case false when recipeSectionId == Ulid.Empty:
                return Error.BadRequestRecipeSectionIdInvalid;
            case false:
            {
                List<string> errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
            
                return Error.BadRequestRecipeSectionEntryAddRequestInvalid(errors);
            }
        }
        
        RecipeSection? recipeSection = await db.RecipeSections
            .Include(rs => rs.RecipeSectionEntries)
            .FirstOrDefaultAsync(r => r.RecipeSectionId == recipeSectionId);
        if (recipeSection == null)
        {
            return Error.NotFoundRecipeSection;
        }
        
        Item? item = await db.Items.FindAsync(recipeSectionEntryAddRequest.ItemId);
        if (item == null)
        {
            return Error.NotFoundItem;
        }
        
        Prep? prep = await db.Preps.FindAsync(recipeSectionEntryAddRequest.PrepId);
        if (recipeSectionEntryAddRequest.PrepId != null && prep == null)
        {
            return Error.NotFoundPrep;
        }

        RecipeSectionEntry entry = new()
        {
            RecipeSectionId = recipeSection.RecipeSectionId,
            RecipeSectionEntryIndex = recipeSection.RecipeSectionEntries.Count,
            Item = item,
            Prep = prep,
            Amount = recipeSectionEntryAddRequest.Amount
        };

        recipeSection.RecipeSectionEntries.Add(entry);
        await db.SaveChangesAsync();
        
        return NoContent();
    }
    
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    public async Task<IActionResult> Edit([Required] Ulid recipeSectionEntryId, [FromBody] RecipeSectionEntryEditRequest recipeSectionEntryEditRequest)
    {
        switch (ModelState.IsValid)
        {
            case false when recipeSectionEntryId == Ulid.Empty:
                return Error.BadRequestRecipeSectionEntryIdInvalid;
            case false:
            {
                List<string> errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
            
                return Error.BadRequestRecipeSectionEntryEditRequestInvalid(errors);
            }
        }

        RecipeSectionEntry? recipeSectionEntry = await db.RecipeSectionEntries.FindAsync(recipeSectionEntryId);
        if (recipeSectionEntry == null)
        {
            return Error.NotFoundRecipeSectionEntry;
        }

        if (recipeSectionEntryEditRequest.ItemId is { } itemId)
        {
            Item? item = await db.Items.FindAsync(itemId);
            if (item == null)
            {
                return Error.NotFoundItem;
            }
            
            recipeSectionEntry.Item = item;
        }

        switch (recipeSectionEntryEditRequest.PrepId)
        {
            case null when recipeSectionEntryEditRequest.UpdatePrep:
                recipeSectionEntry.Prep = null;
                break;
            case { } prepId:
            {
                Prep? prep = await db.Preps.FindAsync(prepId);
                if (prep == null)
                {
                    return Error.NotFoundPrep;
                }
            
                recipeSectionEntry.Prep = prep;
                break;
            }
        }
        
        recipeSectionEntry.Amount = recipeSectionEntryEditRequest.Amount ?? recipeSectionEntry.Amount;
        
        if (recipeSectionEntryEditRequest.RecipeSectionEntryIndex is { } newIndex)
        {
            int oldIndex = recipeSectionEntry.RecipeSectionEntryIndex;

            if (newIndex != oldIndex)
            {
                // Insert at correct sorting index
                RecipeSection? recipeSection = await db.RecipeSections
                    .Include(rs => rs.RecipeSectionEntries)
                    .FirstOrDefaultAsync(rs => rs.RecipeSectionId == recipeSectionEntry.RecipeSectionId);

                if (recipeSection != null)
                {
                    RecipeSectionEntry[] sectionEntries = recipeSection.RecipeSectionEntries.OrderBy(rse => rse.RecipeSectionEntryIndex).ToArray();

                    int[] indices = SortHelper.Reorder(sectionEntries.Length, oldIndex, newIndex);
                    for (int i = 0; i < sectionEntries.Length; i++)
                    {
                        sectionEntries[indices[i]].RecipeSectionEntryIndex = i;
                    }
                }
            }
        }
        
        await db.SaveChangesAsync();
        return NoContent();
    }
    
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    public async Task<IActionResult> Delete([Required] Ulid recipeSectionEntryId)
    {
        if (!ModelState.IsValid && recipeSectionEntryId == Ulid.Empty)
        {
            return Error.BadRequestRecipeSectionEntryIdInvalid;
        }

        RecipeSectionEntry? recipeSectionEntry = await db.RecipeSectionEntries.FindAsync(recipeSectionEntryId);
        if (recipeSectionEntry == null)
        {
            return Error.NotFoundRecipeSectionEntry;
        }

        db.RecipeSectionEntries.Remove(recipeSectionEntry);

        // Normalize order index
        if (recipeSectionEntry.RecipeSectionEntryIndex != 0)
        {
            RecipeSection? recipeSection = await db.RecipeSections
                .Include(rs => rs.RecipeSectionEntries)
                .FirstOrDefaultAsync(rs => rs.RecipeId == recipeSectionEntry.RecipeSectionId);

            if (recipeSection != null)
            {
                // Normalize order index
                int index = 0;
                foreach (RecipeSectionEntry rse in recipeSection.RecipeSectionEntries.OrderBy(rse => rse.RecipeSectionEntryIndex))
                {
                    rse.RecipeSectionEntryIndex = index++;
                }
            }
        }

        await db.SaveChangesAsync();
        
        return NoContent();
    }
}