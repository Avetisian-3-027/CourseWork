using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace LibraryApp.Migrations
{
    [DbContext(typeof(LibraryApp.Infrastructure.LibraryContext))]
    partial class LibraryContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "EFCore")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            modelBuilder.Entity("LibraryApp.Domain.Author", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasAnnotation("Sqlite:Autoincrement", true);

                b.Property<string>("Name")
                    .IsRequired();

                b.HasKey("Id");

                b.ToTable("Authors");
            });

            modelBuilder.Entity("LibraryApp.Domain.Genre", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasAnnotation("Sqlite:Autoincrement", true);

                b.Property<string>("GenreType")
                    .IsRequired();

                b.HasKey("Id");

                b.ToTable("Genres");
            });

            modelBuilder.Entity("LibraryApp.Domain.Book", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasAnnotation("Sqlite:Autoincrement", true);

                b.Property<int>("AuthorId");

                b.Property<int>("GenreId");

                b.Property<string>("Title")
                    .IsRequired();

                b.Property<int>("Year")
                    .IsRequired();

                b.HasKey("Id");

                b.HasIndex("AuthorId");

                b.HasIndex("GenreId");

                b.ToTable("Books");
            });

            modelBuilder.Entity("LibraryApp.Domain.Book", b =>
            {
                b.HasOne("LibraryApp.Domain.Author", "Author")
                    .WithMany("Books")
                    .HasForeignKey("AuthorId")
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne("LibraryApp.Domain.Genre", "Genre")
                    .WithMany("Books")
                    .HasForeignKey("GenreId")
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
