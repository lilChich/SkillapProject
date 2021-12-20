using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skillap.MVC.ViewModels
{
    public class CommentViewModel
    {
        public int PostId { get; set; }
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime CreationDate { get; set; }
        
    }
}
