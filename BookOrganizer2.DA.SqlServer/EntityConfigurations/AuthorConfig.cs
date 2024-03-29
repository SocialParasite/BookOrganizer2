﻿using BookOrganizer2.Domain.AuthorProfile;
using BookOrganizer2.Domain.Common;
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
            builder.Property(x => x.NotesOld);
            builder.HasMany(x => x.Books);

            builder.OwnsMany(
                p => p.Notes, a =>
                {
                    a.ToTable("AuthorNotes");
                    a.WithOwner().HasPrincipalKey("Id");
                    a.Property<NoteId>("Id").HasConversion(c => c.Value, g => g);
                    a.HasKey("Id");
                    a.Property(t => t.Title).HasMaxLength(64);
                });
        }
    }
}
