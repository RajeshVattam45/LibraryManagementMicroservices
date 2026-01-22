using BookCatalogService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookCatalogService.Domain.Interfaces
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetAllAsync ( );
        Task<Book?> GetByIdAsync ( int id );
        Task AddAsync ( Book book );
        Task UpdateAsync ( Book book );
        Task DeleteAsync ( int id );

        // Advanced queries
        Task<IEnumerable<Book>> SearchAsync ( string keyword );
        Task<bool> ExistsAsync ( int id );

        // ⭐ NEW BUSINESS LOGIC SUPPORT
        Task<bool> ExistsByISBNAsync ( string isbn13 );

        Task SaveChangesAsync ( );
    }
}
