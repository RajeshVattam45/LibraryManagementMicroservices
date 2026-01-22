using BookCatalogService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookCatalogService.Domain.Interfaces
{
    public interface IAuthorRepository
    {
        Task<IEnumerable<Author>> GetAllAsync ( );
        Task<Author?> GetByIdAsync ( int id );
        Task AddAsync ( Author author );
        Task UpdateAsync ( Author author );
        Task DeleteAsync ( int id );
        Task<bool> ExistsAsync ( int id );
        Task<IEnumerable<Author>> SearchAsync ( string keyword );
        Task<bool> ExistsByNameAsync ( string name );

        Task SaveChangesAsync ( );
    }
}
