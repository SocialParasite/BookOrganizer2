using BookOrganizer2.Domain.AuthorProfile;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookOrganizer2.DA.SqlServer.EntityConfigurations
{
    public sealed class AuthorConfig : IEntityTypeConfiguration<Author>
    {
        public void Configure(EntityTypeBuilder<Author> builder)
        {
            builder.Property(x => x.Id)
                .HasConversion(c => c.Value, g => g)
                .IsRequired();

            builder.Property(x => x.FirstName)
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(x => x.LastName)
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(x => x.DateOfBirth)
                .HasColumnType("date");

            builder.Property(x => x.MugshotPath)
                .HasMaxLength(256);

            builder.HasMany(x => x.Books);
        }
    }
}
