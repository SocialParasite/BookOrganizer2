using BookOrganizer2.Domain.BookProfile.GenreProfile;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookOrganizer2.DA.SqlServer.EntityConfigurations
{
    public sealed class GenreConfig : IEntityTypeConfiguration<Genre>
    {
        public void Configure(EntityTypeBuilder<Genre> builder)
        {
            builder.Property(x => x.Id)
                .HasConversion(c => c.Value, g => g)
                .IsRequired();

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(32);

            builder.HasMany(x => x.Books);
        }
    }
}
