using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CartSyncBackendTests.Core;

public class ModelValidator : IObjectModelValidator
{
    public void Validate(ActionContext actionContext, ValidationStateDictionary? validationState, string prefix, object? model)
    {
        if (model is null)
        {
            return;
        }
        
        ValidationContext context = new(model, serviceProvider: null, items: null);
        List<ValidationResult> results = [];

        bool isValid = Validator.TryValidateObject(
            model, context, results,
            validateAllProperties: true
        );

        if (isValid)
        {
            return;
        }

        results.ForEach((r) =>
        {
            // Add validation errors to the ModelState
            actionContext.ModelState.AddModelError("", r.ErrorMessage?.ToString() ?? string.Empty);
        });
    }
}