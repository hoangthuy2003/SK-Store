using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Repositories;
using Repositories.UnitOfWork;
using Services.Implementations;
using Services.Interfaces;
using Sk_Store.Services.Interfaces;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
// File: Sk_Store/Program.cs
builder.Services.AddMemoryCache();
var connectionString = builder.Configuration.GetConnectionString("SKStoreDbConnection");
builder.Services.AddDbContext<SkstoreContext>(options =>
    options.UseSqlServer(connectionString,
        b => b.MigrationsAssembly("Sk_Store")
    )
);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.AllowAnyOrigin()
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Cấu hình này sẽ bảo trình chuyển đổi JSON bỏ qua các vòng lặp tham chiếu
        // thay vì gây ra lỗi.
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Thêm định nghĩa chung về Swagger
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "SK_Store API",
        Description = "API cho dự án trang web bán văn phòng phẩm SK_Store"
    });

    // Định nghĩa cách Swagger sẽ sử dụng JWT Bearer token để xác thực
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Vui lòng nhập JWT với tiền tố Bearer vào ô dưới đây. Ví dụ: 'Bearer 12345abcdef'",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // Yêu cầu Swagger áp dụng định nghĩa bảo mật ở trên cho tất cả các endpoint
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddHttpClient();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true; // Lưu token trong HttpContext sau khi xác thực
    options.RequireHttpsMetadata = false; // Đặt là true ở môi trường production
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, // Kiểm tra Issuer
        ValidateAudience = true, // Kiểm tra Audience
        ValidateLifetime = true, // Kiểm tra token còn hạn không
        ValidateIssuerSigningKey = true, // Quan trọng: Kiểm tra chữ ký của token
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"] ?? "")),
        ClockSkew = TimeSpan.Zero // Không cho phép chênh lệch thời gian
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    // Không sử dụng HTTPS redirection trong môi trường development
}
else 
{
    app.UseHttpsRedirection();
}

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication(); // Thêm middleware xác thực
app.UseAuthorization();

app.MapControllers();

app.Run();
