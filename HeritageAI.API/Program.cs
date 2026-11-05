using HeritageAI.API.Filters;
using HeritageAI.Application.Interfaces;
using HeritageAI.Application.Services;
using HeritageAI.Persistence;
using HeritageAI.Persistence.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using HeritageAI.API.Services;


var builder = WebApplication.CreateBuilder(args);

// =====================================================
// SERVICES
// =====================================================

// Controllers
builder.Services.AddControllers();

// OpenAPI (built-in .NET 9) — مع إضافة دعم Bearer Token
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Components ??= new OpenApiComponents();

        // 1) تعريف نوع الأمان (Bearer JWT)
        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "اكتب التوكن فقط، بدون كلمة Bearer — الواجهة هتضيفها تلقائيًا"
        };

        // 2) تفعيل الأمان على كل الـ Endpoints
        document.SecurityRequirements.Add(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });

        return Task.CompletedTask;
    });
});

// =====================================================
// DATABASE
// =====================================================

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new Exception(
        "Database connection string 'DefaultConnection' is missing.");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// =====================================================
// APPLICATION SERVICES
// =====================================================
builder.Services.AddHttpClient<IAiDesignService, AiDesignService>(c => c.Timeout = TimeSpan.FromSeconds(60));
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICraftsmanRepository, CraftsmanRepository>();
builder.Services.AddScoped<IAdminCraftsmanService, AdminCraftsmanService>();
builder.Services.AddScoped<IAdminUserService, AdminUserService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICraftsmanProfileService, CraftsmanProfileService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IAdminProductService, AdminProductService>();
builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();
builder.Services.AddScoped<IProductImageService, ProductImageService>();
builder.Services.AddScoped<IProductImageRepository, ProductImageRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IProductReviewRepository, ProductReviewRepository>();
builder.Services.AddScoped<IProductReviewService, ProductReviewService>();
builder.Services.AddScoped<ICustomOrderRepository, CustomOrderRepository>();
builder.Services.AddScoped<ICustomOrderService, CustomOrderService>();
builder.Services.AddScoped<ISiteContentService, SiteContentService>();
builder.Services.AddScoped<ISiteContentRepository, SiteContentRepository>();
builder.Services.AddScoped<IPublicCraftsmanService, PublicCraftsmanService>();
builder.Services.AddScoped<ITestimonialRepository, TestimonialRepository>();
builder.Services.AddScoped<ITestimonialService, TestimonialService>();
// =====================================================
// JWT CONFIGURATION
// =====================================================
builder.Services.AddScoped<ActiveUserFilter>();

builder.Services.AddControllers(options =>
{
    options.Filters.AddService<ActiveUserFilter>();
});
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new Exception(
        "JWT Key is missing. Check 'Jwt:Key' in appsettings.json.");
}

if (string.IsNullOrWhiteSpace(jwtIssuer))
{
    throw new Exception(
        "JWT Issuer is missing. Check 'Jwt:Issuer' in appsettings.json.");
}

if (string.IsNullOrWhiteSpace(jwtAudience))
{
    throw new Exception(
        "JWT Audience is missing. Check 'Jwt:Audience' in appsettings.json.");
}

// =====================================================
// AUTHENTICATION
// =====================================================

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey)
                    ),

                ClockSkew = TimeSpan.Zero
            };
    });

// =====================================================
// AUTHORIZATION
// =====================================================

builder.Services.AddAuthorization();

// =====================================================
// BUILD APPLICATION
// =====================================================
// =====================================================
// CORS
// =====================================================

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
     "http://localhost:5173", "https://localhost:5173",
     "http://localhost:5174", "https://localhost:5174")
               .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
var app = builder.Build();

// =====================================================
// DATABASE SEEDING (بيانات تجريبية أولية)
// =====================================================
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
    await context.Database.MigrateAsync();
    await DbSeeder.SeedAsync(context, hasher,
        builder.Configuration["Seed:AdminPassword"],
        builder.Configuration["Seed:DemoPassword"]);
}

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseCors("AllowFrontend");
// =====================================================
// HTTP REQUEST PIPELINE
// =====================================================

if (app.Environment.IsDevelopment())
{
    // OpenAPI JSON
    app.MapOpenApi();

    // Swagger UI
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/openapi/v1.json",
            "GHURZZA API v1"
        );
    });
}

app.UseHttpsRedirection();
app.UseMiddleware<HeritageAI.API.Middleware.ExceptionHandlingMiddleware>();
// Authentication before Authorization
app.UseAuthentication();

app.UseAuthorization();

// Controllers
app.MapControllers();

// Root endpoint
app.MapFallbackToFile("index.html");

// =====================================================
// RUN
// =====================================================

app.Run();
