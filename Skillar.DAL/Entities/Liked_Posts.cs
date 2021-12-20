using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skillap.DAL.Entities
{
    public class Liked_Posts
    {
        public int Id { get; set; }
        public int? Score { get; set; }
        public Nullable<bool> Like { get; set; }

        public int UserId { get; set; }
        public ApplicationUsers Users { get; set; }

        public int PostId { get; set; }
        public Posts Posts { get; set; }
    }
}
