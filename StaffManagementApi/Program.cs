using Microsoft.EntityFrameworkCore;
using StaffManagementApi.Data;
using StaffManagementApi.Services;
using StaffManagementApi.Mapping;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Options;


var builder = WebApplication.CreateBuilder(args);
// Konfigurasi Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString)); // Pastikan sesuai dengan database yang digunakan

// Login Register
var key = "this-is-a-very-secret-key-that-is-strong-testing"; // simpan di config di dunia nyata

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "todo-app",
            ValidAudience = "todo-app",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        };
    });

// Tambahkan layanan ke container
builder.Services.AddScoped<IStaffService, StaffService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddAutoMapper(typeof(StaffMapping));

// Tambahkan layanan otorisasi
builder.Services.AddAuthorization();
// builder.Services.AddAuthentication();

// Tambahkan controller
builder.Services.AddControllers();

// KONFIG FE
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient",
    policy =>
    {
        policy.WithOrigins("http://localhost:5173", "https://frontend-pendataan-react.vercel.app") // ganti port
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

app.UseCors("AllowBlazorClient");

// Konfigurasi Middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ActivityMiddleware>();
app.UseEndpoints(endpoints =>
{
    _ = endpoints.MapControllers();
});

app.Run();
