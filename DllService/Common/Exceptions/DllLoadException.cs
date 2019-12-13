using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DllService.Common.Exceptions
{
    public class DllLoadException:GlobalException
    {
        public DllLoadException()
        {
            Status = System.Net.HttpStatusCode.InternalServerError;
        }
    }
}
