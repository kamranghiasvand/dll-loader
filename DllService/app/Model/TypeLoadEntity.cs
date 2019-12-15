
using System.Collections.Generic;
using DllService.app.Model;

namespace DllService.Model
{
    public class TypeLoadEntity:AbstractArg
    {

        public string DllName { get; set; }
        public string FullClassName{get;set;}
        public string MethodName { get; set; }
        public LoadType LoadType { get; set; }
        public List<AbstractArg> ConstructorArgs { get; set; } = new List<AbstractArg>();
        public List<AbstractArg> MethodArgs { get; set; } = new List<AbstractArg>();
    }
}