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
            return new TypeLoadEntityBinder();
        }
    }
}