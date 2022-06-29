using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skillap.BLL.DTO
{
    public class Liked_CommentsDTO
    {
        public int Id { get; set; }
        public int Score { get; set; }
        public bool? Like { get; set; }
        public bool isCreator { get; set; }

        public int UserId { get; set; }
        public int CommentId { get; set; }
    }
}
