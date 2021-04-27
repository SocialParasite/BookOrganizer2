﻿// <auto-generated />
using System;
using BookOrganizer2.DA.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BookOrganizer2.DA.SqlServer.Migrations
{
    [DbContext(typeof(BookOrganizer2DbContext))]
    [Migration("20210426151102_GetBooksWithoutDescriptionSP")]
    partial class GetBooksWithoutDescriptionSP
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.2");

            modelBuilder.Entity("AuthorBook", b =>
                {
                    b.Property<Guid>("AuthorsId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("BooksId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("AuthorsId", "BooksId");

                    b.HasIndex("BooksId");

                    b.ToTable("AuthorBook");
                });

            modelBuilder.Entity("BookFormat", b =>
                {
                    b.Property<Guid>("BooksId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("FormatsId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("BooksId", "FormatsId");

                    b.HasIndex("FormatsId");

                    b.ToTable("BookFormat");
                });

            modelBuilder.Entity("BookGenre", b =>
                {
                    b.Property<Guid>("BooksId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("GenresId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("BooksId", "GenresId");

                    b.HasIndex("GenresId");

                    b.ToTable("BookGenre");
                });

            modelBuilder.Entity("BookOrganizer2.Domain.AuthorProfile.Author", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Biography")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("DateOfBirth")
                        .HasColumnType("date");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("MugshotPath")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<Guid?>("NationalityId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Notes")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("NationalityId");

                    b.ToTable("Authors");
                });

            modelBuilder.Entity("BookOrganizer2.Domain.AuthorProfile.NationalityProfile.Nationality", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.HasKey("Id");

                    b.ToTable("Nationalities");
                });

            modelBuilder.Entity("BookOrganizer2.Domain.BookProfile.Book", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("BookCoverPath")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsRead")
                        .HasColumnType("bit");

                    b.Property<string>("Isbn")
                        .HasMaxLength(13)
                        .HasColumnType("nvarchar(13)");

                    b.Property<Guid?>("LanguageId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Notes")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PageCount")
                        .HasMaxLength(5)
                        .HasColumnType("int");

                    b.Property<Guid?>("PublisherId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("ReleaseYear")
                        .HasMaxLength(4)
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<int>("WordCount")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("LanguageId");

                    b.HasIndex("PublisherId");

                    b.ToTable("Books");
                });

            modelBuilder.Entity("BookOrganizer2.Domain.BookProfile.BookReadDate", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("BookId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("ReadDate")
                        .HasColumnType("Date");

                    b.HasKey("Id");

                    b.HasIndex("BookId");

                    b.ToTable("BookReadDate");
                });

            modelBuilder.Entity("BookOrganizer2.Domain.BookProfile.FormatProfile.Format", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.HasKey("Id");

                    b.ToTable("Formats");
                });

            modelBuilder.Entity("BookOrganizer2.Domain.BookProfile.GenreProfile.Genre", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.HasKey("Id");

                    b.ToTable("Genres");
                });

            modelBuilder.Entity("BookOrganizer2.Domain.BookProfile.LanguageProfile.Language", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.HasKey("Id");

                    b.ToTable("Languages");
                });

            modelBuilder.Entity("BookOrganizer2.Domain.BookProfile.ReadOrder", b =>
                {
                    b.Property<Guid>("BooksId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("SeriesId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Instalment")
                        .HasColumnType("int");

                    b.HasKey("BooksId", "SeriesId");

                    b.HasIndex("SeriesId");

                    b.ToTable("BooksSeries");
                });

            modelBuilder.Entity("BookOrganizer2.Domain.BookProfile.SeriesProfile.Series", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PicturePath")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.ToTable("Series");
                });

            modelBuilder.Entity("BookOrganizer2.Domain.PublisherProfile.Publisher", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LogoPath")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.HasKey("Id");

                    b.ToTable("Publishers");
                });

            modelBuilder.Entity("BookOrganizer2.Domain.Reports.AnnualBookStatisticsInRangeReport", b =>
                {
                    b.Property<int>("AverageBookLength")
                        .HasColumnType("int");

                    b.Property<int>("AveragePagesReadMonthly")
                        .HasColumnType("int");

                    b.Property<int>("LongestBookRead")
                        .HasColumnType("int");

                    b.Property<int>("ShortestBookRead")
                        .HasColumnType("int");

                    b.Property<int>("TotalNumberOfBooksRead")
                        .HasColumnType("int");

                    b.Property<int>("TotalPagesRead")
                        .HasColumnType("int");

                    b.Property<int>("Year")
                        .HasColumnType("int");
                });

            modelBuilder.Entity("BookOrganizer2.Domain.Reports.AnnualBookStatisticsReport", b =>
                {
                    b.Property<int>("AverageBookLength")
                        .HasColumnType("int");

                    b.Property<int>("AveragePagesReadDaily")
                        .HasColumnType("int");

                    b.Property<int>("LongestBookRead")
                        .HasColumnType("int");

                    b.Property<string>("MonthName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ShortestBookRead")
                        .HasColumnType("int");

                    b.Property<int>("TotalNumberOfBooksRead")
                        .HasColumnType("int");

                    b.Property<int>("TotalPagesRead")
                        .HasColumnType("int");
                });

            modelBuilder.Entity("BookOrganizer2.Domain.Reports.MonthlyReadsReport", b =>
                {
                    b.Property<DateTime>("ReadDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");
                });

            modelBuilder.Entity("AuthorBook", b =>
                {
                    b.HasOne("BookOrganizer2.Domain.AuthorProfile.Author", null)
                        .WithMany()
                        .HasForeignKey("AuthorsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BookOrganizer2.Domain.BookProfile.Book", null)
                        .WithMany()
                        .HasForeignKey("BooksId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BookFormat", b =>
                {
                    b.HasOne("BookOrganizer2.Domain.BookProfile.Book", null)
                        .WithMany()
                        .HasForeignKey("BooksId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BookOrganizer2.Domain.BookProfile.FormatProfile.Format", null)
                        .WithMany()
                        .HasForeignKey("FormatsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BookGenre", b =>
                {
                    b.HasOne("BookOrganizer2.Domain.BookProfile.Book", null)
                        .WithMany()
                        .HasForeignKey("BooksId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BookOrganizer2.Domain.BookProfile.GenreProfile.Genre", null)
                        .WithMany()
                        .HasForeignKey("GenresId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BookOrganizer2.Domain.AuthorProfile.Author", b =>
                {
                    b.HasOne("BookOrganizer2.Domain.AuthorProfile.NationalityProfile.Nationality", "Nationality")
                        .WithMany("Authors")
                        .HasForeignKey("NationalityId");

                    b.Navigation("Nationality");
                });

            modelBuilder.Entity("BookOrganizer2.Domain.BookProfile.Book", b =>
                {
                    b.HasOne("BookOrganizer2.Domain.BookProfile.LanguageProfile.Language", "Language")
                        .WithMany()
                        .HasForeignKey("LanguageId");

                    b.HasOne("BookOrganizer2.Domain.PublisherProfile.Publisher", "Publisher")
                        .WithMany("Books")
                        .HasForeignKey("PublisherId");

                    b.Navigation("Language");

                    b.Navigation("Publisher");
                });

            modelBuilder.Entity("BookOrganizer2.Domain.BookProfile.BookReadDate", b =>
                {
                    b.HasOne("BookOrganizer2.Domain.BookProfile.Book", null)
                        .WithMany("ReadDates")
                        .HasForeignKey("BookId");
                });

            modelBuilder.Entity("BookOrganizer2.Domain.BookProfile.ReadOrder", b =>
                {
                    b.HasOne("BookOrganizer2.Domain.BookProfile.Book", "Book")
                        .WithMany("Series")
                        .HasForeignKey("BooksId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BookOrganizer2.Domain.BookProfile.SeriesProfile.Series", "Series")
                        .WithMany("Books")
                        .HasForeignKey("SeriesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Book");

                    b.Navigation("Series");
                });

            modelBuilder.Entity("BookOrganizer2.Domain.AuthorProfile.NationalityProfile.Nationality", b =>
                {
                    b.Navigation("Authors");
                });

            modelBuilder.Entity("BookOrganizer2.Domain.BookProfile.Book", b =>
                {
                    b.Navigation("ReadDates");

                    b.Navigation("Series");
                });

            modelBuilder.Entity("BookOrganizer2.Domain.BookProfile.SeriesProfile.Series", b =>
                {
                    b.Navigation("Books");
                });

            modelBuilder.Entity("BookOrganizer2.Domain.PublisherProfile.Publisher", b =>
                {
                    b.Navigation("Books");
                });
#pragma warning restore 612, 618
        }
    }
}
