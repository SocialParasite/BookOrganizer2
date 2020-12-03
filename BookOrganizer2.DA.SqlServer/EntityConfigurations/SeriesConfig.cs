﻿using BookOrganizer2.Domain.BookProfile;
using BookOrganizer2.Domain.BookProfile.SeriesProfile;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookOrganizer2.DA.SqlServer.EntityConfigurations
{
    public sealed class SeriesConfig : IEntityTypeConfiguration<Series>
    {
        public void Configure(EntityTypeBuilder<Series> builder)
        {
            builder.Property(x => x.Id)
                .HasConversion(c => c.Value, g => g)
                .IsRequired();

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(x => x.PicturePath)
                .HasMaxLength(256);

            builder.Property(x => x.Description);

            builder.HasMany(x => x.Books)
                .WithMany(s => s.Series)
                .UsingEntity<ReadOrder>(e => e.HasOne<Book>().WithMany(),
                    e => e.HasOne<Series>().WithMany())
                .ToTable("BooksSeries")
                .Property(x => x.Instalment);
        }
    }
}
