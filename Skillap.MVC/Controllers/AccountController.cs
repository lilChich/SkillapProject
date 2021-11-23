using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Skillap.BLL.DTO;
using Skillap.BLL.Interfaces.IServices;
using Skillap.BLL.Interfaces.JWT;
using Skillap.DAL.EF;
using Skillap.DAL.Entities;
using Skillap.DAL.Interfaces;
using Skillap.MVC.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Skillap.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService userService;
        private readonly IJwtGenerator jwtGenerator;
        private readonly IMapper mapper;
        private readonly IUnitOfWork UoW;
        private readonly UserManager<ApplicationUsers> appUser;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly DataContext db;
        private readonly SignInManager<ApplicationUsers> signInManager;

        public AccountController(IAuthService UserService, 
            IMapper Mapper, 
            IUnitOfWork uow, 
            IJwtGenerator jwtGenerator,
            UserManager<ApplicationUsers> AppUser,
            SignInManager<ApplicationUsers> signInManager,
            IHostingEnvironment hostingEnvironment,
            DataContext db)
        {
            userService = UserService;
            mapper = Mapper;
            UoW = uow;
            appUser = AppUser;
            this.hostingEnvironment = hostingEnvironment;
            this.db = db;
            this.jwtGenerator = jwtGenerator;
            this.signInManager = signInManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl)
        {
            LoginViewModel model = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult ExternalLogin(string returnUrl, string provider)
        {
            var redirectUrl = Url.Action("ExternalLoginCallBack", "Account",
                new { ReturnUrl = returnUrl });

            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallBack(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            LoginViewModel model = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");

                return View("Login", model);
            }

            var info = await signInManager.GetExternalLoginInfoAsync();

            if (info == null)
            {
                ModelState.AddModelError(string.Empty, "Error loading external login information.");

                return View("Login", model);
            }

            var signInResult = await signInManager.ExternalLoginSignInAsync(info.LoginProvider,
                info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if (signInResult.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);



                if (email != null)
                {

                    var user = await appUser.FindByEmailAsync(email);
                    //var currentUser = mapper.Map<ApplicationUsers>(user);

                    if (user == null)
                    {
                        user = new ApplicationUsers
                        {
                            FirstName = info.Principal.FindFirstValue(ClaimTypes.Name),
                            SecondName = info.Principal.FindFirstValue(ClaimTypes.Surname),
                            Country = info.Principal.FindFirstValue(ClaimTypes.Country),
                            UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                            Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                            Image = info.Principal.FindFirstValue("image"),
                        };

                        await appUser.CreateAsync(user);
                    }

                    await appUser.AddLoginAsync(user, info);
                    await signInManager.SignInAsync(user, isPersistent: false);

                    return LocalRedirect(returnUrl);
                }

                ViewBag.ErrorTitle = $"Email claim not received from: {info.LoginProvider}";
                ViewBag.ErrorMessage = "Please contact me noobbogdan@gmail.com";
            }

            return View("Error");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromForm] LoginViewModel loginViewModel)
        {

            if (loginViewModel.Email == null || loginViewModel.Password == null)
            {
                ModelState.AddModelError("", "These fields must be filled");
            }

            if (!ModelState.IsValid)
            {
                return View(loginViewModel);
            }

            var user = new UserDTO()
            {
                Email = loginViewModel.Email,
                Password = loginViewModel.Password,
                RememberMe = loginViewModel.RememberMe
            };

            var res = await userService.Login(user);

            if (res.Token != null)
            {
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Incorrect data");

            return View(loginViewModel);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (!new EmailAddressAttribute().IsValid(registerViewModel.Email))
            {
                ModelState.AddModelError("", "Incorrect email");
            }

            if (!string.IsNullOrEmpty(appUser.Options.User.AllowedUserNameCharacters))
            {
                ModelState.AddModelError("", "Invalid username");
            }

            if (registerViewModel.ConfirmPassword != registerViewModel.Password)
            {
                ModelState.AddModelError("", "Passwords must be the same");
            }

            if (!ModelState.IsValid)
            {
                return View(registerViewModel);
            }              

            string uniqueFileName = null;

            if (registerViewModel.Image != null)
            {
                string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + registerViewModel.Image.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                registerViewModel.Image.CopyTo(new FileStream(filePath, FileMode.Create));
            }

            var userDTO = new UserDTO()
            {
                FirstName = registerViewModel.FirstName,
                SecondName = registerViewModel.SecondName,
                Gender = registerViewModel.Gender,
                Country = registerViewModel.Country,
                Education = registerViewModel.Education,
                DateOfBirth = registerViewModel.DayOfBirth,
                Email = registerViewModel.Email,
                Password = registerViewModel.Password,
                Image = uniqueFileName,
                NickName = registerViewModel.NickName,
                Role = "User"
            };

            var res = await userService.AddUserAsync(userDTO, registerViewModel.RememberMe);

            if (res.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(registerViewModel);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> MyAccount()
        {
            var user = await userService.GetUserAsync(this.User.Identity.Name);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"Student with such Id cannot be found";
                return View("");
            }

            var model = new UserViewModel()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                SecondName = user.SecondName,
                Email = user.Email,
                Country = user.Country,
                Education = user.Education,
                DayOfBirth = user.DateOfBirth,
                Image = user.Image,
                NickName = user.NickName
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> MyAccount(UserViewModel model)
        {
            if (User.IsInRole("User"))
            {
                var userDto = new UserDTO()
                {
                    Id = await userService.GetUserByIdAsync(this.User),
                    FirstName = model.FirstName,
                    SecondName = model.SecondName,
                    Gender = model.Gender,
                    Email = model.Email,
                    Education = model.Education,
                    Country = model.Country,
                    DateOfBirth = model.DayOfBirth,
                    Role = "User",
                    Image = model.Image,
                    NickName = model.NickName
                };

                var updateUser = await userService.UpdateUserAsync(userDto);

                if (updateUser)
                {
                    return RedirectToAction("Index","Home");
                }
            }

            ModelState.AddModelError("", "Something goes wrong while updating");

            return View(model);
        }
        /*public async FileContentResult UserPhoto()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await userService.GetUserAsync(User.Identity.Name);

                int? userId = user.Id;

                if (userId == null)
                {
                    byte[] imageData = null;

                    return File(imageData, "images/user");
                }

                var userImage = user.Image;

                return new FileContentResult(userImage, "image/jpeg");
            }
        }*/

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await userService.SignOut();

            return RedirectToAction("Index", "Home");
        }
    }
}
