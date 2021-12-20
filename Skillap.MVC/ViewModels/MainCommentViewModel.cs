using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skillap.MVC.ViewModels
{
    public class MainCommentViewModel : CommentViewModel
    {
        public List<SubCommentsViewModel> SubComments { get; set; }
    }
}
