using Microsoft.EntityFrameworkCore;
using StaffManagementApi.Data;

namespace StaffManagementApi
{
    public static class DataExtensions
    {
        public static IServiceCollection AddDatabaseServices(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));

            return services;
        }
    }
}
