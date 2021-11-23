using Skillap.BLL.DTO;
using Skillap.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skillap.BLL.Interfaces.IServices
{
    public interface IMasterService
    {
        public Task CreateMasterClass(MasterClassesDTO masterClassesDto);
        public Task<bool> UpdateMasterClass(MasterClassesDTO masterClassesDto);
        public Task<bool> DeleteMasterClass(MasterClassesDTO masterClassesDto);
        public Task<MasterClassesDTO> GetMasterClassById(int id);

        public Task<bool> StatusInMastersTable(MastersDTO mastersDto);
        public Task<int> ChangeSkillLevel(MastersDTO mastersDto);
    }
}
