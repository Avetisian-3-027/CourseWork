using LibraryApp.Domain;
using LibraryApp.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Services
{
    public class GenreService : ICrudService<Genre>
    {
        private readonly LibraryContext _context;
        public LibraryContext Context => _context;

        public GenreService(LibraryContext context) => _context = context;

        public async Task AddAsync(Genre entity)
        {
            _context.Genres.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var genre = await _context.Genres.FindAsync(id);
            if (genre != null)
            {
                _context.Genres.Remove(genre);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Genre>> GetAllAsync() =>
            await _context.Genres.Include(a => a.Books).ToListAsync();

        public async Task<Genre?> GetByIdAsync(int id) =>
            await _context.Genres.Include(a => a.Books).FirstOrDefaultAsync(a => a.Id == id);
        public async Task DeleteGenreIfNoBooksAsync(int genreId)
        {
            var genre = await _context.Genres.Include(g => g.Books).FirstOrDefaultAsync(g => g.Id == genreId);
            if (genre != null && !genre.Books.Any())
            {
                _context.Genres.Remove(genre);
                await _context.SaveChangesAsync();
            }
        }
        public async Task UpdateAsync(Genre entity)
        {
            _context.Genres.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
