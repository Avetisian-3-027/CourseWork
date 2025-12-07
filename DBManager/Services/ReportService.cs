using LibraryApp.Domain;
using LibraryApp.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Services
{
    public class ReportService
    {
        private readonly LibraryContext _context;

        public ReportService(LibraryContext context)
        {
            _context = context;
        }

        public async Task<Dictionary<string, int>> GetBooksCountByGenreAsync()
        {
            return await _context.Genres
                .AsNoTracking()
                .Select(g => new
                {
                    g.GenreType,
                    Count = g.Books.Count
                })
                .OrderByDescending(x => x.Count)
                .ToDictionaryAsync(x => x.GenreType, x => x.Count);
        }

        public async Task<Author?> GetAuthorWithMostBooksAsync()
        {
            return await _context.Authors
                .AsNoTracking()
                .Include(a => a.Books)
                .OrderByDescending(a => a.Books.Count)
                .FirstOrDefaultAsync();
        }

        public async Task<Book?> GetOldestBookAsync()
        {
            return await _context.Books
                .AsNoTracking()
                .Include(b => b.Author)
                .OrderBy(b => b.Year)
                .FirstOrDefaultAsync();
        }
    }
}
