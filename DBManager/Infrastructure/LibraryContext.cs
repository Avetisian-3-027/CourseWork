using System;
using System.IO;
using System.Reflection;
using LibraryApp.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LibraryApp.Infrastructure
{
    public class LibraryContext : DbContext
    {
        public LibraryContext(DbContextOptions<LibraryContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; } = null!;
        public DbSet<Author> Authors { get; set; } = null!;
        public DbSet<Genre> Genres { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Author>(eb =>
            {
                eb.HasKey(a => a.Id);
                eb.Property(a => a.Id)
                  .ValueGeneratedOnAdd();
                eb.Property(a => a.Name).IsRequired();
            });

            modelBuilder.Entity<Genre>(eb =>
            {
                eb.HasKey(g => g.Id);
                eb.Property(g => g.Id)
                  .ValueGeneratedOnAdd();
                eb.Property(g => g.GenreType).IsRequired();
            });

            modelBuilder.Entity<Book>(eb =>
            {
                eb.HasKey(b => b.Id);
                eb.Property(b => b.Id)
                  .ValueGeneratedOnAdd();

                eb.Property(b => b.Title).IsRequired();

                eb.Property(b => b.Year)
                    .IsRequired()
                    .HasColumnType("INTEGER");

                eb.HasOne(b => b.Author)
                  .WithMany(a => a.Books)
                  .HasForeignKey(b => b.AuthorId)
                  .OnDelete(DeleteBehavior.Cascade);

                eb.HasOne(b => b.Genre)
                  .WithMany(g => g.Books)
                  .HasForeignKey(b => b.GenreId)
                  .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }

}
