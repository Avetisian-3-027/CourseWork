using LibraryApp.Domain;
using LibraryApp.Infrastructure;
using LibraryApp.Services;
using LibraryApp.UI;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace LibraryApp
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            string dataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
            if (!Directory.Exists(dataFolder))
                Directory.CreateDirectory(dataFolder);

            string databasePath = Path.Combine(dataFolder, "Library.db");
            string connectionString = $"Data Source={databasePath}";

            
            var migrationsAssemblyName = typeof(LibraryContext).Assembly.GetName().Name;

            var options = new DbContextOptionsBuilder<LibraryContext>()
                .UseSqlite(connectionString, b => b.MigrationsAssembly(migrationsAssemblyName))
                .Options;

            using var context = new LibraryContext(options);

            try
            {
           
                context.Database.Migrate();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Migrate failed: {ex.Message}. Falling back to EnsureCreated().");
                try { context.Database.EnsureCreated(); } catch (Exception inner) { Console.Error.WriteLine($"EnsureCreated failed: {inner.Message}"); }
            }

            var bookService = new BookService(context);
            var authorService = new AuthorService(context);
            var genreService = new GenreService(context);
            var reportService = new ReportService(context);

            Application.Run(new BooksForm(bookService, authorService, genreService, reportService));
        }
    }
}
