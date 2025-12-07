using LibraryApp.Domain;
using LibraryApp.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Services
{
    public class AuthorService : ICrudService<Author>
    {
        private readonly LibraryContext _context;
        public LibraryContext Context => _context;

        public AuthorService(LibraryContext context) => _context = context;

        public async Task AddAsync(Author entity)
        {
            _context.Authors.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author != null)
            {
                _context.Authors.Remove(author);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Author>> GetAllAsync() =>
            await _context.Authors.Include(a => a.Books).ToListAsync();

        public async Task<Author?> GetByIdAsync(int id) =>
            await _context.Authors.Include(a => a.Books).FirstOrDefaultAsync(a => a.Id == id);
        public async Task DeleteAuthorIfNoBooksAsync(int authorId)
        {
            var author = await _context.Authors.Include(a => a.Books).FirstOrDefaultAsync(a => a.Id == authorId);
            if (author != null && !author.Books.Any())
            {
                _context.Authors.Remove(author);
                await _context.SaveChangesAsync();
            }
        }
        public async Task UpdateAsync(Author entity)
        {
            _context.Authors.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
