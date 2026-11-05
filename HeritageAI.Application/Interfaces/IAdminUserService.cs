using HeritageAI.Application.Dto.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritageAI.Application.Interfaces
{
    public interface IAdminUserService
    {
        Task<List<UserAdminDto>> GetAllAsync();
        Task SetActiveStatusAsync(int userId, bool isActive);
    }
}
