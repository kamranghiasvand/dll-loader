using DllService.app.Model.Binder;
using DllService.app.Model.Converter;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DllService.app.Model
{
    //  [ModelBinder(BinderType = typeof(TypeLoadEntityBinder))]
    [JsonConverter(typeof(AbstractArgJsonConverter))]
    public class AbstractArg
    {
        public string Kind { get; set; }
    }
   
}