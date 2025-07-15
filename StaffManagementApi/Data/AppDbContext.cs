using StaffManagementApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace StaffManagementApi.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) 
    : DbContext(options)
{
    public DbSet<Staff> Staffs => Set<Staff>();
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Staff>().HasIndex(s => s.BinusianId).IsUnique();
    }
}

