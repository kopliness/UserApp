using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.EntityConfiguration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Age)
            .IsRequired();
            
        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.HasIndex(u => u.Email).IsUnique();
        
        builder.HasCheckConstraint("CK_User_Age", "Age > 0");
    }
}


