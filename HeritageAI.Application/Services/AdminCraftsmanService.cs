using HeritageAI.Application.Dto.Admin;
using HeritageAI.Application.Interfaces;
using HeritageAI.Domain.Entities;
using HeritageAI.Domain.Enums;

public class AdminCraftsmanService : IAdminCraftsmanService
{
    private readonly ICraftsmanRepository _craftsmanRepository;
    private readonly IProductRepository _productRepository;
    private readonly INotificationRepository _notifications;
    private const string CraftsmanHome = "/craftsman/dashboard";
    public AdminCraftsmanService(
    ICraftsmanRepository craftsmanRepository,
    IProductRepository productRepository,
    INotificationRepository notifications)
    {
        _craftsmanRepository = craftsmanRepository;
        _productRepository = productRepository;
        _notifications = notifications;
    }

    public async Task<List<CraftsmanAdminDto>> GetPendingAsync()
    {
        var craftsmen = await _craftsmanRepository.GetPendingAsync();
        var result = new List<CraftsmanAdminDto>();

        foreach (var c in craftsmen)
        {
            var dto = MapToDto(c);

            var products = await _productRepository.GetByCraftsmanIdAsync(c.UserId);
            dto.Works = products.Select(p => new CraftsmanWorkDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                CategoryName = p.Category.Name,
                ImageUrls = p.Images
                    .OrderByDescending(i => i.IsMain)
                    .Select(i => i.Url)
                    .ToList()
            }).ToList();

            result.Add(dto);
        }

        return result;
    }
    public async Task<List<CraftsmanAdminDto>> GetAllAsync()
    {
        var craftsmen = await _craftsmanRepository.GetAllAsync();
        var products = await _productRepository.GetAllAsync();
        var counts = products
            .GroupBy(p => p.CraftsmanId)
            .ToDictionary(g => g.Key, g => g.Count());

        return craftsmen
            .Select(c =>
            {
                var dto = MapToDto(c);
                dto.ProductsCount = counts.GetValueOrDefault(c.UserId);
                return dto;
            })
            .OrderByDescending(d => d.CreatedAt)
            .ToList();
    }
    public async Task<CraftsmanAdminDto?> GetDetailsAsync(int userId)
    {
        var all = await _craftsmanRepository.GetAllAsync();
        var craftsman = all.FirstOrDefault(c => c.UserId == userId);
        if (craftsman is null) return null;

        var dto = MapToDto(craftsman);
        var products = await _productRepository.GetByCraftsmanIdAsync(userId);

        dto.ProductsCount = products.Count;
        dto.Works = products
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new CraftsmanWorkDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                CategoryName = p.Category.Name,
                Status = p.Status.ToString(),
                ImageUrls = p.Images
                    .OrderByDescending(i => i.IsMain)
                    .Select(i => i.Url)
                    .ToList()
            }).ToList();

        return dto;
    }
    public async Task ApproveAsync(int userId)
    {
        var craftsman = await _craftsmanRepository.GetByUserIdAsync(userId);

        if (craftsman is null)
            throw new KeyNotFoundException("Craftsman not found.");

        var changed = craftsman.Status != CraftsmanStatus.Approved;
        craftsman.Status = CraftsmanStatus.Approved;

        // أعماله اللي أضافها أثناء المراجعة بتنشر مباشرة
        var products = await _productRepository.GetByCraftsmanIdAsync(userId);
        foreach (var product in products.Where(p => p.Status == ProductStatus.Draft))
        {
            product.Status = ProductStatus.Published;
        }

        if (changed)
        {
            _notifications.Add(new Notification
            {
                UserId = userId,
                Title = "تمت الموافقة على حسابك",
                Message = "قبلت الإدارة طلبك كحرفي. صار بإمكانك نشر منتجاتك واستقبال الطلبات.",
                Link = CraftsmanHome
            });
        }

        await _craftsmanRepository.SaveChangesAsync();
    }

    public async Task RejectAsync(int userId)
    {
        var craftsman = await _craftsmanRepository.GetByUserIdAsync(userId);

        if (craftsman is null)
            throw new KeyNotFoundException("Craftsman not found.");

        var changed = craftsman.Status != CraftsmanStatus.Rejected;
        craftsman.Status = CraftsmanStatus.Rejected;

        // إخفاء كل منتجاته المنشورة تلقائيًا
        var products = await _productRepository.GetByCraftsmanIdAsync(userId);
        foreach (var product in products.Where(p => p.Status == ProductStatus.Published))
        {
            product.Status = ProductStatus.Rejected;
        }

        if (changed)
        {
            _notifications.Add(new Notification
            {
                UserId = userId,
                Title = "تم رفض طلب انضمامك",
                Message = "رفضت الإدارة طلبك كحرفي، وتم إخفاء منتجاتك. تواصل معنا إذا بدك توضيح.",
                Link = CraftsmanHome
            });
        }

        await _craftsmanRepository.SaveChangesAsync();
    }
    private static CraftsmanAdminDto MapToDto(Craftsman c) => new()
    {
        UserId = c.UserId,
        FullName = c.User.FullName,
        Email = c.User.Email,
        Bio = c.Bio,
        IsActive = c.User.IsActive,
        StoryText = c.StoryText,
        ProfileImageUrl = c.ProfileImageUrl,
        Skills = c.Skills.Select(s => s.Name).ToList(),
        Status = c.Status.ToString(),
        CreatedAt = c.User.CreatedAt
    };
}