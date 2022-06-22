using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skillap.BLL.Exceptions
{
    public class ValidationExceptions : Exception
    {
        public string Property { get; protected set; }
        public ValidationExceptions(string message, string prop) : base(message)
        {
            Property = prop;
        }
    }
}
