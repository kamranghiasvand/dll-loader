using DllService.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DllService.Validation
{
    public abstract class ValidationContext<T>
    {
        protected List<Rule<T>> validations = new List<Rule<T>>();

        public void validate(T t)
        {
            foreach (Rule<T> validate in validations)
            {
                List<String> validate1 = validate.Invoke(t);
                if (validate1 != null && validate1.Count() != 0)
                    throw new ValidationException(validate1.First());
            }
        }
        public abstract void init();
    }
}
