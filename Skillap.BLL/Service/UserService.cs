using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Skillap.BLL.DTO;
using Skillap.BLL.Interfaces.IServices;
using Skillap.BLL.Interfaces.JWT;
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
            var user = await UserManager.FindByEmailAsync(userDto.Email);


            //return await SignInManager.PasswordSignInAsync(user, userDto.Password, userDto.RememberMe, false);

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

            throw new Exception();
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

            if (userDto.Role == "User")
            {
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

            }

            await SignInManager.RefreshSignInAsync(user);

            return true;
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
                catch(Exception ex)
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
                catch(Exception ex)
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
                catch(Exception ex)
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

        public async Task<MasterClassesDTO> GetMasterClassById(int id) => Mapper.Map<MasterClassesDTO>(await UoW.MasterClasses.GetByIdAsync(id));
    }
}
