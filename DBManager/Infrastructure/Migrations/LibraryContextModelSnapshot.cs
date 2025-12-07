using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using LibraryApp.Infrastructure;

#nullable disable

namespace LibraryApp.Infrastructure.Migrations
{
    [DbContext(typeof(LibraryContext))]
    partial class LibraryContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "EFCore");

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

                b.Property<int>("Year");

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
#pragma warning restore 612, 618
        }
    }
}