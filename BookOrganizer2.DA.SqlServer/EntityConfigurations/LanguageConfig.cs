using BookOrganizer2.Domain.BookProfile.LanguageProfile;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookOrganizer2.DA.SqlServer.EntityConfigurations
{
    public sealed class LanguageConfig : IEntityTypeConfiguration<Language>
    {
        public void Configure(EntityTypeBuilder<Language> builder)
        {
            builder.Property(x => x.Id)
                .HasConversion(c => c.Value, g => g)
                .IsRequired();

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(32);
        }
    }
}
