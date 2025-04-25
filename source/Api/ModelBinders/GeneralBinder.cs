using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace Api.ModelBinders;

public class GeneralBinder
{
    
}

// 1. The binder itself
public class GuidBackedValueObjectModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        // 1. get the incoming raw value (from route, query, etc)
        var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.FieldName);
        var str = valueResult.FirstValue;
        if (string.IsNullOrEmpty(str))
            return Task.CompletedTask;

        // 2. try to parse it as a Guid
        if (!Guid.TryParse(str, out var guidValue))
        {
            bindingContext.ModelState.TryAddModelError(bindingContext.ModelName,
                $"Cannot convert '{str}' to a Guid-backed value object.");
            return Task.CompletedTask;
        }

        // 3. invoke the single‐Guid ctor to create your VO
        var targetType = bindingContext.ModelType;
        var ctor = targetType.GetConstructor(new[] { typeof(Guid) });
        if (ctor == null)
            throw new InvalidOperationException(
                $"No single-Guid constructor found on {targetType.Name}");

        var model = ctor.Invoke([guidValue]);
        bindingContext.Result = ModelBindingResult.Success(model);
        return Task.CompletedTask;
    }
}

// 2. A provider that tells MVC “use that binder for any type that matches our rule”
public class GuidBackedValueObjectModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        var t = context.Metadata.ModelType;

        // only if there’s a public ctor(Guid) …
        if (t.GetConstructor(new[] { typeof(Guid) }) != null)
            return new BinderTypeModelBinder(typeof(GuidBackedValueObjectModelBinder));

        return null;
    }
}