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

        public Task UpdateMasterClasses(MasterClasses masterClasses);
        public Task<bool> StatusInMastersTable(MastersDTO mastersDto);
        public Task<int> ChangeSkillLevel(MastersDTO mastersDto);
        public MasterClasses GetMasterClassByNameAndDescription(string name, string description);
        public MasterClasses GetMasterClassByIdThroughDb(int id);
        public Task<Masters> GetMasterByIdAsync(int id);
        public IEnumerable<MasterClassesDTO> GetAllMasterClassesAsync();
        public IEnumerable<MastersDTO> GetAllMastersAsync();
    }
}
