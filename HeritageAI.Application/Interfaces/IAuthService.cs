using HeritageAI.Application.Dto.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritageAI.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
        Task ChangePasswordAsync(int userId, ChangePasswordDto request);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
    }
}
