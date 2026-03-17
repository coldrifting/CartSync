using System.Diagnostics.CodeAnalysis;
using CartSyncBackend.Database.Interfaces;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.Mvc;

namespace CartSyncBackend.Controllers.Core;

public class ControllerCore : ControllerBase
{
    protected bool TryGetEditObject<TSource, TEdit>(TSource source, JsonPatchDocument<TEdit> patch, [NotNullWhen(true)] out TEdit? editRequest)
        where TSource : IEditable<TEdit> where TEdit : class
    {
        editRequest = source.ToEditRequest();

        try
        {
            patch.ApplyTo(editRequest, jsonPatchError =>
            {
                string key = jsonPatchError.AffectedObject.GetType().Name;
                ModelState.AddModelError(key, jsonPatchError.ErrorMessage);
            });
        }
        catch (ArgumentOutOfRangeException)
        {
            ModelState.AddModelError("error", "Index out of range");
            return false;
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("error", ex.Message);
            return false;
        }

        bool isModelValid = TryValidateModel(editRequest);
        
        return ModelState.IsValid && isModelValid;
    }
}