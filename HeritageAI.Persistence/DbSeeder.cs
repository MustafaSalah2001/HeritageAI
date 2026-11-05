using HeritageAI.Application.Interfaces;
using HeritageAI.Domain.Entities;
using HeritageAI.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HeritageAI.Persistence;

// بيانات تجريبية واقعية: أدمن واحد، 3 حرفيين (موافق عليهم مسبقاً مع منتجات
// منشورة)، ومشتريين اتنين. كل الباسوردات بتتعمل Hash حقيقي بنفس آلية
// AuthService، فبتقدري تسجلي دخول فوراً من صفحة Login العادية.
public static class DbSeeder
{
    private static string SharedPassword = "Passw0rd!";
    public static async Task SeedAsync(AppDbContext context, IPasswordHasher hasher,
     string? adminPassword = null, string? demoPassword = null)
    {
        if (await context.Users.AnyAsync(u => u.Email == "admin@ghorzza.ps"))
            return;

        if (!string.IsNullOrWhiteSpace(demoPassword))
            SharedPassword = demoPassword;

        // ===== 1) الأدمن =====
        await EnsureUserAsync(context, hasher, "مدير المنصة", "admin@ghorzza.ps",
            string.IsNullOrWhiteSpace(adminPassword) ? SharedPassword : adminPassword,
            UserRole.Admin);
        // ===== 2) التصنيفات (بيعيد استخدام أي تصنيف موجود عندك مسبقاً بنفس الاسم) =====
        var thobeId = await GetOrCreateCategoryIdAsync(context, "ثوب");
        var pillowId = await GetOrCreateCategoryIdAsync(context, "وسادة");
        var shawlId = await GetOrCreateCategoryIdAsync(context, "شال");

        // ===== 3) الحرفيين الثلاثة (بحالة Approved فوراً عشان يبينوا بالـ landing page) =====
        var craftsman1Id = await CreateCraftsmanAsync(context, hasher,
            fullName: "فاطمة الحلبي",
            email: "fatima.craftsman@ghorzza.ps",
            bio: "حرفية متخصصة بتطريز القدس التقليدي منذ أكثر من 15 عاماً، تتوارث غرزة الصليب المقدسية عن والدتها وجدتها.",
            storyText: "بدأت فاطمة التطريز وهي بعمر 12 عاماً في بيت جدتها بالبلدة القديمة بالقدس. اليوم تدرب أكثر من 20 حرفية شابة وتحافظ على دقة النقشة المقدسية الأصيلة في كل قطعة تصنعها.",
            skills: new[] { "تطريز القدس", "غرزة الصليب", "تطريز يدوي دقيق" });

        var craftsman2Id = await CreateCraftsmanAsync(context, hasher,
            fullName: "سميرة أبو حماد",
            email: "samira.craftsman@ghorzza.ps",
            bio: "حرفية من خانيونس تجمع بين الألوان الدافئة المستوحاة من شمس الساحل والتقنيات الحديثة في التطريز اليدوي.",
            storyText: "تعلمت سميرة التطريز من نساء الحي، وحولت موهبتها لمصدر دخل مستقل يعيل أسرتها، وهي اليوم من أكثر الحرفيات طلباً في تطريز غزة الساحلي.",
            skills: new[] { "تطريز غزة", "ألوان ساحلية", "تطريز على الكتان" });

        var craftsman3Id = await CreateCraftsmanAsync(context, hasher,
            fullName: "نائلة الخليلي",
            email: "naila.craftsman@ghorzza.ps",
            bio: "معلمة تطريز معتمدة من بلدة الخليل القديمة، اشتهرت بدقة الزخارف الهندسية النيلية المعقدة.",
            storyText: "تدير نائلة ورشة صغيرة تضم خمس حرفيات، وتعتبر من الأسماء الموثوقة بالحفاظ على نقشة الخليل الهندسية الدقيقة بخيوط الصوف والحرير.",
            skills: new[] { "تطريز الخليل", "زخارف هندسية", "تطريز بالصوف" });

        // ===== 4) منتجات كل حرفي (Published مباشرة عشان تبين بالسوق والـ landing page) =====
        await AddProductAsync(context, craftsman1Id, thobeId,
            name: "ثوب القدس الملكي",
            description: "ثوب تراثي كامل بقبة صدر مطرزة يدوياً بغرزة الصليب المقدسية باللون الأحمر الغامق، مستوحى من زخارف باب العامود التاريخية.",
            price: 320, style: EmbroideryStyle.Tatreez, colors: "أحمر، أسود، ذهبي", customizable: true, days: 14);

        await AddProductAsync(context, craftsman1Id, pillowId,
            name: "وسادة نقش القدس",
            description: "وسادة زخرفية مربعة مطرزة بنقشة القدس التقليدية، مثالية لإضفاء لمسة تراثية أنيقة.",
            price: 45, style: EmbroideryStyle.Tatreez, colors: "أحمر، كريمي", customizable: false, days: 5);

        await AddProductAsync(context, craftsman2Id, thobeId,
            name: "ثوب غزة الصيفي",
            description: "ثوب خفيف مناسب للأجواء الحارة، بألوان دافئة مستوحاة من شمس الساحل وأمواج البحر.",
            price: 250, style: EmbroideryStyle.CrossStitch, colors: "برتقالي، أبيض", customizable: true, days: 10);

        await AddProductAsync(context, craftsman2Id, shawlId,
            name: "شال غزة الدافئ",
            description: "شال صوفي ناعم بحواف مطرزة بنقشات ساحلية بسيطة وأنيقة.",
            price: 60, style: EmbroideryStyle.Tatreez, colors: "برتقالي، بني", customizable: false, days: 4);

        await AddProductAsync(context, craftsman3Id, pillowId,
            name: "وسادة نقش الخليل",
            description: "وسادة زخرفية مطرزة بنقشة الخليل الهندسية النيلية الدقيقة.",
            price: 45, style: EmbroideryStyle.Tatreez, colors: "نيلي، كريمي", customizable: false, days: 6);

        await AddProductAsync(context, craftsman3Id, shawlId,
            name: "شال شتوي مطرز",
            description: "شال صوفي دافئ بحدود مطرزة بنقشة الخليل الهندسية، مثالي لفصل الشتاء.",
            price: 75, style: EmbroideryStyle.CrossStitch, colors: "نيلي، رمادي", customizable: true, days: 8);

        await AddProductAsync(context, craftsman3Id, thobeId,
            name: "ثوب الخليل التراثي",
            description: "ثوب تراثي كامل بزخارف هندسية دقيقة على طراز الخليل الأصيل.",
            price: 300, style: EmbroideryStyle.Tatreez, colors: "نيلي، ذهبي", customizable: true, days: 16);

        // ===== 5) المشترين الاتنين =====
        await EnsureUserAsync(context, hasher, "رهف عودة", "rahaf.buyer@ghorzza.ps", SharedPassword, UserRole.Buyer);
        await EnsureUserAsync(context, hasher, "خالد مراد", "khaled.buyer@ghorzza.ps", SharedPassword, UserRole.Buyer);
    }

    private static async Task<int> GetOrCreateCategoryIdAsync(AppDbContext context, string name)
    {
        var category = await context.Categories.FirstOrDefaultAsync(c => c.Name == name);
        if (category is not null)
            return category.Id;

        category = new Category { Name = name };
        context.Categories.Add(category);
        await context.SaveChangesAsync();
        return category.Id;
    }

    private static async Task<int> EnsureUserAsync(
        AppDbContext context, IPasswordHasher hasher,
        string fullName, string email, string password, UserRole role)
    {
        var existing = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (existing is not null)
            return existing.Id;

        var user = new User
        {
            FullName = fullName,
            Email = email,
            PasswordHash = hasher.Hash(password),
            Role = role,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user.Id;
    }

    private static async Task<int> CreateCraftsmanAsync(
        AppDbContext context, IPasswordHasher hasher,
        string fullName, string email, string bio, string storyText, string[] skills)
    {
        var userId = await EnsureUserAsync(context, hasher, fullName, email, SharedPassword, UserRole.Craftsman);

        var existingCraftsman = await context.Craftsmen.FirstOrDefaultAsync(c => c.UserId == userId);
        if (existingCraftsman is not null)
            return existingCraftsman.UserId;

        var craftsman = new Craftsman
        {
            UserId = userId,
            Bio = bio,
            StoryText = storyText,
            Status = CraftsmanStatus.Approved
        };
        context.Craftsmen.Add(craftsman);
        await context.SaveChangesAsync();

        foreach (var skillName in skills)
        {
            context.CraftsmanSkills.Add(new CraftsmanSkill
            {
                CraftsmanId = craftsman.UserId,
                Name = skillName
            });
        }
        await context.SaveChangesAsync();

        return craftsman.UserId;
    }

    private static async Task AddProductAsync(
        AppDbContext context, int craftsmanId, int categoryId,
        string name, string description, decimal price,
        EmbroideryStyle style, string colors, bool customizable, int days)
    {
        context.Products.Add(new Product
        {
            Name = name,
            Description = description,
            Price = price,
            CraftsmanId = craftsmanId,
            CategoryId = categoryId,
            EmbroideryStyle = style,
            Colors = colors,
            IsCustomizable = customizable,
            EstimatedProductionDays = days,
            Status = ProductStatus.Published,
            CreatedAt = DateTime.UtcNow
        });
        await context.SaveChangesAsync();
    }
}