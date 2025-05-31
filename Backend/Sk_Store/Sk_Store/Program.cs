using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Repositories;
using Repositories.UnitOfWork;
using Services.Implementations;
using Sk_Store.Services.Interfaces;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
// File: Sk_Store/Program.cs

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
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthService, AuthService>();

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
}

app.UseHttpsRedirection();

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthorization();

app.MapControllers();

app.Run();
