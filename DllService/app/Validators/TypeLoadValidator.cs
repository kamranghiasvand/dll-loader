using DllService.app.Model;
using DllService.Model;
using DllService.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DllService.app.Validators
{
    public class TypeLoadValidator : ValidationContext<TypeLoadEntity>
    {
        public override void init()
        {
            validations.Add(RuleFactory.emptyRule<TypeLoadEntity>(m => m.DllName, "DllName"));
            validations.Add(RuleFactory.emptyRule<TypeLoadEntity>(m => m.FullClassName, "FullClassName"));
            validations.Add(RuleFactory.emptyRule<TypeLoadEntity>(m => m.MethodName, "MethodName"));
            validations.Add(RuleFactory.nullRule<TypeLoadEntity>(m => m.LoadType, "LoadType"));
            validations.Add(checkArgs);
        }
        private List<string> checkArgs(TypeLoadEntity entity)
        {
            if (entity.ConstructorArgs != null)
            {
                for (int i = 0; i < entity.ConstructorArgs.Count; i++)
                {
                    if (entity.ConstructorArgs[i] is TypeLoadEntity arg)
                    {
                        if (string.IsNullOrEmpty(arg.DllName))
                        {
                            return new List<string> { string.Format("Field {0} of ConstructorArgs[{1}] is empty", "DllName", i) };
                        }
                        if (string.IsNullOrEmpty(arg.FullClassName))
                        {
                            return new List<string> { string.Format("Field {0} of ConstructorArgs[{1}] is empty", "FullClassName", i) };
                        }
                    }
                    if (entity.ConstructorArgs[i] is PrimitiveArg primitive)
                    {
                        if (primitive.Arg == null)
                        {
                            return new List<string> { string.Format("Field {0} of  ConstructorArgs[{1}] is empty", "Arg", i) };
                        }
                    }
                }
            }
            if (entity.MethodArgs != null)
            {
                for (int i = 0; i < entity.MethodArgs.Count; i++)
                {
                    if (entity.MethodArgs[i] is TypeLoadEntity arg)
                    {
                        if (string.IsNullOrEmpty(arg.DllName))
                        {
                            return new List<string> { string.Format("Field {0} of MethodArgs[{1}] is empty", "DllName", i) };
                        }
                        if (string.IsNullOrEmpty(arg.FullClassName))
                        {
                            return new List<string> { string.Format("Field {0} of MethodArgs[{1}] is empty", "FullClassName", i) };
                        }
                    }
                    if (entity.MethodArgs[i] is PrimitiveArg primitive)
                    {
                        if (primitive.Arg == null)
                        {
                            return new List<string> { string.Format("Field {0} of  MethodArgs[{1}] is empty", "Arg", i) };
                        }
                    }
                }
            }
            return null;
        }
    }
}
