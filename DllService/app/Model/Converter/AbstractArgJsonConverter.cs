using System;
using DllService.Model;
using Newtonsoft.Json.Linq;

namespace DllService.app.Model.Converter
{
    public class AbstractArgJsonConverter : JsonCreationConverter<AbstractArg>
    {
        protected override AbstractArg Create(Type objectType, JObject jObject)
        {
            if (jObject == null) throw new ArgumentNullException("jObject");
           
            if (jObject["Kind"].Value<String>().Equals("custom"))
            {
                return new TypeLoadEntity();
            }
            if (jObject["Kind"].Value<String>().Equals("primitive"))
            {
                return new PrimitiveArg();
            }
            return null;
        }
    }
}