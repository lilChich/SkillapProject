using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skillap.DAL.Entities
{
    public class Comments
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedTime { get; set; }

        public int PostId { get; set; }
        public Posts Post { get; set; }

        public IList<Liked_Comments> CommentsLiked { get; set; }
    }
}
