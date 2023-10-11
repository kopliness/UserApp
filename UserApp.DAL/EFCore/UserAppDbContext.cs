using System.Data.Common;
using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.EFCore;

public class UserAppDbContext : DbContext
{
    public UserAppDbContext(DbContextOptions<UserAppDbContext> options) : base(options) { }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    
    public DbSet<Account> Accounts { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserAppDbContext).Assembly);
    }
}