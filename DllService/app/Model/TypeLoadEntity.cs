
using System.Collections.Generic;

namespace DllService.Model
{
    public class TypeLoadEntity
    {

        public string DllName { get; set; }
        public string FullClassName{get;set;}
        public string MethodName { get; set; }
        public LoadType LoadType { get; set; }
        public List<object> ConstructorArgs { get; set; } = new List<object>();
        public List<object> MethodArgs { get; set; } = new List<object>();
    }
}