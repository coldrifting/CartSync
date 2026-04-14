using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CartSync.Utils;

public static class Patch
{
    public static bool TryPatch<TEntity, TEdit>(ModelStateDictionary modelState, TEntity entity, JsonPatchDocument<TEdit> patch, ref TEdit editRequest) where TEdit : class
    {
        int errorCount = 0;
        
        patch.ApplyTo(editRequest, error =>
        {
            errorCount++;
            string key = error.AffectedObject.GetType().Name;
            modelState.AddModelError(key, error.ErrorMessage);
        });

        return errorCount == 0;
    }
}