using LibraryCatalog.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryCatalog.Data
{
    public class LibraryCatalogDbContext : DbContext
    {
        public LibraryCatalogDbContext(DbContextOptions<LibraryCatalogDbContext> options) : base(options)
        {
        }

        public virtual DbSet<Author> Authors { get; set; }

        public virtual DbSet<Publication> Publications { get; set; }

        public virtual DbSet<Publisher> Publishers { get; set; }

        public virtual DbSet<Review> Reviews { get; set; }
    }
}