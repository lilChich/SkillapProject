using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Skillap.BLL.DTO;
using Skillap.BLL.Exceptions;
using Skillap.BLL.Interfaces.IServices;
using Skillap.BLL.Interfaces.JWT;
using Skillap.BLL.Models;
using Skillap.DAL.EF;
using Skillap.DAL.Entities;
using Skillap.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Skillap.BLL.Service
{
    public class UserService : IAuthService, IMasterService
    {
        public IMapper Mapper { get; set; }
        public IUnitOfWork UoW { get; set; }
        public IJwtGenerator jwtGenerator { get; set; }
        public DataContext Context { get; set; }
        public UserManager<ApplicationUsers> UserManager { get; set; }
        public SignInManager<ApplicationUsers> SignInManager { get; set; }
        public RoleManager<ApplicationRole> RoleManager { get; set; }

        public UserService(IMapper mapper, IUnitOfWork uow,
            IJwtGenerator jwtGenerator,
            UserManager<ApplicationUsers> userManager,
            SignInManager<ApplicationUsers> signInManager,
            RoleManager<ApplicationRole> roleManager,
            DataContext context)
        {
            Mapper = mapper;
            UoW = uow;
            UserManager = userManager;
            SignInManager = signInManager;
            RoleManager = roleManager;
            Context = context;
            this.jwtGenerator = jwtGenerator;
        }

        /*public async Task<SignInResult> Login(UserDTO userDto)
        {
            var user = await UserManager.FindByEmailAsync(userDto.Email);


            //return await SignInManager.PasswordSignInAsync(user, userDto.Password, userDto.RememberMe, false);

            var result = await SignInManager.PasswordSignInAsync(user, userDto.Password, userDto.RememberMe, false);

            if (result.Succeeded)
            { 
            }
        }*/

        public async Task<UserDTO> Login(UserDTO userDto)
        {
            try
            {
                var user = await UserManager.FindByEmailAsync(userDto.Email);

                if (user == null)
                {
                    throw new ValidationExceptions("Cannot find such a user", "");
                    //return userDto;                
                }

                var result = await SignInManager.PasswordSignInAsync(user, userDto.Password, userDto.RememberMe, false);

                if (result.Succeeded)
                {
                    return new UserDTO
                    {
                        FirstName = user.FirstName,
                        SecondName = user.SecondName,
                        NickName = user.NickName,
                        DateOfBirth = user.DateOfBirth,
                        Country = user.Country,
                        Education = user.Education,
                        Token = jwtGenerator.CreateToken(user),
                        Image = user.Image
                    };
                }
                else
                {
                    throw new ValidationExceptions("", "");
                }
            }
            catch
            {
                throw new ValidationExceptions("Password or Email is incorrect", "");
            }

            //return userDto;
        }

        public async Task<IdentityResult> AddUserAsync(UserDTO userDto, bool isPersistent)
        {
            var user = Mapper.Map<ApplicationUsers>(userDto);
            var res = await UserManager.CreateAsync(user, user.PasswordHash);

            if (!res.Succeeded)
            {

                return res;
            }

            await SignInManager.SignInAsync(user, isPersistent);

            if (!await RoleManager.RoleExistsAsync("Admin"))
            {
                await RoleManager.CreateAsync(new ApplicationRole() { Name = "Admin" });
                await Login(userDto);
            }
            if (userDto.Email.Contains("noobbogdan@gmail.com"))
            {
                await UserManager.AddToRoleAsync(user, "Admin");
            }

            if (!await RoleManager.RoleExistsAsync("Moderator"))
            {
                await RoleManager.CreateAsync(new ApplicationRole() { Name = "Moderator" });
            }

            if (!await RoleManager.RoleExistsAsync("User"))
            {
                await RoleManager.CreateAsync(new ApplicationRole() { Name = "User" });
            }
            if (userDto.Email.Contains("@"))
            {
                await UserManager.AddToRoleAsync(user, "User");
            }

            if (!await RoleManager.RoleExistsAsync("VIP"))
            {
                await RoleManager.CreateAsync(new ApplicationRole() { Name = "VIP" });
            }

            return res;
        }

        public async Task SignOut()
        {
            await SignInManager.SignOutAsync();
        }

        public async Task<IdentityResult> ChangePasswordAsync(UserDTO userDto, string currentPassword, string newPassword)
        {
            try
            {
                var user = await UserManager.FindByEmailAsync(userDto.Email);

                if (user == null)
                {
                    throw new ValidationExceptions("Cannot find such a user", "");
                }

                var res = await UserManager.ChangePasswordAsync(user, currentPassword, newPassword);

                if (!res.Succeeded)
                {
                    return res;
                }

                return res;
            }
            catch
            {
                throw new ValidationExceptions("Something goes wrong while updating", "");
            }
        }

        public async Task<UserDTO> GetUserAsync(string email)
        {
            try
            {
                return Mapper.Map<UserDTO>(await UserManager.FindByEmailAsync(email));
            }
            catch
            {
                return new UserDTO();
            }
        }

        public async Task<UserDTO> GetUserAsync(ClaimsPrincipal claim) => Mapper.Map<UserDTO>(await UserManager.GetUserAsync(claim));

        public async Task<UserDTO> GetUserClaimsAsync(ApplicationUsers user) => (UserDTO)await UserManager.GetClaimsAsync(user);

        public async Task<UserDTO> GetUserRoleAsync(ApplicationUsers user) => (UserDTO)await UserManager.GetRolesAsync(user);

        public async Task<bool> UpdateUserAsync(UserDTO userDto)
        {
            var user = await UserManager.FindByEmailAsync(userDto.Email);

            if ((user != null) && (user.Id != userDto.Id))
            {
                return false;
            }

            try
            {
                user.Id = userDto.Id;
                user.FirstName = userDto.FirstName;
                user.SecondName = userDto.SecondName;
                user.DateOfBirth = userDto.DateOfBirth;
                user.Education = userDto.Education;
                user.Email = userDto.Email;
                user.Gender = userDto.Gender;
                user.Country = userDto.Country;
                user.Image = userDto.Image;
                user.NickName = userDto.NickName;

                await UserManager.UpdateAsync(user);
            }
            catch
            {
                throw new Exception("Cannot update user");
            }
            /*if (userDto.Role == "User")
            {
               

            }*/

            await SignInManager.RefreshSignInAsync(user);

            return true;
        }

        public async Task<IdentityResult> DeleteUserAsync(ApplicationUsers user)
        {
            //var user = Mapper.Map<ApplicationUsers>(userDto);

            var res = await UserManager.DeleteAsync(user);

            return res;
        }

        public Task GetUser(int idUser)
        {
            var appUser = from u in Context.Users
                          where u.Id == idUser
                          select u;



            return (Task)appUser;

        }

        public async Task<int> GetUserByIdAsync(ClaimsPrincipal user) => (await UserManager.GetUserAsync(user)).Id;

        public async Task CreateMasterClass(MasterClassesDTO masterClassesDto)
        {
            var mappedMasterClass = Mapper.Map<MasterClasses>(masterClassesDto);
            var masterClass =  await GetMasterClassById(masterClassesDto.Id);

            if (masterClass != null)
            {
                return;
            }
            else
            {
                try
                {
                    await UoW.MasterClasses.CreateAsync(mappedMasterClass);
                    await UoW.MasterClasses.SaveAsync();
                }
                catch
                {
                    throw new Exception("Cannot create Master-class");
                }
            }
        }

        public async Task<bool> UpdateMasterClass(MasterClassesDTO masterClassesDto)
        {
            var masterClass = await GetMasterClassById(masterClassesDto.Id);
            var currentMasterClass = Mapper.Map<MasterClasses>(masterClass);

            if ((currentMasterClass != null) && (currentMasterClass.Id != masterClassesDto.Id))
            {
                return false;
            }
            else
            {
                try
                {
                    currentMasterClass.Id = masterClassesDto.Id;
                    currentMasterClass.Name = masterClassesDto.Name;
                    currentMasterClass.Description = masterClassesDto.Description;
                    currentMasterClass.Category = masterClassesDto.Category;
                    currentMasterClass.Relevance = masterClassesDto.Relevance;
                    currentMasterClass.Level = masterClassesDto.Level;

                    await UoW.MasterClasses.UpdateAsync(currentMasterClass);
                    await UoW.MasterClasses.SaveAsync();
                }
                catch
                {
                    throw new Exception("Cannot update Master-class");
                }
            }

            return true;
        }

        public async Task<bool> DeleteMasterClass(MasterClassesDTO masterClassesDto)
        {
            var masterClass = await GetMasterClassById(masterClassesDto.Id);
            var currentMasterClass = Mapper.Map<MasterClasses>(masterClass);

            if (currentMasterClass == null)
            {
                return false;
            }
            else
            {
                try
                {
                    await UoW.MasterClasses.DeleteAsync(currentMasterClass.Id);
                    await UoW.MasterClasses.SaveAsync();
                }
                catch
                {
                    throw new Exception("Cannot update Master-class");
                }
            }

            return true;
        }

        public async Task<bool> DeleteMaster(MastersDTO masterClassesDto)
        {
            var masterClass = await GetMasterClassById(masterClassesDto.Id);
            var currentMasterClass = Mapper.Map<MasterClasses>(masterClass);

            if (currentMasterClass == null)
            {
                return false;
            }
            else
            {
                try
                {
                    await UoW.MasterClasses.DeleteAsync(currentMasterClass.Id);
                    await UoW.MasterClasses.SaveAsync();
                }
                catch
                {
                    throw new Exception("Cannot update Master-class");
                }
            }

            return true;
        }

        public Task<bool> StatusInMastersTable(MastersDTO mastersDto)
        {
            throw new NotImplementedException();
        }

        public Task<int> ChangeSkillLevel(MastersDTO mastersDto)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateMasterClasses(MasterClasses masterClasses)
        {
            await UoW.MasterClasses.UpdateAsync(masterClasses);
        }
        
        public async Task<MasterClassesDTO> GetMasterClassById(int id) => Mapper.Map<MasterClassesDTO>(await UoW.MasterClasses.GetByIdAsync(id));

        public MasterClasses GetMasterClassByNameAndDescription(string name, string description)
        {
            /*var masterClass = from m in Context.MasterClasses
                              where m.Name == name
                              where m.Description == description
                              select m;*/

            var currentMasterClass = Context.MasterClasses.Where(m => m.Name == name).Where(m => m.Description == description).FirstOrDefault();

            return currentMasterClass;
        }

        public MasterClasses GetMasterClassByIdThroughDb(int id)
        {
            /*var masterClass = from m in Context.MasterClasses
                              where m.Name == name
                              where m.Description == description
                              select m;*/

            var currentMasterClass = Context.MasterClasses.Where(m => m.Id == id).FirstOrDefault();

            return currentMasterClass;
        }

        public async Task<Tags> GetTagByNameAsync(string name) => (await UoW.Tags.GetByNameAsync(name));

        public Tags GetTagByName(string name)
        {
            /*var masterClass = from m in Context.MasterClasses
                              where m.Name == name
                              where m.Description == description
                              select m;*/

            var currentTag = Context.Tags.Where(t => t.Name == name).FirstOrDefault();

            return currentTag;
        }

        public async Task<ApplicationUsers> GetUserByIdAsync(int id) => (await UoW.ApplicationUsers.GetByIdAsync(id));

        public async Task<TagsDTO> GetTagById(int id) => Mapper.Map<TagsDTO>(await UoW.Tags.GetByIdAsync(id));

        public async Task<Masters> GetMasterByIdAsync(int id) => (await UoW.Masters.GetByIdAsync(id));

        public IEnumerable<MasterClassesDTO> GetAllMasterClassesAsync()
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<MasterClasses, MasterClassesDTO>()).CreateMapper();
            return mapper.Map<IEnumerable<MasterClasses>, IEnumerable<MasterClassesDTO>>(UoW.MasterClasses.GetAll());
        }

        public IEnumerable<UserDTO> GetAllUsersAsync()
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<ApplicationUsers, UserDTO>()).CreateMapper();
            return mapper.Map<IEnumerable<ApplicationUsers>, IEnumerable<UserDTO>>(UoW.ApplicationUsers.GetAll());
        }

        public IEnumerable<MastersDTO> GetAllMastersAsync()
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Masters, MastersDTO>()).CreateMapper();
            return mapper.Map<IEnumerable<Masters>, IEnumerable<MastersDTO>>(UoW.Masters.GetAll());
        }

        public IEnumerable<TagsDTO> GetAllTags()
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Tags, TagsDTO>()).CreateMapper();
            return mapper.Map<IEnumerable<Tags>, IEnumerable<TagsDTO>>(UoW.Tags.GetAll());
        }

        public async Task CreateTagAsync(TagsDTO tagsDto)
        {
            var mappedTag = Mapper.Map<Tags>(tagsDto);
            var tag = GetTagByName(tagsDto.Name);

            if (tag != null)
            {
                return;
            }
            else
            {
                try
                {
                    await UoW.Tags.CreateAsync(mappedTag);
                }
                catch
                {
                    throw new Exception("Cannot create tag");
                }
            }
        }

        public async Task<bool> UpdateTagAsync(TagsDTO tagDto)
        {
            var tag = await GetTagByNameAsync(tagDto.Name);
            var currentTag = Mapper.Map<Tags>(tag);

            if ((currentTag != null) && (currentTag.Id != tagDto.Id))
            {
                return false;
            }
            else
            {
                try
                {
                    currentTag.Id = tagDto.Id;
                    currentTag.Name = tagDto.Name;

                    await UoW.Tags.UpdateAsync(currentTag);
                }
                catch
                {
                    throw new Exception("Cannot update Tag");
                }
            }

            return true;
        }

        public async Task<bool> DeleteTag(TagsDTO tagDto)
        {
            var tag = await GetTagByNameAsync(tagDto.Name);
            var currentTag = Mapper.Map<Tags>(tag);

            if (currentTag == null)
            {
                return false;
            }
            else
            {
                try
                {
                    await UoW.Tags.DeleteAsync(currentTag.Id);
                }
                catch
                {
                    throw new Exception("Cannot delete Tag");
                }
            }

            return true;
        }

        public IEnumerable<PostsDTO> GetAllPosts()
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Posts, PostsDTO>()).CreateMapper();
            return mapper.Map<IEnumerable<Posts>, IEnumerable<PostsDTO>>(UoW.Posts.GetAll());
        }

        public Posts GetPostByNameAndDescription(string name, string description)
        {
            var currentPost = Context.Posts.Where(p => p.Name == name).Where(p => p.Description == description).FirstOrDefault();

            return currentPost;
        }

        public async Task<PostsDTO> GetPostById(int id) => Mapper.Map<PostsDTO>(await UoW.Posts.GetByIdAsync(id));

        public async Task CreatePostAsync(PostsDTO postsDto)
        {
            var mappedPost = Mapper.Map<Posts>(postsDto);

           
                try
                {
                    await UoW.Posts.CreateAsync(mappedPost);
                }
                catch 
                {
                    throw new Exception("Cannot create post");
                }
            
        }

        public async Task CreateLikedPostAsync(Liked_PostsDTO dto)
        {
            var mappedLikedPost = Mapper.Map<Liked_Posts>(dto);

            try
            {
                await UoW.LikedPosts.CreateAsync(mappedLikedPost);
            }
            catch
            {
                throw new Exception("Cannot create likedPost");
            }
        }

        public async Task<PostMenuModel> LoadPostsAsync(string name, int page, SortType sort, int amountOfElementsOnPage)
        {
            var posts = await UoW.Posts.FindAsync(i => true);

            if (!string.IsNullOrEmpty(name))
            {
                posts = posts.Where(i => i.Name.Contains(name));
            }

            posts = sort switch
            {
                SortType.NameDesc => posts.OrderByDescending(i => i.Name),
                SortType.DateAsc => posts.OrderBy(i => i.CreatedTime),
                SortType.DateDesc => posts.OrderByDescending(i => i.CreatedTime),
                SortType.StatusAsc => posts.OrderBy(i => i.Status),
                SortType.StatusDesc => posts.OrderByDescending(i => i.Status),
                _ => posts.OrderBy(i => i.Name),
            };
            var count = posts.Count();
            var items = posts.Skip((page - 1) * amountOfElementsOnPage).Take(amountOfElementsOnPage).ToList();

            var mappedItems = Mapper.Map<IEnumerable<PostsDTO>>(items);

            PostMenuModel viewModel = new PostMenuModel()
            {
                PageViewModel = new PageViewModel(count, page, amountOfElementsOnPage),
                SortViewModel = new SortViewModel(sort),
                FilterViewModel = new FilterViewModel(name),
                Posts = mappedItems
            };
            return viewModel;
        }
    }
}
    