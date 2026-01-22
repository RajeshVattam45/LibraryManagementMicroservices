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
    public class PublisherRepository : IPublisherRepository
    {
        private readonly BookCatalogDbContext _context;

        public PublisherRepository ( BookCatalogDbContext context )
        {
            _context = context;
        }

        public async Task<IEnumerable<Publisher>> GetAllAsync ( )
        {
            return await _context.Publishers
                .AsNoTracking ()
                .ToListAsync ();
        }

        public async Task<Publisher?> GetByIdAsync ( int id )
        {
            return await _context.Publishers
                .Include ( p => p.Books )
                .FirstOrDefaultAsync ( p => p.Id == id );
        }

        public async Task AddAsync ( Publisher publisher )
        {
            await _context.Publishers.AddAsync ( publisher );
        }

        public async Task UpdateAsync ( Publisher publisher )
        {
            publisher.UpdatedAt = DateTime.UtcNow;
            _context.Publishers.Update ( publisher );
        }

        public async Task DeleteAsync ( int id )
        {
            var publisher = await _context.Publishers.FindAsync ( id );
            if (publisher != null)
                _context.Publishers.Remove ( publisher );
        }

        public async Task<bool> ExistsAsync ( int id )
        {
            return await _context.Publishers.AnyAsync ( p => p.Id == id );
        }

        public async Task<IEnumerable<Publisher>> SearchAsync ( string keyword )
        {
            keyword = keyword.ToLower ();

            return await _context.Publishers
                .Where ( p =>
                    p.Name.ToLower ().Contains ( keyword ) ||
                    p.Country.ToLower ().Contains ( keyword ) ||
                    p.Address.ToLower ().Contains ( keyword ) )
                .AsNoTracking ()
                .ToListAsync ();
        }

        public async Task<bool> ExistsByNameAsync ( string name )
        {
            return await _context.Publishers.AnyAsync (
                p => p.Name.ToLower () == name.ToLower ()
            );
        }

        public async Task SaveChangesAsync ( )
        {
            await _context.SaveChangesAsync ();
        }
    }
}
