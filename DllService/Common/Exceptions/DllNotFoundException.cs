using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DllService.Common.Exceptions
{
    public class DllNotFoundException: GlobalException
    {
        public DllNotFoundException(string mess):base(mess)
        {
            Status = System.Net.HttpStatusCode.NotFound;
        }
    }
}
