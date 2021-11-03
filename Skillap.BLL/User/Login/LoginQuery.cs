using MediatR;
using Skillap.BLL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skillap.BLL.User.Login
{
    public class LoginQuery : IRequest<UserDTO>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
