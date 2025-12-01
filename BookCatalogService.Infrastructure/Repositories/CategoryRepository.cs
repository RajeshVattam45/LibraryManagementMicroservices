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
    public class CategoryRepository : ICategoryRepository
    {
        private readonly BookCatalogDbContext _context;

        public CategoryRepository ( BookCatalogDbContext context )
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllAsync ( )
        {
            return await _context.Categories
                .AsNoTracking ()
                .ToListAsync ();
        }

        public async Task<Category?> GetByIdAsync ( int id )
        {
            return await _context.Categories
                .Include ( c => c.ParentCategory )
                .Include ( c => c.Books )
                .FirstOrDefaultAsync ( c => c.Id == id );
        }

        public async Task AddAsync ( Category category )
        {
            await _context.Categories.AddAsync ( category );
        }

        public async Task UpdateAsync ( Category category )
        {
            category.UpdatedAt = DateTime.UtcNow;
            _context.Categories.Update ( category );
        }

        public async Task DeleteAsync ( int id )
        {
            var category = await _context.Categories.FindAsync ( id );
            if (category != null)
            {
                _context.Categories.Remove ( category );
            }
        }

        public async Task<bool> ExistsAsync ( int id )
        {
            return await _context.Categories.AnyAsync ( c => c.Id == id );
        }

        public async Task<IEnumerable<Category>> SearchAsync ( string keyword )
        {
            keyword = keyword.ToLower ();

            return await _context.Categories
                .Where ( c =>
                    c.Name.ToLower ().Contains ( keyword ) ||
                    c.Code.ToLower ().Contains ( keyword ) ||
                    c.Description.ToLower ().Contains ( keyword ) )
                .AsNoTracking ()
                .ToListAsync ();
        }

        public async Task SaveChangesAsync ( )
        {
            await _context.SaveChangesAsync ();
        }
    }
}
