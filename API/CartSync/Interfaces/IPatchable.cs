using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CartSync.Interfaces;

public interface IPatchable<TEdit> where TEdit : class
{
    public bool TryGetPatch(ModelStateDictionary modelState, JsonPatchDocument<TEdit> jsonPatch, out TEdit patch);
    public void ApplyPatch(TEdit patch);
}

public interface IPatchableStoreSpecific<TEdit> where TEdit : class
{
    public bool TryGetPatch(ModelStateDictionary modelState, JsonPatchDocument<TEdit> jsonPatch, Ulid storeId, out TEdit patch);
    public void ApplyPatch(TEdit patch, Ulid storeId);
}