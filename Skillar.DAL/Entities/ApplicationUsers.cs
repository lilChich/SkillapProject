using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skillap.DAL.Entities
{
    public class ApplicationUsers : IdentityUser<int>
    {
        [Required]
        [MaxLength(30)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(30)]
        public string SecondName { get; set; }
        [Required]
        public bool Gender { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        [MinLength(2)]
        [MaxLength(35)]
        public string Country { get; set; }
        [Required]
        [MaxLength(200)]
        public string Education { get; set; }
        public byte[] Image { get; set; }

        public IList<Masters> Masters { get; set; }
        public IList<Liked_Comments> CommentsLiked { get; set; }
        public IList<Liked_Posts> PostsLiked { get; set; }

        public ApplicationUsers()
        {

        }
    }
}
