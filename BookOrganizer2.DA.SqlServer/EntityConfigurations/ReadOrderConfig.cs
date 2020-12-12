using BookOrganizer2.Domain.BookProfile;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookOrganizer2.DA.SqlServer.EntityConfigurations
{
    public sealed class ReadOrderConfig : IEntityTypeConfiguration<ReadOrder>
    {
        public void Configure(EntityTypeBuilder<ReadOrder> builder)
        {
            builder.ToTable("BooksSeries");

            builder.HasKey(r => new { r.BooksId, r.SeriesId });

            builder.HasOne(b => b.Book)
                .WithMany(bs => bs.Series)
                .HasForeignKey(b => b.BooksId);

            builder.HasOne(s => s.Series)
                .WithMany(bs => bs.Books)
                .HasForeignKey(s => s.SeriesId);
        }
    }
}
