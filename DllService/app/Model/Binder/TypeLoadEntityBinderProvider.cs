using System;
using System.Collections.Generic;
using DllService.Model;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DllService.app.Model.Binder
{
    public class TypeLoadEntityBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType != typeof(TypeLoadEntity))
                return null;
            var subclasses = new[] { typeof(PrimitiveArg), typeof(TypeLoadEntity), };

            var binders = new Dictionary<Type, (ModelMetadata, IModelBinder)>();
            foreach (var type in subclasses)
            {
                var modelMetadata = context.MetadataProvider.GetMetadataForType(type);
                binders[type] = (modelMetadata, context.CreateBinder(modelMetadata));
            }
            return new TypeLoadEntityBinder(binders);
        }
    }
}