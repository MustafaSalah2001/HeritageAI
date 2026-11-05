using HeritageAI.Application.Dto.Auth;
using HeritageAI.Application.Interfaces;
using HeritageAI.Domain.Entities;
using HeritageAI.Domain.Enums;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ICraftsmanRepository _craftsmanRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly INotificationRepository _notifications;

    public AuthService(
        IUserRepository userRepository,
        ICraftsmanRepository craftsmanRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        INotificationRepository notifications)
    {
        _userRepository = userRepository;
        _craftsmanRepository = craftsmanRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _notifications = notifications;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);

        if (existingUser is not null)
            throw new InvalidOperationException("Email already registered.");

        if (request.Role == UserRole.Admin)
            throw new InvalidOperationException("Cannot self-register as Admin.");

        if (request.Role == UserRole.Craftsman && (request.Bio?.Trim().Length ?? 0) < 20)
            throw new InvalidOperationException("Craftsman bio must be at least 20 characters.");

        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = _passwordHasher.Hash(request.Password),
            Role = request.Role,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        if (request.Role == UserRole.Craftsman)
        {
            var craftsman = new Craftsman
            {
                UserId = user.Id,
                Bio = request.Bio?.Trim() ?? string.Empty,
                Status = CraftsmanStatus.Pending
            };

            await _craftsmanRepository.AddAsync(craftsman);

            // إشعار لكل أدمن: حرفي جديد بانتظار المراجعة (بيتحفظ مع الحرفي بنفس الـ SaveChanges)
            await _notifications.AddForAdminsAsync(
                "حرفي جديد بانتظار المراجعة",
                $"{user.FullName} سجّل كحرفي وبانتظار موافقتك.",
                "/admin/craftsmen");

            await _userRepository.SaveChangesAsync();
        }

        var (token, expiresAt) = _jwtTokenGenerator.GenerateToken(user);

        return new AuthResponseDto
        {
            Token = token,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role.ToString(),
            ExpiresAt = expiresAt
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password.");
      

        if (!user.IsActive)
            throw new UnauthorizedAccessException("Your account has been deactivated.");
        var (token, expiresAt) = _jwtTokenGenerator.GenerateToken(user);

        return new AuthResponseDto
        {
            Token = token,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role.ToString(),
            ExpiresAt = expiresAt
        };

    }
    public async Task ChangePasswordAsync(int userId, ChangePasswordDto request)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user is null)
            throw new InvalidOperationException("User not found.");

        if (!_passwordHasher.Verify(request.CurrentPassword, user.PasswordHash))
            throw new InvalidOperationException("Current password is incorrect.");

        if (string.IsNullOrWhiteSpace(request.NewPassword) || request.NewPassword.Length < 8)
            throw new InvalidOperationException("New password must be at least 8 characters.");

        if (request.NewPassword == request.CurrentPassword)
            throw new InvalidOperationException("New password must be different from the current one.");

        user.PasswordHash = _passwordHasher.Hash(request.NewPassword);
        await _userRepository.SaveChangesAsync();
    }
}