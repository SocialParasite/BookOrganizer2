using BookOrganizer2.DA.SqlServer.EntityConfigurations;
using BookOrganizer2.Domain.AuthorProfile;
using BookOrganizer2.Domain.AuthorProfile.NationalityProfile;
using BookOrganizer2.Domain.BookProfile;
using BookOrganizer2.Domain.BookProfile.FormatProfile;
using BookOrganizer2.Domain.BookProfile.GenreProfile;
using BookOrganizer2.Domain.BookProfile.LanguageProfile;
using BookOrganizer2.Domain.BookProfile.SeriesProfile;
using BookOrganizer2.Domain.PublisherProfile;
using BookOrganizer2.Domain.Reports;
using Microsoft.EntityFrameworkCore;

namespace BookOrganizer2.DA.SqlServer
{
    public class BookOrganizer2DbContext : DbContext
    {
        public string ConnectionString;

        public BookOrganizer2DbContext() { }

        public BookOrganizer2DbContext(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Nationality> Nationalities { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Format> Formats { get; set; }
        public DbSet<Series> Series { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new AuthorConfig());
            modelBuilder.ApplyConfiguration(new BookConfig());
            modelBuilder.ApplyConfiguration(new BookReadDateConfig());
            modelBuilder.ApplyConfiguration(new FormatConfig());
            modelBuilder.ApplyConfiguration(new GenreConfig());
            modelBuilder.ApplyConfiguration(new LanguageConfig());
            modelBuilder.ApplyConfiguration(new NationalityConfig());
            modelBuilder.ApplyConfiguration(new PublisherConfig());
            modelBuilder.ApplyConfiguration(new SeriesConfig());
            modelBuilder.ApplyConfiguration(new ReadOrderConfig());

            modelBuilder.Entity<AnnualBookStatisticsReport>().HasNoKey().ToView(null);
            modelBuilder.Entity<AnnualBookStatisticsInRangeReport>().HasNoKey().ToView(null);
            modelBuilder.Entity<MonthlyReadsReport>().HasNoKey().ToView(null);
            modelBuilder.Entity<BooksWithoutDescriptionReport>().HasNoKey().ToView(null);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            ConnectionString ??= ConnectivityService.GetConnectionString();

            optionsBuilder.UseSqlServer(ConnectionString);

            base.OnConfiguring(optionsBuilder);
        }
    }
}
