using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skillap.DAL.Entities
{
    public class Posts
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedTime { get; set; }
        public bool Status { get; set; }

        public IList<Post_Tags> PostTags { get; set; }
        public IList<Liked_Posts> PostsLiked { get; set; }
        public IList<Comments> Comments { get; set; }

    }
}
