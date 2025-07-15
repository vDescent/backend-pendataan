using Microsoft.AspNetCore.Mvc;
using StaffManagementApi.Data;
using StaffManagementApi.Entities;
using StaffManagementApi.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;



[ApiController]
[Route("api/auth")]

public class AuthEndpoints : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly PasswordHasher<User> _hasher = new();

    public AuthEndpoints(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("register")]
    public IActionResult Register(RegisterDto dto)
    {
        if (dto.Password != dto.ConfirmPassword)
            return BadRequest("Password dan konfirmasi tidak sama.");

        if (_context.Users.Any(u => u.Email == dto.Email))
            return BadRequest("Email sudah digunakan.");

        var user = new User
        {
            Name = dto.Nama,
            Email = dto.Email
        };

        user.PasswordHash = _hasher.HashPassword(user, dto.Password);

        _context.Users.Add(user);
        _context.SaveChanges();

        return Ok("Berhasil register.");
    }

    [HttpPost("login")]
    public IActionResult Login(LoginDto dto)
    {
        
        var user = _context.Users.SingleOrDefault(u => u.Email == dto.Email);
        if (user == null) return Unauthorized("Email tidak ditemukan.");

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        if (result == PasswordVerificationResult.Failed)
            return Unauthorized("Password salah.");

        // Buat JWT
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("this-is-a-very-secret-key-that-is-strong-testing"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: "todo-app",
            audience: "todo-app",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(3),
            signingCredentials: creds
        );

        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token)
        });
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult GetProfile()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;

        return Ok(new
        {
            Message = "Kamu berhasil masuk",
            UserId = userId,
            Email = email
        });
    }
}
