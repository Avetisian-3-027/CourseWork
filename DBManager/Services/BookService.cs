using LibraryApp.Domain;
using LibraryApp.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryApp.Services
{
    public class BookService : ICrudService<Book>
    {
        private readonly LibraryContext _context;
        public LibraryContext Context => _context;

        public BookService(LibraryContext context) => _context = context;

        public async Task AddAsync(Book entity)
        {
            _context.Books.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var book = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
                return;

            var author = book.Author;
            var genre = book.Genre;

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            if (!await _context.Books.AnyAsync(b => b.AuthorId == author.Id))
                _context.Authors.Remove(author);

            if (!await _context.Books.AnyAsync(b => b.GenreId == genre.Id))
                _context.Genres.Remove(genre);

            await _context.SaveChangesAsync();
        }

        public async Task<List<Book>> GetAllAsync() =>
            await _context.Books.Include(b => b.Author).ToListAsync();
            

        public async Task<Book?> GetByIdAsync(int id) =>
            await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .FirstOrDefaultAsync(b => b.Id == id);

        public async Task UpdateAsync(Book entity)
        {
            _context.Books.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
