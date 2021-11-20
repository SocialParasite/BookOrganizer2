using BookOrganizer2.Domain.BookProfile;
using BookOrganizer2.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookOrganizer2.DA.SqlServer.EntityConfigurations
{
    public sealed class BookConfig : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.Property(x => x.Id)
                .HasConversion(c => c.Value, g => g)
                .IsRequired();

            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(x => x.ReleaseYear)
                .HasMaxLength(4);

            builder.Property(x => x.PageCount)
                .HasMaxLength(5);
            builder.Property(x => x.WordCount);
            builder.Property(x => x.Isbn)
                .HasMaxLength(13);
            builder.Property(x => x.BookCoverPath)
                .HasMaxLength(256);
            builder.Property(x => x.Description);
            builder.Property(x => x.NotesOld);
            builder.Property(x => x.IsRead)
                .HasColumnType("bit");

            builder.HasOne(x => x.Language);
            builder.HasOne(x => x.Publisher);

            builder.HasMany(x => x.Authors);
            builder.HasMany(x => x.Genres);
            builder.HasMany(x => x.Formats);
            builder.HasMany(x => x.ReadDates);

            builder.OwnsMany(
                p => p.Notes,
                a =>
                {
                    a.ToTable("BookNotes");
                    a.WithOwner().HasPrincipalKey("Id");
                    a.Property<NoteId>("Id").HasConversion(c => c.Value, g => g);
                    a.HasKey("Id");
                    a.Property(t => t.Title).HasMaxLength(64);
                });
        }
    }
}
