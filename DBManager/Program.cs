using LibraryApp.Domain;
using LibraryApp.Infrastructure;
using LibraryApp.Services;
using LibraryApp.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.IO;
using System.Linq;
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

            // явно указываем сборку миграций Ч помогает EF искать миграции в рантайме.
            var migrationsAssemblyName = Assembly.GetExecutingAssembly().GetName().Name;

            var options = new DbContextOptionsBuilder<LibraryContext>()
                .UseSqlite(connectionString, b => b.MigrationsAssembly(migrationsAssemblyName))
                .Options;

            var context = new LibraryContext(options);

            try
            {
                var migrationsAssembly = context.GetService<IMigrationsAssembly>();
                var migrations = migrationsAssembly?.Migrations;

                if (migrations == null || !migrations.Any())
                {
                    // ћиграции не найдены в рантайме Ч создаЄм схему из модели
                    context.Database.EnsureCreated();
                }
                else
                {
                    // ѕримен€ем миграции
                    context.Database.Migrate();
                }
            }
            catch (Exception ex)
            {
                // ‘оллбек: гарантируем, что Ѕƒ хот€ бы создана.
                try { context.Database.EnsureCreated(); } catch { }
                Console.Error.WriteLine($"Database initialization failed: {ex.Message}");
            }

            var bookService = new BookService(context);
            var authorService = new AuthorService(context);
            var genreService = new GenreService(context);
            var reportService = new ReportService(context);

            Application.Run(new BooksForm(bookService, authorService, genreService, reportService));
        }
    }
}