using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skillap.BLL.DTO
{
    public class MastersDTO
    {
        public int Id { get; set; }
        public bool Status { get; set; }
        public int SkillLevel { get; set; }

        public int MasterClassId { get; set; }
        public int ApplicationUserId { get; set; }
    }
}
