using DllService.app.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DllService.Validation
{
    public delegate List<string> Rule<T>(T t);

    public class RuleFactory
    {
        public static Rule<T> nullRule<T>(Func<T, object>  supplier, string field)
        {
            return (t) =>
            {
                var value = supplier.Invoke(t);
                if (value == null)
                    return new List<string>() { string.Format("field {0} is null", field) };
                return null;
            };
        }
        public static Rule<T> emptyRule<T>(Func<T,string> supplier,string field)
        {
            return (t) =>
            {
                var value = supplier.Invoke(t);
                if (String.IsNullOrEmpty(value))
                    return new List<string>() { string.Format("Field {0} is empty", field) };
                return null;
            };
        }
    }
}
