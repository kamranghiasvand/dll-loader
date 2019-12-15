using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DllService.Model;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace DllService.app.Model.Binder
{
    public class TypeLoadEntityBinder : IModelBinder
    {
        Dictionary<Type, (ModelMetadata, IModelBinder)> binders;
        public TypeLoadEntityBinder(Dictionary<Type, (ModelMetadata, IModelBinder)> binders)
        {
            this.binders = binders;
        }
        public async Task BindModelAsync(ModelBindingContext ctx)
        {
            var modelKindName = ModelNames.CreatePropertyModelName(ctx.ModelName, nameof(AbstractArg.Kind));            
            var modelTypeValue = ctx.ValueProvider.GetValue(modelKindName).FirstValue;
            IModelBinder modelBinder;
            ModelMetadata modelMetadata;
            if(modelTypeValue==null)
            {
                ctx.Result=ModelBindingResult.Failed();
                return;
            }
            if (modelTypeValue.ToLower().Trim() == "primitive")
            {
                (modelMetadata, modelBinder) = binders[typeof(PrimitiveArg)];
            }
            else if (modelTypeValue.ToLower().Trim() == "custom")
            {
                (modelMetadata, modelBinder) = binders[typeof(TypeLoadEntity)];
            }
            else
            {
                ctx.Result = ModelBindingResult.Failed();
                return;
            }
            var newBindingContext = DefaultModelBindingContext.CreateBindingContext(
            ctx.ActionContext,
            ctx.ValueProvider,
            modelMetadata,
            bindingInfo: null,
            ctx.ModelName);
            await modelBinder.BindModelAsync(newBindingContext);
            ctx.Result = newBindingContext.Result;
            if (newBindingContext.Result.IsModelSet)
        {
            // Setting the ValidationState ensures properties on derived types are correctly 
            ctx.ValidationState[newBindingContext.Result] = new ValidationStateEntry
            {
                Metadata = modelMetadata,
            };
        }
        }
    }
}