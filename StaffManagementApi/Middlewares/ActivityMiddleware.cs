using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using StaffManagementApi.Data; // ganti sesuai namespace

public class ActivityMiddleware
{

    private readonly RequestDelegate _next;
    private readonly IServiceProvider _serviceProvider;

    public ActivityMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
    {
        _next = next;
        _serviceProvider = serviceProvider;
    }

    public async Task Invoke(HttpContext context)
    {
        // 1️⃣ Skip middleware untuk endpoint public
        var path = context.Request.Path.Value?.ToLower();
        Console.WriteLine($"[ActivityMiddleware] Path: {context.Request.Path}");
        // Skip middleware kalau belum ada Authorization header
        if (!context.Request.Headers.ContainsKey("Authorization"))
        {
            await _next(context);
            return;
        }
        if (path != null &&
            (path.StartsWith("api/auth/login") || path.StartsWith("api/auth/register")))
        {
            await _next(context);
            return;
        }

        // 2️⃣ Buat scope untuk mengambil scoped service (AppDbContext)
        using (var scope = _serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // 3️⃣ Ambil user ID dari JWT Claims
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                await _next(context);
                return;
            }

            if (!int.TryParse(userIdClaim.Value, out var userId))
            {
                await _next(context);
                return;
            }

            // 4️⃣ Ambil user dari database
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null)
            {
                await _next(context);
                return;
            }

            // 5️⃣ Cek idle timeout (misalnya 1 menit untuk testing)
            // Hanya cek kalau LastActivity tidak null
            if (user.LastActivity != null &&
                DateTime.UtcNow - user.LastActivity > TimeSpan.FromHours(1))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Session expired due to inactivity");
                return;
            }

            // 6️⃣ Update LastActivity ke waktu sekarang
            user.LastActivity = DateTime.UtcNow;
            await dbContext.SaveChangesAsync();
        }

        // 7️⃣ Lanjut ke middleware berikutnya
        await _next(context);
    }
    
}
