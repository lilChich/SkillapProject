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
        public Task<IdentityResult> ChangePasswordAsync(UserDTO userDto, string currentPassword, string newPassword);
        public Task SignOut();

        public Task<UserDTO> GetUserAsync(string email);
        public Task<UserDTO> GetUserAsync(ClaimsPrincipal claim);
        public Task<UserDTO> GetUserClaimsAsync(ApplicationUsers user);
        public Task<UserDTO> GetUserRoleAsync(ApplicationUsers user);
        public Task<bool> UpdateUserAsync(UserDTO userDto);
        public Task GetUser(int idUser);
        public Task<ApplicationUsers> GetUserByIdAsync(int id);

        public Task<IdentityResult> DeleteUserAsync(ApplicationUsers user);

        public Task<int> GetUserByIdAsync(ClaimsPrincipal user);

        public Task<Tags> GetTagByNameAsync(string name);
        public Tags GetTagByName(string name);
        public Task<TagsDTO> GetTagById(int id);
        public Task CreateTagAsync(TagsDTO tagsDto);
        public Task<bool> UpdateTagAsync(TagsDTO tagDto);
        public Task<bool> DeleteTag(TagsDTO tagDto);


        public Posts GetPostByNameAndDescription(string name, string description);
        public Task<PostsDTO> GetPostById(int id);
        public Task CreatePostAsync(PostsDTO postsDto);

        public Task CreateLikedPostAsync(Liked_PostsDTO dto);


        public IEnumerable<UserDTO> GetAllUsersAsync();
        public IEnumerable<TagsDTO> GetAllTags();
        public IEnumerable<PostsDTO> GetAllPosts();


    }
}
