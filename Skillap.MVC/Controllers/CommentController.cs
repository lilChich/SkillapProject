using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Skillap.BLL.Interfaces.IServices;
using Skillap.DAL.EF;
using Skillap.DAL.Entities;
using Skillap.DAL.Interfaces;
using Skillap.MVC.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skillap.MVC.Controllers
{
    public class CommentController : Controller
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork UoW;
        private readonly DataContext db;
        private readonly IAuthService userService;

        public CommentController(IMapper Mapper,
            IUnitOfWork uow,
            DataContext db,
            IAuthService UserService)
        {
            mapper = Mapper;
            UoW = uow;
            this.db = db;
            userService = UserService;
        }
        
        [HttpGet]
        public async Task<IActionResult> ViewPost(int id)
        {
            var user = await userService.GetUserAsync(this.User.Identity.Name);
            var post = await db.Posts.FindAsync(id);
            var likedPost = await UoW.LikedPosts.FindAsync(x => x.PostId == post.Id);
            //var likedPost = db.LikedPosts.Where(x => x.PostId == post.Id).ToList();
            post.PostsLiked = new List<Liked_Posts>(likedPost);

            var comments = await UoW.Comments.FindAsync(c => c.PostId == post.Id);
            //var comments = db.Comments.Where(c => c.PostId == post.Id).ToList();
            post.Comments = new List<Comments>(comments);
            

            return View(post);
        }

        [HttpPost]
        public async Task<IActionResult> ViewPost(CommentViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (vm.Message == null)
            {
                return View();
            }

            var post = await UoW.Posts.GetByIdAsync(vm.Id);

            if (vm.Id > 0)
            {
                post.Comments = post.Comments ?? new List<Comments>();

                post.Comments.Add(new Comments
                {
                    Content = vm.Message,
                    CreatedTime = DateTime.Now
                });

                await UoW.Posts.UpdateAsync(post);
            }

            var user = userService.GetUserAsync(this.User.Identity.Name);
            var allCommentsOnPost = db.Comments.Where(c => c.PostId == post.Id).ToList();
            post.Comments = new List<Comments>(allCommentsOnPost);
            var likedPost = db.LikedPosts.Where(x => x.PostId == post.Id).ToList();
            post.PostsLiked = new List<Liked_Posts>(likedPost);

            return View(post);
        }

        [Authorize]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await UoW.Comments.GetByIdAsync(id);

            if (comment == null)
            {
                ViewBag.ErrorMassage = $"Comment with Id = {id} cannot be found";
                return View("NotFound");
            }
            else
            {
                await UoW.Comments.DeleteAsync(comment.Id);

                return RedirectToAction("ManagePosts");
            }

        }
    }
}
