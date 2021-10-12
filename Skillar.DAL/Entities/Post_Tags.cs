using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skillap.DAL.Entities
{
    public class Post_Tags
    {
        public int Id { get; set; }

        public int PostId { get; set; }
        public Posts Post { get; set; }
        public int TagId { get; set; }
        public Tags Tag { get; set; }
    }
}
