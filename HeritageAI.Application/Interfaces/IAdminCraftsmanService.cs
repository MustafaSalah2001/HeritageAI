using HeritageAI.Application.Dto.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritageAI.Application.Interfaces
{
    public interface IAdminCraftsmanService
    {
        Task<List<CraftsmanAdminDto>> GetPendingAsync();
        Task<List<CraftsmanAdminDto>> GetAllAsync();
        Task<CraftsmanAdminDto?> GetDetailsAsync(int userId);
        Task ApproveAsync(int userId);
        Task RejectAsync(int userId);

    }
}
