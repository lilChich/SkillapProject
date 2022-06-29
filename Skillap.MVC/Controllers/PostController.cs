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
//using System.Data.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Skillap.BLL.DTO;
using Microsoft.AspNetCore.SignalR;
using Skillap.BLL.Hubs;

namespace Skillap.MVC.Controllers
{
    public class PostController : Controller
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork UoW;
        private readonly DataContext db;
        private readonly IAuthService userService;
        private readonly UserManager<ApplicationUsers> userManager;
        private readonly IHubContext<PostChatHub> postChatHub;

        public PostController(IMapper Mapper,
            IUnitOfWork uow,
            DataContext db,
            IAuthService UserService,
            UserManager<ApplicationUsers> userManager,
            IHubContext<PostChatHub> postChatHub)
        {
            mapper = Mapper;
            UoW = uow;
            this.db = db;
            userService = UserService;
            this.userManager = userManager;
            this.postChatHub = postChatHub;
        }
        
        [HttpGet, Route("ViewPost")]
        [AllowAnonymous]
        public async Task<IActionResult> ViewPost(int id)
        {
            var user = await userService.GetUserAsync(this.User.Identity.Name);
            var post = await db.Posts.FindAsync(id);
            var likedPost = await UoW.LikedPosts.FindAsync(x => x.PostId == post.Id);
            
            //var likedPost = await db.LikedPosts.Where(x => x.PostId == post.Id).ToListAsync();
            post.PostsLiked = new List<Liked_Posts>(likedPost);

            var postTags = await UoW.PostsTags.FindAsync(x => x.PostId == post.Id);
            post.PostTags = new List<Post_Tags>(postTags);


            //var comments = await db.Comments.Where(c => c.PostId == post.Id).ToListAsync();
            var comments = await UoW.Comments.FindAsync(c => c.PostId == post.Id);

            foreach (var c in comments)
            {
                var likedComments = await UoW.LikedComments.FindAsync(x => x.CommentId == c.Id);

                c.CommentsLiked = new List<Liked_Comments>(likedComments);
            }

            //var comments = db.Comments.Where(c => c.PostId == post.Id).ToList();
            post.Comments = new List<Comments>(comments);

            //return Ok(post);
            return View(post);
        }

        [HttpPost, Route("ViewPost")]
        public async Task<IActionResult> ViewPost(CommentViewModel vm)
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            if (vm.Message == null)
            {
                return View();
            }

            var post = await UoW.Posts.GetByIdAsync(vm.PostId);

            if (post == null)
            {
                return BadRequest();
            }

            var comments = (await UoW.Comments.FindAsync(x => x.PostId == post.Id)).ToList();
            post.Comments = comments;

            post.Comments = post.Comments ?? new List<Comments>();

            var comment = new Comments()
            {
                Content = vm.Message,
                CreatedTime = DateTime.Now,
                PostId = post.Id
            };

            await UoW.Comments.CreateAsync(comment);

            var user = await userService.GetUserAsync(this.User.Identity.Name);
            var findComment = await db.Comments.Where(x => x.PostId == post.Id && x.Content == comment.Content && x.CreatedTime == comment.CreatedTime).FirstOrDefaultAsync();

            var likedComment = new Liked_CommentsDTO()
            {
                UserId = user.Id,
                CommentId = findComment.Id,
                Score = 0,
                Like = null,
                isCreator = true
            };

            var mappedLikedComment = mapper.Map<Liked_Comments>(likedComment);

            try
            {              
                await UoW.LikedComments.CreateAsync(mappedLikedComment);
            }
            catch
            {
                throw new Exception();
            }
           
            //var allCommentsOnPost = db.Comments.Where(c => c.PostId == post.Id).ToList();
            var allCommentsOnPost = await UoW.Comments.FindAsync(c => c.PostId == post.Id);
            post.Comments = new List<Comments>(allCommentsOnPost);
            //var likedPost = db.LikedPosts.Where(x => x.PostId == post.Id).ToList();
            var likedPost = await UoW.LikedPosts.FindAsync(x => x.PostId == post.Id);
            post.PostsLiked = new List<Liked_Posts>(likedPost);
            var postTags = await UoW.PostsTags.FindAsync(x => x.PostId == post.Id);
            post.PostTags = new List<Post_Tags>(postTags);
            await postChatHub.Clients.All.SendAsync("LoadComments");

            return View(post);
            //return Ok(post);
        }

        [HttpDelete, Route("DeletePost")]
        public async Task<IActionResult> DeletePost(int id)
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            var user = await userManager.FindByEmailAsync(this.User.Identity.Name);
            var post = await UoW.Posts.GetByIdAsync(id);

            if (post == null)
            {
                ViewBag.ErrorMassage = $"Post with Id = {id} cannot be found";
                return BadRequest("Cannot find post with such id");
                //return View("NotFound");
            }

            try
            {
                /*var usersPost = await (from postOfTheUser in db.Posts
                                       join likedPost in db.LikedPosts
                                       on id equals likedPost.PostId
                                       join creator in db.Users
                                       on likedPost.UserId equals user.Id
                                       select postOfTheUser)
                                   .FirstOrDefaultAsync();*/

                var usersPost = await (from p in db.Posts
                                       join lp in db.LikedPosts
                                       on p.Id equals lp.PostId
                                       where p.Id == id
                                       join creator in db.Users
                                       on lp.UserId equals user.Id
                                       where creator.Id == user.Id
                                       where lp.isCreator == true
                                       select p).FirstOrDefaultAsync();

                if (!this.User.IsInRole("Admin") && usersPost == null)
                {
                    return BadRequest("Seems like you cannot do this via your claims or you're not a creator of the post");
                }
                else
                {
                    await UoW.Posts.DeleteAsync(post.Id);

                    return Ok();
                    //return RedirectToAction("ManagePosts");
                }
            }
            catch
            {
                throw new Exception();
            }
        }

        //[Authorize]
        [HttpDelete, Route("DeleteComment")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            var user = await userManager.FindByEmailAsync(this.User.Identity.Name);
            var comment = await UoW.Comments.GetByIdAsync(id);
            var post = await UoW.Posts.GetByIdAsync(comment.PostId);

            var usersComment = await (from c in db.Comments
                                      join lc in db.LikedComments
                                      on c.Id equals lc.CommentId
                                      where c.Id == id
                                      join creator in db.Users
                                      on lc.UserId equals user.Id
                                      where creator.Id == user.Id
                                      where lc.isCreator == true
                                      select c).FirstOrDefaultAsync();

            if (comment == null)
            {
                ViewBag.ErrorMassage = $"Comment with Id = {id} cannot be found";
                return View("NotFound");
            }
            
            if (!this.User.IsInRole("Admin") && usersComment != null)
            {
                return BadRequest();
                //return RedirectToAction("ManagePosts");
            }
            else
            {
                await UoW.Comments.DeleteAsync(comment.Id);
                await postChatHub.Clients.All.SendAsync("LoadComments");

                return Ok();
            }

            //return BadRequest();
        }

        //[Authorize]
        [HttpPost, Route("EditPost")]
        public async Task<IActionResult> EditPost(EditPostViewModel model)
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            var user = await userManager.FindByEmailAsync(this.User.Identity.Name);
            var post = await UoW.Posts.GetByIdAsync(model.Id);


            if (post == null)
            {
                ViewBag.ErrorMassage = $"Post with Id = {model.Id} cannot be found";
                return View("NotFound");
            }

            try
            {
                var usersPost = await (from postOfTheUser in db.Posts
                                       join likedPost in db.LikedPosts
                                       on postOfTheUser.Id equals likedPost.PostId
                                       where postOfTheUser.Id == post.Id
                                       join creator in db.Users
                                       on likedPost.UserId equals creator.Id
                                       where creator.Id == user.Id
                                       select postOfTheUser)
                                   .FirstOrDefaultAsync();

                if (!this.User.IsInRole("Admin") && usersPost == null)
                {
                    return BadRequest("Seems like you cannot do this via your claims or you're not a creator of the post");
                }
                else
                {
                    post.Name = model.Name;
                    post.Description = model.Description;
                    post.Status = model.Status;

                    string pattern = @"\B(\#[a-zA-Z0-9]+\b)(?!;)";

                    var allTagsOnPost = await (from t in db.Tags
                                               join postTags in db.PostsTags
                                               on t.Id equals postTags.TagId
                                               join p in db.Posts
                                               on postTags.PostId equals p.Id
                                               where post.Id == p.Id
                                               select t).ToListAsync();

                    foreach (Match match in Regex.Matches(model.Tags, pattern))
                    {
                        var tagToFind = await db.Tags.Where(x => x.Name == match.Value).FirstOrDefaultAsync();
                        bool flag = false;            

                        foreach (Tags tag in allTagsOnPost)
                        {
                            if (match.Value == tag.Name)
                            {
                                flag = true;
                                break;
                            }
                        }

                        if (flag == true)
                        {
                            continue;
                        }

                        if (tagToFind == null)
                        {
                            var tag = new Tags()
                            {
                                Name = match.Value
                            };

                            await UoW.Tags.CreateAsync(tag);

                            var thisTag = await db.Tags.Where(x => x.Name == tag.Name).FirstOrDefaultAsync();

                            var post_Tags = new Post_Tags()
                            {
                                PostId = post.Id,
                                TagId = thisTag.Id
                            };

                            await UoW.PostsTags.CreateAsync(post_Tags);
                            post.PostTags.Add(post_Tags);
                        }
                        else
                        {
                            var post_Tags = new Post_Tags()
                            {
                                PostId = post.Id,
                                TagId = tagToFind.Id
                            };

                            await UoW.PostsTags.CreateAsync(post_Tags);
                            post.PostTags.Add(post_Tags);
                        }   
                    }
                    await UoW.Posts.UpdateAsync(post);
                    await postChatHub.Clients.All.SendAsync("ViewPost");
                    //var res = await db.Set<Posts>().UpdateAsync(post);

                    return Ok(post);
                }
            }
            catch
            {
                throw new Exception();
            }

            //return View(model);
        }

        [HttpPost, Route("Like")]
        public async Task<IActionResult> Like(int id)
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            var user = await userService.GetUserAsync(this.User.Identity.Name);
            var post = await userService.GetPostById(id);

            var postToLike = await db.LikedPosts.Where(l => l.PostId == id && l.UserId == user.Id).FirstOrDefaultAsync();
            //var likedPost = new Liked_PostsDTO();


            if (postToLike == null)
            {
                var likedPost = new Liked_PostsDTO()
                {
                    PostId = post.Id,
                    UserId = user.Id,
                    Like = null,
                    Score = 0,
                    isCreator = false
                };

                var mappedLikedPost = mapper.Map<Liked_Posts>(likedPost);

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
                    else if (postToLike.Like == true && postToLike.Score == 1)
                    {
                        var dislike = postToLike.Like = false;

                        if (dislike == false)
                        {
                            postToLike.Score -= 1;
                            await UoW.LikedPosts.UpdateAsync(postToLike);
                        }
                    }
                    else if (postToLike.Like == false && postToLike.Score == 0)
                    {
                        var like = postToLike.Like = true;

                        if (like == true)
                        {
                            postToLike.Score += 1;
                            await UoW.LikedPosts.UpdateAsync(postToLike);
                        }
                    }
                    else if (postToLike.Like == true && postToLike.Score == 0)
                    {
                        var like = postToLike.Like = true;

                        if (like == true)
                        {
                            postToLike.Score += 1;
                            await UoW.LikedPosts.UpdateAsync(postToLike);
                        }
                    }
                    else if (postToLike.Like == false && postToLike.Score == -1)
                    {
                        var like = postToLike.Like = true;

                        if (like == true)
                        {
                            postToLike.Score += 2;
                            await UoW.LikedPosts.UpdateAsync(postToLike);
                        }
                    }
                }
                catch
                {
                    throw new Exception();
                }
            }
            return Ok(post);
            //return RedirectToAction("ViewPost", "Comment", new { id = id });
        }

        [HttpPost, Route("Dislike")]
        public async Task<IActionResult> Dislike(int id)
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            var user = await userService.GetUserAsync(this.User.Identity.Name);
            var post = await userService.GetPostById(id);

            var postToDislike = await db.LikedPosts.Where(l => l.PostId == id && l.UserId == user.Id).FirstOrDefaultAsync();

            if (postToDislike == null)
            {
                var dislikedPost = new Liked_PostsDTO()
                {
                    PostId = post.Id,
                    UserId = user.Id,
                    Like = null,
                    Score = 0,
                    isCreator = false
                };

                var mappedDislikedPost = mapper.Map<Liked_Posts>(dislikedPost);

                try
                {
                    await UoW.LikedPosts.CreateAsync(mappedDislikedPost);

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

                        if (dislike == false)
                        {
                            postToDislike.Score -= 1;
                            await UoW.LikedPosts.UpdateAsync(postToDislike);
                        }
                    }
                    else if (postToDislike.Like == false && postToDislike.Score == 0)
                    {
                        var dislike = postToDislike.Like = false;

                        if (dislike == false)
                        {
                            postToDislike.Score -= 1;
                            await UoW.LikedPosts.UpdateAsync(postToDislike);
                        }
                    }
                    else if (postToDislike.Like == true && postToDislike.Score == 0)
                    {
                        var dislike = postToDislike.Like = false;

                        if (dislike == false)
                        {
                            postToDislike.Score -= 1;
                            await UoW.LikedPosts.UpdateAsync(postToDislike);
                        }
                    }
                    else if (postToDislike.Like == false && postToDislike.Score == -1)
                    {
                        var like = postToDislike.Like = true;

                        if (like == true)
                        {
                            postToDislike.Score += 1;
                            await UoW.LikedPosts.UpdateAsync(postToDislike);
                        }
                    }
                    else if (postToDislike.Like == true && postToDislike.Score == 1)
                    {
                        var dislike = postToDislike.Like = false;

                        if (dislike == false)
                        {
                            postToDislike.Score -= 2;

                            await UoW.LikedPosts.UpdateAsync(postToDislike);
                        }
                    }
                }
                catch
                {
                    throw new Exception();
                }
            }

            return Ok(post);
            //return RedirectToAction("ViewPost", "Comment", new { id = id });
        }

        [HttpPost, Route("LikeComment")]
        public async Task<IActionResult> LikeComment(int id)
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            var user = await userService.GetUserAsync(this.User.Identity.Name);
            var comment = await UoW.Comments.GetByIdAsync(id);

            var commentToLike = await db.LikedComments.Where(c => c.CommentId == id && c.UserId == user.Id).FirstOrDefaultAsync();
            //var likedPost = new Liked_PostsDTO();


            if (commentToLike == null)
            {
                var likedComment = new Liked_CommentsDTO()
                {
                    CommentId = comment.Id,
                    UserId = user.Id,
                    Like = null,
                    Score = 0,
                    isCreator = false
                };

                var mappedLikedComment = mapper.Map<Liked_Comments>(likedComment);

                try
                {
                    await UoW.LikedComments.CreateAsync(mappedLikedComment);

                    if (mappedLikedComment.Like == null)
                    {
                        var like = mappedLikedComment.Like = true;

                        if (like == true)
                        {
                            mappedLikedComment.Score += 1;
                            await UoW.LikedComments.UpdateAsync(mappedLikedComment);
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
                    if (commentToLike.Like == null)
                    {
                        var like = commentToLike.Like = true;

                        if (like == true)
                        {
                            commentToLike.Score += 1;
                            await UoW.LikedComments.UpdateAsync(commentToLike);
                        }
                    }
                    else if (commentToLike.Like == true && commentToLike.Score == 1)
                    {
                        var dislike = commentToLike.Like = false;

                        if (dislike == false)
                        {
                            commentToLike.Score -= 1;
                            await UoW.LikedComments.UpdateAsync(commentToLike);
                        }
                    }
                    else if (commentToLike.Like == false && commentToLike.Score == 0)
                    {
                        var like = commentToLike.Like = true;

                        if (like == true)
                        {
                            commentToLike.Score += 1;
                            await UoW.LikedComments.UpdateAsync(commentToLike);
                        }
                    }
                    else if (commentToLike.Like == true && commentToLike.Score == 0)
                    {
                        var like = commentToLike.Like = true;

                        if (like == true)
                        {
                            commentToLike.Score += 1;
                            await UoW.LikedComments.UpdateAsync(commentToLike);
                        }
                    }
                    else if (commentToLike.Like == false && commentToLike.Score == -1)
                    {
                        var like = commentToLike.Like = true;

                        if (like == true)
                        {
                            commentToLike.Score += 2;
                            await UoW.LikedComments.UpdateAsync(commentToLike);
                        }
                    }
                }
                catch
                {
                    throw new Exception();
                }
            }
            return Ok(comment);
            //return RedirectToAction("ViewPost", "Comment", new { id = id });
        }

        [HttpPost, Route("DislikeComment")]
        public async Task<IActionResult> DislikeComment(int id)
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            var user = await userService.GetUserAsync(this.User.Identity.Name);
            var comment = await db.Comments.Where(x => x.Id == id).FirstOrDefaultAsync();

            var commentToDislike = await db.LikedComments.Where(c => c.CommentId == id && c.UserId == user.Id).FirstOrDefaultAsync();

            if (commentToDislike == null)
            {
                var dislikedComment = new Liked_CommentsDTO()
                {
                    CommentId = comment.Id,
                    UserId = user.Id,
                    Like = null,
                    Score = 0,
                    isCreator = false
                };

                var mappedDislikedComment = mapper.Map<Liked_Comments>(dislikedComment);

                try
                {
                    await UoW.LikedComments.CreateAsync(mappedDislikedComment);

                    if (mappedDislikedComment.Like == null)
                    {
                        var dislike = mappedDislikedComment.Like = false;

                        if (dislike == false)
                        {
                            mappedDislikedComment.Score -= 1;
                            await UoW.LikedComments.UpdateAsync(mappedDislikedComment);
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
                    if (commentToDislike.Like == null)
                    {
                        var dislike = commentToDislike.Like = false;

                        if (dislike == false)
                        {
                            commentToDislike.Score -= 1;
                            await UoW.LikedComments.UpdateAsync(commentToDislike);
                        }
                    }
                    else if (commentToDislike.Like == false && commentToDislike.Score == 0)
                    {
                        var dislike = commentToDislike.Like = false;

                        if (dislike == false)
                        {
                            commentToDislike.Score -= 1;
                            await UoW.LikedComments.UpdateAsync(commentToDislike);
                        }
                    }
                    else if (commentToDislike.Like == true && commentToDislike.Score == 0)
                    {
                        var dislike = commentToDislike.Like = false;

                        if (dislike == false)
                        {
                            commentToDislike.Score -= 1;
                            await UoW.LikedComments.UpdateAsync(commentToDislike);
                        }
                    }
                    else if (commentToDislike.Like == false && commentToDislike.Score == -1)
                    {
                        var like = commentToDislike.Like = true;

                        if (like == true)
                        {
                            commentToDislike.Score += 1;
                            await UoW.LikedComments.UpdateAsync(commentToDislike);
                        }
                    }
                    else if (commentToDislike.Like == true && commentToDislike.Score == 1)
                    {
                        var dislike = commentToDislike.Like = false;

                        if (dislike == false)
                        {
                            commentToDislike.Score -= 2;

                            await UoW.LikedComments.UpdateAsync(commentToDislike);
                        }
                    }
                }
                catch
                {
                    throw new Exception();
                }
            }

            return Ok(comment);
            //return RedirectToAction("ViewPost", "Comment", new { id = id });
        }
    }
}
