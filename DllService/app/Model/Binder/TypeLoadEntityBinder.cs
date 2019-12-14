using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DllService.app.Model.Binder
{
    public class TypeLoadEntityBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            throw new System.NotImplementedException();
        }
    }
}