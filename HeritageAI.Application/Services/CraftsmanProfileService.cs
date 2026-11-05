using HeritageAI.Application.Dto.Craftsman;
using HeritageAI.Application.Interfaces;
using HeritageAI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritageAI.Application.Services
{
    public class CraftsmanProfileService : ICraftsmanProfileService
    {
        private readonly ICraftsmanRepository _craftsmanRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IFileStorageService _fileStorage;

        public CraftsmanProfileService(
            IFileStorageService fileStorage,
            ICraftsmanRepository craftsmanRepository,
            IUserRepository userRepository,
            IPasswordHasher passwordHasher)
        {
            _craftsmanRepository = craftsmanRepository;
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _fileStorage = fileStorage;

        }

        public async Task<CraftsmanProfileDto> GetMyProfileAsync(int userId)
        {
            var craftsman = await _craftsmanRepository.GetByUserIdAsync(userId);

            if (craftsman is null)
                throw new KeyNotFoundException("Craftsman profile not found.");

            return MapToDto(craftsman);
        }
        public async Task<string> UploadProfileImageAsync(int userId, Stream fileStream, string fileName)
        {
            var craftsman = await _craftsmanRepository.GetByUserIdAsync(userId);

            if (craftsman is null)
                throw new KeyNotFoundException("Craftsman profile not found.");

            if (!string.IsNullOrWhiteSpace(craftsman.ProfileImageUrl))
            {
                _fileStorage.DeleteFile(craftsman.ProfileImageUrl);
            }

            var url = await _fileStorage.SaveProductImageAsync(fileStream, fileName);

            craftsman.ProfileImageUrl = url;
            await _craftsmanRepository.SaveChangesAsync();

            return url;
        }


        public async Task<CraftsmanProfileDto> UpdateMyProfileAsync(int userId, UpdateCraftsmanProfileDto dto)
        {
            var craftsman = await _craftsmanRepository.GetByUserIdAsync(userId);

            if (craftsman is null)
                throw new KeyNotFoundException("Craftsman profile not found.");

            // تحديث بيانات الـ User الأساسية
            if (string.IsNullOrWhiteSpace(dto.FullName))
                throw new InvalidOperationException("Full name cannot be empty.");

            var existingEmail = await _userRepository.GetByEmailAsync(dto.Email);
            if (existingEmail is not null && existingEmail.Id != userId)
                throw new InvalidOperationException("Email already in use.");

            craftsman.User.FullName = dto.FullName;
            craftsman.User.Email = dto.Email;

            // تحديث بيانات الـ Craftsman
            craftsman.Bio = dto.Bio;
            craftsman.StoryText = dto.StoryText;

            await _craftsmanRepository.SaveChangesAsync();

            return MapToDto(craftsman);
        }

        public async Task UpdatePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user is null)
                throw new KeyNotFoundException("User not found.");

            if (!_passwordHasher.Verify(currentPassword, user.PasswordHash))
                throw new UnauthorizedAccessException("Current password is incorrect.");

            user.PasswordHash = _passwordHasher.Hash(newPassword);
            await _userRepository.SaveChangesAsync();
        }

        public async Task DeactivateMyAccountAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user is null)
                throw new KeyNotFoundException("User not found.");

            user.IsActive = false;
            await _userRepository.SaveChangesAsync();
        }
        public async Task DeleteProfileImageAsync(int userId)
        {
            var craftsman = await _craftsmanRepository.GetByUserIdAsync(userId);

            if (craftsman is null)
                throw new KeyNotFoundException("Craftsman profile not found.");

            if (string.IsNullOrWhiteSpace(craftsman.ProfileImageUrl))
                throw new InvalidOperationException("No profile image to delete.");

            _fileStorage.DeleteFile(craftsman.ProfileImageUrl);
            craftsman.ProfileImageUrl = null;

            await _craftsmanRepository.SaveChangesAsync();
        }
        private static CraftsmanProfileDto MapToDto(Craftsman c) => new()
        {
            UserId = c.UserId,
            FullName = c.User.FullName,
            Email = c.User.Email,
            Bio = c.Bio,
            StoryText = c.StoryText,
            ProfileImageUrl = c.ProfileImageUrl,
            Status = c.Status.ToString(),
            Skills = c.Skills.Select(s => s.Name).ToList()
        };
        public async Task<List<string>> UpdateSkillsAsync(int userId, List<string> skills)
        {
            var craftsman = await _craftsmanRepository.GetByUserIdAsync(userId);

            if (craftsman is null)
                throw new KeyNotFoundException("Craftsman profile not found.");

            // نمسح كل المهارات القديمة، ونحط الجديدة بدالها
            craftsman.Skills.Clear();

            foreach (var skillName in skills.Where(s => !string.IsNullOrWhiteSpace(s)))
            {
                craftsman.Skills.Add(new CraftsmanSkill { CraftsmanId = userId, Name = skillName.Trim() });
            }

            await _craftsmanRepository.SaveChangesAsync();

            return craftsman.Skills.Select(s => s.Name).ToList();
        }
    }
}
