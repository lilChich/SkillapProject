using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skillap.DAL.Entities
{
    public class Liked_Comments
    {
        public int Id { get; set; }
        public int Score { get; set; }

        public int UserId { get; set; }
        public ApplicationUsers ApplicationUser { get; set; }

        public int CommentId { get; set; }
        public Comments Comment { get; set; }
    }
}
