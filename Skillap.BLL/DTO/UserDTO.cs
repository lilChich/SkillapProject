using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skillap.BLL.DTO
{
    public class UserDTO
    {
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string NickName { get; set; }
        public bool Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Country { get; set; }
        public string Education { get; set; }
        public byte?[] Image { get; set; }
        public string Token { get; set; }
    }
}
