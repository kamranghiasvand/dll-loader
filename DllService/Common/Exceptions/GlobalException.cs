using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DllService.Common.Exceptions
{
    public class GlobalException :Exception
    {
        public GlobalException()
        {

        }
        public GlobalException(string mess):base(mess)
        {
           
        }
        public HttpStatusCode Status { get; set; } = HttpStatusCode.InternalServerError;

    }
}
