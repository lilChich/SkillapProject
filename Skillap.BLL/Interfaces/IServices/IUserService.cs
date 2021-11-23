using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Skillap.BLL.DTO;
using Skillap.DAL.Entities;

namespace Skillap.BLL.Interfaces.IServices
{
    public interface IAuthService
    {
        public Task<IdentityResult> AddUserAsync(UserDTO userDto, bool isPersistent);
        public Task<UserDTO> Login(UserDTO userDto);
        public Task SignOut();

        public Task<UserDTO> GetUserAsync(string email);
        public Task<UserDTO> GetUserAsync(ClaimsPrincipal claim);
        public Task<UserDTO> GetUserClaimsAsync(ApplicationUsers user);
        public Task<UserDTO> GetUserRoleAsync(ApplicationUsers user);
        public Task<bool> UpdateUserAsync(UserDTO userDto);
        public Task GetUser(int idUser);
        public Task<int> GetUserByIdAsync(ClaimsPrincipal user);

    }
}
