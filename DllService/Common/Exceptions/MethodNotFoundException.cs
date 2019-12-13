using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DllService.Common.Exceptions
{
    public class MethodNotFoundException:GlobalException
    {
        public MethodNotFoundException(string mess):base(mess)
        {
            Status = System.Net.HttpStatusCode.NotFound;
        }
    }
}
