using BookOrganizer2.Domain.BookProfile;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookOrganizer2.DA.SqlServer.EntityConfigurations
{
    public sealed class BookReadDateConfig : IEntityTypeConfiguration<BookReadDate>
    {
        public void Configure(EntityTypeBuilder<BookReadDate> builder)
        {
            builder.Property(x => x.Id)
                .HasConversion(c => c.Value, g => g)
                .IsRequired();

            builder.Property(x => x.ReadDate)
                .IsRequired()
                .HasColumnType("Date");
        }
    }
}
