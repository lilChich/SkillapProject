using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Skillap.BLL.DTO;
using Skillap.BLL.Interfaces.IServices;
using Skillap.BLL.Models;
using Skillap.DAL.EF;
using Skillap.DAL.Entities;
using Skillap.DAL.Interfaces;
using Skillap.MVC.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Skillap.MVC.Controllers
{
    public class LogicController : Controller
    {

        private readonly IMasterService userService;
        private readonly IAuthService secondUserService;
        private readonly IMapper mapper;
        private readonly IUnitOfWork UoW;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly DataContext db;
        private readonly UserManager<ApplicationUsers> appUser;

        public LogicController(IMasterService UserService,
            UserManager<ApplicationUsers> appUser,
            IAuthService secondUserService,
            IMapper Mapper,
            IUnitOfWork uow,
            DataContext db,
            IHostingEnvironment hostingEnvironment)
        {
            userService = UserService;
            mapper = Mapper;
            UoW = uow;
            this.hostingEnvironment = hostingEnvironment;
            this.db = db;
            this.secondUserService = secondUserService;
            this.appUser = appUser;

        }

        //[Authorize]
        [HttpGet, Route("ManageMasterClasses")]
        public async Task<IActionResult> ManageMasterClasses()
        {
            if (!this.User.Identity.IsAuthenticated || !this.User.IsInRole("Admin"))
            {
                return Unauthorized();
            }

            var masterClasses = await UoW.MasterClasses.FindAsync(x => true);
            return Ok(masterClasses);
        }

        //[Authorize]
        [HttpGet, Route("ManageUsers")]
        public async Task<IActionResult> ManageUsers()
        {
            if (!this.User.Identity.IsAuthenticated || !this.User.IsInRole("Admin"))
            {
                return Unauthorized();
            }

            var users = await UoW.ApplicationUsers.FindAsync(x => true);
            //var users = secondUserService.GetAllUsersAsync();
            return Ok(users);
        }

        //[Authorize]
        [HttpGet, Route("ManageMasters")]
        public async Task<IActionResult> ManageMasters()
        {
            if (!this.User.Identity.IsAuthenticated || !this.User.IsInRole("Admin"))
            {
                return Unauthorized();
            }

            var masters = await UoW.Masters.FindAsync(x => true);
            return Ok(masters);
        }

        //[Authorize]
        [HttpGet, Route("ManageTags")]
        public async Task<IActionResult> ManageTags()
        {
            if (!this.User.Identity.IsAuthenticated || !this.User.IsInRole("Admin"))
            {
                return Unauthorized();
            }

            var tags = secondUserService.GetAllTags();
            //var tags = await UoW.Tags.FindAsync(x => true);
            return Ok(tags);
        }

        //[Authorize]
        [HttpGet, Route("ManagePosts")]
        public async Task<IActionResult> ManagePosts(string name, int page = 1, SortType sort = SortType.NameAsc)
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            //var posts = await UoW.Posts.FindAsync(x => true);
            var viewModel = await secondUserService.LoadPostsAsync(name, page, sort, 3);
            var posts = secondUserService.GetAllPosts();
            return Ok(posts);
            //return Unauthorized();
        }

        [HttpDelete, Route("DeleteUser")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (!this.User.Identity.IsAuthenticated || !this.User.IsInRole("Admin"))
            {
                return Unauthorized();
            }
            else
            {
                var user = await secondUserService.GetUserByIdAsync(id);

                if (user == null)
                {
                    ViewBag.ErrorMassage = $"User with Id = {id} cannot be found";
                    return View("NotFound");
                }
                else
                {
                    var res = await secondUserService.DeleteUserAsync(user);
                    //var users = secondUserService.GetAllUsersAsync();

                    if (res.Succeeded)
                    {
                        return Ok(res);
                        //return RedirectToAction("ManageUsers", users);
                    }

                    return BadRequest("Can't delete user");
                    //return View("ManageUsers", users);
                }
            }
           
        }

        [HttpGet, Route("EditUsers")]
        public async Task<IActionResult> EditUsers(int id)
        {
            if (!this.User.Identity.IsAuthenticated || !this.User.IsInRole("Admin"))
            {
                return Unauthorized();
            }

            var user = await secondUserService.GetUserByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMassage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }

            var userClaims = await appUser.GetClaimsAsync(user);
            var userRoles = await appUser.GetRolesAsync(user);

            var model = new EditUserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                SecondName = user.SecondName,
                Gender = user.Gender,
                Country = user.Country,
                Education = user.Education,
                DayOfBirth = user.DateOfBirth,
                Email = user.Email,
                ExistingPhotoPath = user.Image,
                NickName = user.NickName,
                Claims = userClaims.Select(c => c.Value).ToList(),
                Roles = userRoles

            };

            return Ok(model);
            //return View(model);
        }

        [HttpPost, Route("EditUsers")]
        public async Task<IActionResult> EditUsers(EditUserViewModel model)
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            if (ModelState.IsValid)
            {
                var user = await secondUserService.GetUserByIdAsync(model.Id);
                user.FirstName = model.FirstName;
                user.SecondName = model.SecondName;
                user.NickName = model.NickName;
                user.Education = model.Education;
                user.Country = model.Country;
                user.DateOfBirth = model.DayOfBirth;
                user.Gender = model.Gender;
                user.Email = model.Email;

                string uniqueFileName = null;

                if (model.Image != null)
                {
                    string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "images");
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Image.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        model.Image.CopyTo(fileStream);
                    }


                    if (model.ExistingPhotoPath != null)
                    {
                        string filePathToDelete = Path.Combine(hostingEnvironment.WebRootPath, "images", model.ExistingPhotoPath);
                        System.IO.File.Delete(filePathToDelete);
                    }

                    user.Image = uniqueFileName;
                }

                await UoW.ApplicationUsers.UpdateAsync(user);

                return Ok(user);
                //return RedirectToAction("ManageUsers", "Logic");
                
            }

            ModelState.AddModelError("", "Something goes wrong while updating");

            return BadRequest("Something went wrong");
            //return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditMasterClasses(int id)
        {
            var masterClass = await userService.GetMasterClassById(id);

            if (masterClass == null)
            {
                ViewBag.ErrorMassage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }


            var model = new EditMasterClassViewModel
            {
                Id = masterClass.Id,
                Name = masterClass.Name,
                Description = masterClass.Description,
                Category = masterClass.Category,
                Level = masterClass.Level,
                Relevance = masterClass.Relevance

            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditMaster(int id)
        {
            var master = await UoW.Masters.GetByIdAsync(id);

            if (master == null)
            {
                ViewBag.ErrorMassage = $"Master with Id = {id} cannot be found";
                return View("NotFound");
            }


            var model = new EditMasterViewModel
            {
                UserId = master.ApplicationUserId,
                MasterClassId = master.MasterClassId,
                Status = master.Status,
                SkillLevel = master.SkillLevel


            };

            return View(model);
        }

        [HttpPost, Route("EditMaster")]
        public async Task<IActionResult> EditMaster(EditMasterViewModel model)
        {
            if (!this.User.Identity.IsAuthenticated || !this.User.IsInRole("Admin"))
            {
                return Unauthorized();
            }

            var master = await userService.GetMasterByIdAsync(model.Id);

            if (master == null)
            {
                ViewBag.ErrorMassage = $"Master with Id = {model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                /*var curUser = new MapperConfiguration(cfg => cfg.CreateMap<EditUserViewModel, UserDTO>()).CreateMapper();
                var mapUser = curUser.Map<UserDTO>(editUserViewModel);
                var res = secondUserService.UpdateUserAsync(mapUser);

                if (res.IsCompleted)
                {
                    return RedirectToAction("ManageUsers");
                }*/

                master.ApplicationUserId = model.SelectedUser;
                master.MasterClassId = model.SelectedMasterClass;
                master.SkillLevel = model.SkillLevel;
                master.Status = model.Status;

                //var result = await appUser.UpdateAsync(user);

                var res = db.Set<Masters>().Update(master);

                if (res != null)
                {
                    await res.Context.SaveChangesAsync();
                    return Ok(model);
                    //return RedirectToAction("ManageMasters");
                }

                return BadRequest();
                //return View(model);
            }
        }

        [HttpPost, Route("EditMasterClasses")]
        public async Task<IActionResult> EditMasterClasses(EditMasterClassViewModel model)
        {
            if (!this.User.Identity.IsAuthenticated || !this.User.IsInRole("Admin"))
            {
                return Unauthorized();
            }

            var masterClass = await userService.GetMasterClassById(model.Id);
            var currentMasterClass = mapper.Map<MasterClasses>(masterClass);


            if (currentMasterClass == null)
            {
                ViewBag.ErrorMassage = $"MasterClass with Id = {model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                /*var curUser = new MapperConfiguration(cfg => cfg.CreateMap<EditUserViewModel, UserDTO>()).CreateMapper();
                var mapUser = curUser.Map<UserDTO>(editUserViewModel);
                var res = secondUserService.UpdateUserAsync(mapUser);

                if (res.IsCompleted)
                {
                    return RedirectToAction("ManageUsers");
                }*/

                currentMasterClass.Name = model.Name;
                currentMasterClass.Description = model.Description;
                currentMasterClass.Category = model.Category;
                currentMasterClass.Level = model.Level;
                currentMasterClass.Relevance = model.Relevance;

                //var result = await appUser.UpdateAsync(user);

                var res = db.Set<MasterClasses>().Update(currentMasterClass);

                if (res != null)
                {
                    await res.Context.SaveChangesAsync();
                    return Ok(model);
                }
/*                try
                {
                    await UoW.MasterClasses.UpdateAsync(currentMasterClass);
                }
                catch
                {
                    throw new Exception();
                }*/
                

                /*if (res != null)
                {
                    await res.Context.SaveChangesAsync();
                    return RedirectToAction("ManageMasterClasses");
                }*/

                return Ok(currentMasterClass);
                //return View(model);
            }
        }

        [HttpDelete, Route("DeleteMasterClass")]
        public async Task<IActionResult> DeleteMasterClass(int id)
        {
            if (!this.User.Identity.IsAuthenticated || !this.User.IsInRole("Admin"))
            {
                return Unauthorized();
            }

            var masterClass = await userService.GetMasterClassById(id);

            if (masterClass == null)
            {
                ViewBag.ErrorMassage = $"MasterClass with Id = {id} cannot be found";
                return View("NotFound");
            }
            else
            {
                var res = await userService.DeleteMasterClass(masterClass);

                if (!res)
                {
                    return RedirectToAction("ManageMasterClasses");
                }
                //var masterClasses = userService.GetAllMasterClassesAsync();
                return Ok();
                //return View("ManageMasterClasses", masterClasses);
            }
        }

        [HttpDelete, Route("DeleteMaster")]
        public async Task<IActionResult> DeleteMaster(int id)
        {
            var master = await userService.GetMasterByIdAsync(id);

            if (master == null)
            {
                ViewBag.ErrorMassage = $"Master with Id = {id} cannot be found";
                return View("NotFound");
            }
            else
            {
                await UoW.Masters.DeleteAsync(master.Id);
                await UoW.Masters.SaveAsync();

                return Ok();
                //return RedirectToAction("ManageMasters");
            }

        }

        [Authorize]
        [HttpGet]
        public ActionResult Masters()
        {
            var selectedListUsers = new List<SelectListItem>();
            var allUsers = UoW.ApplicationUsers.GetAll();

            foreach (var user in allUsers)
            {
                selectedListUsers.Add(new SelectListItem(user.NickName, user.Id.ToString()));
            }
            //var  = new MasterClasses;
            ViewBag.Users = selectedListUsers;
            ViewBag.UsersSelect = new SelectList(db.Users, "Id", "NickName");
            var selectedListMasterClasses = new List<SelectListItem>();

            //int selectedLevel = 1;

            //var allMasterClasses = db.MasterClasses.Where(m => m.Level == selectedLevel).ToList();
            var allMasterClasses = UoW.MasterClasses.GetAll();

            foreach (var masterClass in allMasterClasses)
            {
                selectedListMasterClasses.Add(new SelectListItem(masterClass.Name, masterClass.Id.ToString()));
            }

            ViewBag.MasterClasses = selectedListMasterClasses;
            ViewBag.MasterClassesSelect = new SelectList(db.MasterClasses, "Id", "Name");

            var model = new MastersViewModel()
            {
                Users = selectedListUsers,
                MasterClasses = selectedListMasterClasses,

            };

            return View(model);
        }

        public ActionResult GetMasterClasses([FromForm]int level)
        {
            var listOfMasterClasses = db.MasterClasses.Where(m => m.Level < level - 1).ToList();
            var sortedList = listOfMasterClasses.OrderBy(m => m.Level).ToList();
            return PartialView(sortedList);
        }

        //[Authorize]
        [HttpPost, Route("Masters")]
        public async Task<ActionResult> Masters(MastersViewModel modelMasters)
        {
            var user = db.Users.Where(u => u.Id == modelMasters.SelectedUser).FirstOrDefault();
            var masterClass = db.MasterClasses.Where(m => m.Id == modelMasters.SelectedMasterClass).FirstOrDefault();

            if (!ModelState.IsValid)
            {
                return View(modelMasters);
            }

            try
            {
                var masterDto = new Masters()
                {
                    ApplicationUserId = user.Id,
                    MasterClassId = masterClass.Id,
                    Status = modelMasters.Status,
                    SkillLevel = modelMasters.SkillLevel
                };

                await UoW.Masters.CreateAsync(masterDto);
                return Ok(masterDto);
            }
            catch
            {
                return View();
            }



            //var masters = userService.GetAllMastersAsync();
            
            //return View("ManageMasters", masters);
        }

        [Authorize]
        [HttpGet]
        public ActionResult MasterClasses()
        {
            return View();
        }

        //[Authorize]
        [HttpPost, Route("MasterClasses")]
        public async Task<ActionResult> MasterClasses(MasterClassViewModel model)
        {
            if (!this.User.Identity.IsAuthenticated || !this.User.IsInRole("Admin"))
            {
                return Unauthorized();
            }

            if (model.Name == null || model.Description == null || model.Category == null)
            {
                ModelState.AddModelError("", "These fields must be filled");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var res = userService.GetMasterClassByNameAndDescription(model.Name, model.Description);

            if (res != null)
            {
                ModelState.AddModelError("", "We already have such name + description for MasterClass");
                return View(model);
            }

            var masterClass = new MasterClassesDTO()
            {

                Name = model.Name,
                Description = model.Description,
                Category = model.Category,
                Relevance = model.Relevance,
                Level = model.Level

            };

            try
            {
                await userService.CreateMasterClass(masterClass);
            }
            catch
            {
                throw new Exception();
            }

            //var allMasterClasses = userService.GetAllMasterClassesAsync();
            return Ok(masterClass);
            //return View("ManageMasterClasses", allMasterClasses);
        }

        [Authorize]
        [HttpGet]
        public ActionResult Tag()
        {
            return View();
        }

        [HttpPost, Route("Tag")]
        public async Task<ActionResult> Tag(TagViewModel model)
        {
            if (!this.User.Identity.IsAuthenticated || !this.User.IsInRole("Admin"))
            {
                return Unauthorized();
            }

            if (model.Name == null)
            {
                ModelState.AddModelError("", "These fields must be filled");
            }

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Enter correct name");
                return View(model);
            }

            var res = secondUserService.GetTagByName(model.Name);

            if (res != null)
            {
                ModelState.AddModelError("", "We already have such name for tag");
                return View(model);
            }

            var tag = new TagsDTO()
            {
                Name = model.Name
            };

            try
            {
                await secondUserService.CreateTagAsync(tag);
            }
            catch
            {
                throw new Exception("Something went wrong");
            }
            //var allTags = secondUserService.GetAllTags();
            return Ok(tag);
            //return View("ManageTags", allTags);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditTag(int id)
        {
            var tag = await UoW.Tags.GetByIdAsync(id);

            if (tag == null)
            {
                ViewBag.ErrorMassage = $"Tag with Id = {id} cannot be found";
                return View("NotFound");
            }


            var model = new EditTagViewModel
            {
                Name = tag.Name

            };

            return View(model);
        }

        [HttpPost, Route("EditTag")]
        public async Task<IActionResult> EditTag(EditTagViewModel model)
        {
            if (!this.User.Identity.IsAuthenticated || !this.User.IsInRole("Admin"))
            {
                return Unauthorized();
            }

            var tag = await UoW.Tags.GetByIdAsync(model.Id);

            if (tag == null)
            {
                ViewBag.ErrorMassage = $"Tag with Id = {model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                tag.Name = model.Name;

                //var res = db.Set<Tags>().Update(tag);

                try
                {
                    await UoW.Tags.UpdateAsync(tag);
                    return Ok(model);
                }
                catch
                {
                    throw new Exception();
                }
                
/*                if (res != null)
                {
                    await res.Context.SaveChangesAsync();
                    return RedirectToAction("ManageTags");
                }*/

                //return View(model);
            }
        }

        [HttpDelete, Route("DeleteTag")]
        public async Task<IActionResult> DeleteTag(int id)
        {
            if (!this.User.Identity.IsAuthenticated || !this.User.IsInRole("Admin"))
            {
                return Unauthorized();
            }

            var tag = await UoW.Tags.GetByIdAsync(id);

            if (tag == null)
            {
                ViewBag.ErrorMassage = $"Master with Id = {id} cannot be found";
                return View("NotFound");
            }
            else
            {
                await UoW.Tags.DeleteAsync(tag.Id);

                return Ok();
                //return RedirectToAction("ManageTags");
            }

        }

        [Authorize]
        [HttpGet]
        public ActionResult Post()
        {
            return View();
        }

        //[Authorize]
        [HttpPost, Route("Post")]
        public async Task<ActionResult> Post(PostViewModel model)
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            if (model.Name == null || model.Description == null)
            {
                ModelState.AddModelError("", "These fields must be filled");
            }

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Enter correct name or description");
                return View(model);
            }

            var res = secondUserService.GetPostByNameAndDescription(model.Name, model.Description);

            if (res != null)
            {
                ModelState.AddModelError("", "We already have such Post");
                return View(model);
            }

            var post = new Posts()
            {
                Name = model.Name,
                Description = model.Description,
                CreatedTime = DateTime.UtcNow,
                Status = model.Status
            };

            await UoW.Posts.CreateAsync(post);

            if (ModelState.IsValid)
            {
                string pattern = @"\B(\#[a-zA-Z0-9]+\b)(?!;)";

                foreach (Match match in Regex.Matches(model.Tags, pattern))
                {
                    try
                    {
                        var tagToFind = db.Tags.Where(x => x.Name == match.Value).FirstOrDefault();

                        if (tagToFind == null)
                        {
                            var tag = new Tags()
                            {
                                Name = match.Value
                            };

                            await UoW.Tags.CreateAsync(tag);
                            var thisTag = db.Tags.Where(x => x.Name == tag.Name).FirstOrDefault();

                            var post_Tags = new Post_Tags()
                            {
                                PostId = post.Id,
                                TagId = thisTag.Id
                            };

                            await UoW.PostsTags.CreateAsync(post_Tags);
                        }
                        else
                        {
                            var post_Tags = new Post_Tags()
                            {
                                PostId = post.Id,
                                TagId = tagToFind.Id
                            };

                            await UoW.PostsTags.CreateAsync(post_Tags);
                        }
                    }
                    catch
                    {
                        throw new Exception();
                    }
                   
                }
            }

            //await secondUserService.CreatePostAsync(post);

            var user = await secondUserService.GetUserAsync(this.User.Identity.Name);
            var findPost = db.Posts.Where(p => p.Name == post.Name && p.Description == post.Description).FirstOrDefault();

            var likedPost = new Liked_PostsDTO()
            {
                PostId = findPost.Id,
                UserId = user.Id,
                Like = null,
                Score = 0,
                isCreator = true
            };

            var mappedLikedPost = mapper.Map<Liked_Posts>(likedPost);

            try
            {
                await UoW.LikedPosts.CreateAsync(mappedLikedPost);
                //await secondUserService.CreateLikedPostAsync(likedPost);
            }
            catch
            {
                throw new Exception("Something went wrong");
            }

            return Ok(post);
            //var allPosts = secondUserService.GetAllPosts();
            //return View("ManagePosts", allPosts);
        }

        /*[Authorize]
        public async Task<IActionResult> Like(int id)
        {
            var user = await secondUserService.GetUserAsync(this.User.Identity.Name);
            var post = await secondUserService.GetPostById(id);
            var postToLike = db.LikedPosts.Where(l => l.PostId == id).FirstOrDefault();

                try
                {
                    await UoW.LikedPosts.CreateAsync(mappedLikedPost);

                    if (mappedLikedPost.Like == null)
                    {
                        var like = mappedLikedPost.Like = true;

                        if (like == true)
                        {
                            mappedLikedPost.Score += 1;
                            await UoW.LikedPosts.UpdateAsync(mappedLikedPost);
                        }
                    }
                }
                catch
                {
                    throw new Exception("Something went wrong");
                }
            }
            else
            {
                try
                {
                    if (postToLike.Like == null)
                    {
                        var like = postToLike.Like = true;

                    if (like == true)
                    {
                        postToLike.Score += 1;
                        await UoW.LikedPosts.UpdateAsync(postToLike);
                    }
                }
                else if (postToLike.Like == true)
                {
                    var dislike = postToLike.Like = false;

                    if (dislike == false)
                    {
                        postToLike.Score -= 1;
                        await UoW.LikedPosts.UpdateAsync(postToLike);
                    }
                }
                else
                {
                    var like = postToLike.Like = true;

                    if (like == true)
                    {
                        postToLike.Score += 1;
                        await UoW.LikedPosts.UpdateAsync(postToLike);
                    }
                }
            }
            catch
            {
                throw new Exception("Cannot like post");
            }

            return RedirectToAction("ViewPost","Comment", new { id = id });
        }

        [Authorize]
        public async Task<IActionResult> Dislike(int id)
        {
            // var post = secondUserService.GetPostById(id);
            var likedPost = db.LikedPosts.Where(l => l.PostId == id).FirstOrDefault();

            try
            {
                var like = likedPost.Like = false;

                    if (mappedDislikedPost.Like == null)
                    {
                        var dislike = mappedDislikedPost.Like = false;

                        if (dislike == false)
                        {
                            mappedDislikedPost.Score -= 1;
                            await UoW.LikedPosts.UpdateAsync(mappedDislikedPost);
                        }
                    }
                }
                catch
                {
                    throw new Exception("Something went wrong");
                }
            }
            else
            {
                try
                {
                    if (postToDislike.Like == null)
                    {
                        var dislike = postToDislike.Like = false;

                    await UoW.LikedPosts.UpdateAsync(likedPost);
                }
            }
            catch
            {
                throw new Exception("Cannot dislike post");
            }

            return RedirectToAction("F", "Comment", new { id = id });
        }*/

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditPost(int id)
        {
            var post = await UoW.Posts.GetByIdAsync(id);

            if (post == null)
            {
                ViewBag.ErrorMassage = $"Post with Id = {id} cannot be found";
                return View("NotFound");
            }

            var model = new EditPostViewModel
            {
                Name = post.Name,
                Description = post.Description,
                Status = post.Status

            };

            return View(model);
        }

        /*[Authorize]
        [HttpPost]
        public async Task<IActionResult> EditPost(EditPostViewModel model)
        {
            var post = await UoW.Posts.GetByIdAsync(model.Id);


            if (post == null)
            {
                ViewBag.ErrorMassage = $"Post with Id = {model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                post.Name = model.Name;
                post.Description = model.Description;
                post.Status = model.Status;

                var res = db.Set<Posts>().Update(post);

                if (res != null)
                {
                    await res.Context.SaveChangesAsync();
                    return RedirectToAction("ManagePosts");
                }

                return View(model);
            }
        }*/

        

        [Authorize]
        [HttpGet]
        public IActionResult GreedyAlgorithmForMasters()
        {
            var masters = UoW.Masters.GetAll().OrderByDescending(i => i.SkillLevel).ToList();
            return View(masters);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> GreedyAlgorithmForMastersPost(int id)
        {
            var masters = db.Masters.OrderByDescending(i => i.SkillLevel).ToList();
            var masterClasses = db.MasterClasses.OrderByDescending(i => i.Level).ToList();
            
            foreach (var mc in masterClasses)
            {
                var currentMasters = masters.Where(x => x.MasterClassId == mc.Id);
                bool isChosen = false;

                foreach (var m in currentMasters)
                {
                    if (m.SkillLevel >= mc.Level + 1)
                    {
                        if (isChosen == true)
                        {
                            goto marked;
                        }

                        if (m.Status == false)
                        {
                            m.Status = true;

                            var res = db.Set<Masters>().Update(m);

                            if (res != null)
                            {
                                await res.Context.SaveChangesAsync();
                                isChosen = true;
                                continue;
                            }
                        }
                        else
                        {
                            isChosen = true;
                            continue;
                        }
                        marked:
                            if (m.Status == true)
                            {
                                m.Status = false;

                                var res = db.Set<Masters>().Update(m);

                                if (res != null)
                                {
                                    await res.Context.SaveChangesAsync();
                                    continue;
                                }
                            }
                            else
                            {
                                continue;
                            }
                    }
                }
            }

            return View("GreedyAlgorithmForMasters", masters);
        }
    }
}
