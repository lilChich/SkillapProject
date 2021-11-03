using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Skillap.BLL.Exceptions
{
    public class RestException : Exception
    {
        public HttpStatusCode Code { get; }
        public object Errors { get; set; }

        public RestException(HttpStatusCode code, object errors = null)
        {
            Code = code;
            Errors = errors;
        }
    }
}
