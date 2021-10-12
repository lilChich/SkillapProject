using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skillap.DAL.Entities
{
    public class Tags
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public IList<Post_Tags> PostTags { get; set; }
    }
}
