using HeritageAI.Application.Dto.Admin;
using HeritageAI.Application.Interfaces;
using HeritageAI.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritageAI.Application.Services
{
    public class AdminUserService : IAdminUserService
    {
        private readonly IUserRepository _userRepository;

        public AdminUserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<UserAdminDto>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();

            return users.Select(u => new UserAdminDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                Role = u.Role.ToString(),
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt
            }).ToList();
        }

        public async Task SetActiveStatusAsync(int userId, bool isActive)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user is null)
                throw new KeyNotFoundException("User not found.");

            if (user.Role == UserRole.Admin)
                throw new InvalidOperationException("Cannot deactivate an Admin account.");

            user.IsActive = isActive;
            await _userRepository.SaveChangesAsync();
        }
    }
}
