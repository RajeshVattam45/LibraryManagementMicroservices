using BookCatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookCatalogService.Infrastructure.Data
{
    public class BookCatalogDbContext : DbContext
    {
        public BookCatalogDbContext ( DbContextOptions<BookCatalogDbContext> options )
            : base ( options )
        {
        }

        // DbSets (Tables)
        public DbSet<Book> Books { get; set; } = null!;
        public DbSet<Author> Authors { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Publisher> Publishers { get; set; } = null!;

        protected override void OnModelCreating ( ModelBuilder modelBuilder )
        {
            base.OnModelCreating ( modelBuilder );

            // Example constraints & configurations (optional but recommended)

            modelBuilder.Entity<Book> ()
                .HasIndex ( b => b.ISBN )
                .IsUnique ();

            modelBuilder.Entity<Category> ()
                .HasOne ( c => c.ParentCategory )
                .WithMany ()
                .HasForeignKey ( c => c.ParentCategoryId )
                .OnDelete ( DeleteBehavior.Restrict );
        }
    }
}
