using ExpenseTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracker.Infrastructure.Persistence.Configurations;

public class UserCredentialsEntityTypeConfiguration : IEntityTypeConfiguration<UserCredentials>
{
    public void Configure(EntityTypeBuilder<UserCredentials> builder)
    {
        builder.HasKey(uc => uc.Id);
        builder.Property(uc => uc.PasswordHash)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasOne<User>()
            .WithOne()
            .HasForeignKey<UserCredentials>(uc => uc.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}