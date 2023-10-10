using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.EntityConfiguration;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Name).IsRequired();

        builder.HasData(
            new Role { Id = 1, Name = "User" },
            new Role { Id = 2, Name = "Admin" },
            new Role { Id = 3, Name = "Support" },
            new Role { Id = 4, Name = "SuperAdmin" }
        );
    }
}

