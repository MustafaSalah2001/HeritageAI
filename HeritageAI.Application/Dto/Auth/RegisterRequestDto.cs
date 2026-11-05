using HeritageAI.Domain.Enums;

namespace HeritageAI.Application.Dto.Auth
{
    public class RegisterRequestDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserRole Role { get; set; } // Buyer أو Craftsman فقط (مش Admin)
        public string? Bio { get; set; }   // نبذة الحرفي عند التسجيل
    }
}