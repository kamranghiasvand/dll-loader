using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DllService.Common.Exceptions
{
    public class DllAlreadyLoadedException : GlobalException
    {
        public DllAlreadyLoadedException()
        {
            this.Status = HttpStatusCode.NotAcceptable;
        }
    }
}
