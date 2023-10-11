using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.EntityConfiguration;

public class AccountConfiguration: IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasKey(login => login.Login);

        builder.Property(login => login.Login)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(password => password.Password)
            .IsRequired()
            .HasMaxLength(30);
    }
}