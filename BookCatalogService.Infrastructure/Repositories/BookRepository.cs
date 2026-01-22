using BookCatalogService.Domain.Entities;
using BookCatalogService.Domain.Interfaces;
using BookCatalogService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookCatalogService.Infrastructure.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly BookCatalogDbContext _context;

        public BookRepository ( BookCatalogDbContext context )
        {
            _context = context;
        }

        public async Task<IEnumerable<Book>> GetAllAsync ( )
        {
            return await _context.Books
                .Include ( b => b.Author )
                .Include ( b => b.Category )
                .Include ( b => b.Publisher )
                .AsNoTracking ()
                .ToListAsync ();
        }

        public async Task<Book?> GetByIdAsync ( int id )
        {
            return await _context.Books
                .Include ( b => b.Author )
                .Include ( b => b.Category )
                .Include ( b => b.Publisher )
                .FirstOrDefaultAsync ( x => x.Id == id );
        }

        public async Task AddAsync ( Book book )
        {
            await _context.Books.AddAsync ( book );
        }

        public async Task UpdateAsync ( Book book )
        {
            _context.Books.Update ( book );
        }

        public async Task DeleteAsync ( int id )
        {
            var book = await _context.Books.FindAsync ( id );

            if (book != null)
                _context.Books.Remove ( book );
        }

        public async Task<IEnumerable<Book>> SearchAsync ( string keyword )
        {
            keyword = keyword.ToLower ();

            return await _context.Books
                .Where ( b =>
                       b.Title.ToLower ().Contains ( keyword ) ||
                       b.Author.Name.ToLower ().Contains ( keyword ) ||
                       b.Category.Name.ToLower ().Contains ( keyword ) ||
                       b.ISBN.Contains ( keyword ) )
                .Include ( b => b.Author )
                .Include ( b => b.Category )
                .Include ( b => b.Publisher )
                .AsNoTracking ()
                .ToListAsync ();
        }

        public async Task<bool> ExistsAsync ( int id )
        {
            return await _context.Books.AnyAsync ( x => x.Id == id );
        }

        public async Task<bool> ExistsByISBNAsync ( string isbn13 )
        {
            return await _context.Books.AnyAsync ( b => b.ISBN13 == isbn13 );
        }

        public async Task SaveChangesAsync ( )
        {
            await _context.SaveChangesAsync ();
        }
    }
}
