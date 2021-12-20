using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skillap.MVC.ViewModels
{
    public class SubCommentsViewModel : CommentViewModel
    {
        public int MainCommentId { get; set; }
    }
}
