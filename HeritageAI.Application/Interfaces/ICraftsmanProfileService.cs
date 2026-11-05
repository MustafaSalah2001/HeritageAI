using HeritageAI.Application.Dto.Craftsman;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritageAI.Application.Interfaces
{
    public interface ICraftsmanProfileService
    {
        Task<CraftsmanProfileDto> GetMyProfileAsync(int userId);
        Task<CraftsmanProfileDto> UpdateMyProfileAsync(int userId, UpdateCraftsmanProfileDto dto);
        Task UpdatePasswordAsync(int userId, string currentPassword, string newPassword);
        Task DeactivateMyAccountAsync(int userId);
        Task<string> UploadProfileImageAsync(int userId, Stream fileStream, string fileName);
        Task DeleteProfileImageAsync(int userId);   // ← جديد
        Task<List<string>> UpdateSkillsAsync(int userId, List<string> skills);
    }
}
