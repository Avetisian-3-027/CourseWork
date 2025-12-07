using LibraryApp.Domain;
using Microsoft.EntityFrameworkCore;

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
            // Authors
            modelBuilder.Entity<Author>(eb =>
            {
                eb.HasKey(a => a.Id);
                eb.Property(a => a.Id)
                    .ValueGeneratedOnAdd()
                    .HasAnnotation("Sqlite:Autoincrement", true);

                eb.Property(a => a.Name)
                    .IsRequired();

                eb.HasMany(a => a.Books)
                    .WithOne(b => b.Author)
                    .HasForeignKey(b => b.AuthorId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Genres
            modelBuilder.Entity<Genre>(eb =>
            {
                eb.HasKey(g => g.Id);
                eb.Property(g => g.Id)
                    .ValueGeneratedOnAdd()
                    .HasAnnotation("Sqlite:Autoincrement", true);

                eb.Property(g => g.GenreType)
                    .IsRequired();

                eb.HasMany(g => g.Books)
                    .WithOne(b => b.Genre)
                    .HasForeignKey(b => b.GenreId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Books
            modelBuilder.Entity<Book>(eb =>
            {
                eb.HasKey(b => b.Id);
                eb.Property(b => b.Id)
                    .ValueGeneratedOnAdd()
                    .HasAnnotation("Sqlite:Autoincrement", true);

                eb.Property(b => b.Title)
                    .IsRequired();

                eb.Property(b => b.Year)
                    .IsRequired();

                eb.HasOne(b => b.Author)
                    .WithMany(a => a.Books)
                    .HasForeignKey(b => b.AuthorId);

                eb.HasOne(b => b.Genre)
                    .WithMany(g => g.Books)
                    .HasForeignKey(b => b.GenreId);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}