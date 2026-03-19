using System.Diagnostics.CodeAnalysis;
using CartSyncBackend.Models.Interfaces;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.Mvc;

namespace CartSyncBackend.Controllers.Core;

public class ControllerCore : ControllerBase
{
    protected bool TryGetEditObject<TEdit>(IEditable<TEdit> source, JsonPatchDocument<TEdit> patch, [NotNullWhen(true)] out TEdit? editRequest, Ulid? storeId = null)
        where TEdit : class
    {
        editRequest = source.ToEditRequest(storeId);

        patch.ApplyTo(editRequest, jsonPatchError =>
        {
            string key = jsonPatchError.AffectedObject.GetType().Name;
            ModelState.AddModelError(key, jsonPatchError.ErrorMessage);
        });

        bool isModelValid = TryValidateModel(editRequest);
        
        return ModelState.IsValid && isModelValid;
    }
}