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
        // 1. Validasi Nama (username)
        if (string.IsNullOrWhiteSpace(dto.Nama) || dto.Nama.Length < 8 || !dto.Nama.All(char.IsLetter))
            return BadRequest("Nama minimal 8 huruf dan tidak boleh mengandung angka.");

        // 2. Validasi Email
        if (string.IsNullOrWhiteSpace(dto.Email) ||
            !(dto.Email.EndsWith("@binus.ac.id", StringComparison.OrdinalIgnoreCase) ||
            dto.Email.EndsWith("@binus.edu", StringComparison.OrdinalIgnoreCase)))
            return BadRequest("Email harus menggunakan domain @binus.ac.id atau @binus.edu.");

        // 3. Validasi Password
        if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 8)
            return BadRequest("Password minimal 8 karakter.");

        // 4. Cek confrim password
        if (dto.Password != dto.ConfirmPassword)
            return BadRequest("Password dan konfirmasi tidak sama.");

        // 5. Cek apakah email sudah digunakan
        if (_context.Users.Any(u => u.Email == dto.Email))
            return BadRequest("Email sudah digunakan.");

        // 6. Simpan user
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
        // 1. Cek format email
        if (string.IsNullOrWhiteSpace(dto.Email) ||
            !(dto.Email.EndsWith("@binus.ac.id", StringComparison.OrdinalIgnoreCase) ||
            dto.Email.EndsWith("@binus.edu", StringComparison.OrdinalIgnoreCase)))
        {
            return BadRequest("Email harus menggunakan domain @binus.ac.id atau @binus.edu.");
        }

        // 2. Cek panjang password
        if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 8)
        {
            return BadRequest("Password minimal 8 karakter.");
        }

        // 3. Cek apakah email ada di database
        var user = _context.Users.SingleOrDefault(u => u.Email == dto.Email);
        if (user == null)
        {
            return Unauthorized("Email tidak ditemukan.");
        }

        // 4. Verifikasi password
        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            return Unauthorized("Password salah.");
        }

        user.LastActivity = DateTime.UtcNow;
        _context.SaveChanges();

        // 5. Buat JWT token jika semua validasi lolos
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name) 
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("this-is-a-very-secret-key-that-is-strong-testing"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: "todo-app",
            audience: "todo-app",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
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

        if (userId == null) return Unauthorized();

        var user = _context.Users.SingleOrDefault(u => u.Id.ToString() == userId);
        if (user == null) return NotFound("User tidak ditemukan");

        return Ok(new
        {
            Message = "Kamu berhasil masuk",
            UserId = user.Id,
            Email = user.Email,
            Name = user.Name,
        });
    }
}
