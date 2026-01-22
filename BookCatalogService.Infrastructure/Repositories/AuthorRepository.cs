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
    public class AuthorRepository : IAuthorRepository
    {
        private readonly BookCatalogDbContext _context;

        public AuthorRepository ( BookCatalogDbContext context )
        {
            _context = context;
        }

        public async Task<IEnumerable<Author>> GetAllAsync ( )
        {
            return await _context.Authors
                .AsNoTracking ()
                .ToListAsync ();
        }

        public async Task<Author?> GetByIdAsync ( int id )
        {
            return await _context.Authors
                .Include ( a => a.Books )
                .FirstOrDefaultAsync ( a => a.Id == id );
        }

        public async Task AddAsync ( Author author )
        {
            await _context.Authors.AddAsync ( author );
        }

        public async Task UpdateAsync ( Author author )
        {
            author.UpdatedAt = DateTime.UtcNow;
            _context.Authors.Update ( author );
        }

        public async Task DeleteAsync ( int id )
        {
            var author = await _context.Authors.FindAsync ( id );
            if (author != null)
            {
                _context.Authors.Remove ( author );
            }
        }

        public async Task<bool> ExistsAsync ( int id )
        {
            return await _context.Authors.AnyAsync ( a => a.Id == id );
        }

        public async Task<IEnumerable<Author>> SearchAsync ( string keyword )
        {
            keyword = keyword.ToLower ();

            return await _context.Authors
                .Where ( a =>
                    a.Name.ToLower ().Contains ( keyword ) ||
                    a.Nationality.ToLower ().Contains ( keyword ) )
                .AsNoTracking ()
                .ToListAsync ();
        }

        public async Task<bool> ExistsByNameAsync ( string name )
        {
            return await _context.Authors.AnyAsync ( a => a.Name.ToLower () == name.ToLower () );
        }

        public async Task SaveChangesAsync ( )
        {
            await _context.SaveChangesAsync ();
        }
    }
}
