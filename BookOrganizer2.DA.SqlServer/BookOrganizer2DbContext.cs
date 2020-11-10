using BookOrganizer2.DA.SqlServer.EntityConfigurations;
using BookOrganizer2.Domain.AuthorProfile;
using BookOrganizer2.Domain.AuthorProfile.NationalityProfile;
using BookOrganizer2.Domain.PublisherProfile;
using Microsoft.EntityFrameworkCore;

namespace BookOrganizer2.DA.SqlServer
{
    public class BookOrganizer2DbContext : DbContext
    {
        private string _connectionString;

        public BookOrganizer2DbContext() { }

        public BookOrganizer2DbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DbSet<Author> Authors { get; set; }
        public DbSet<Nationality> Nationalities { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new AuthorConfig());
            modelBuilder.ApplyConfiguration(new NationalityConfig());
            modelBuilder.ApplyConfiguration(new PublisherConfig());
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            _connectionString ??= ConnectivityService.GetConnectionString();

            optionsBuilder.UseSqlServer(_connectionString);

            base.OnConfiguring(optionsBuilder);
        }
    }
}
