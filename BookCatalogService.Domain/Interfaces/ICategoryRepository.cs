using BookCatalogService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookCatalogService.Domain.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync ( );
        Task<Category?> GetByIdAsync ( int id );
        Task AddAsync ( Category category );
        Task UpdateAsync ( Category category );
        Task DeleteAsync ( int id );
        Task<bool> ExistsAsync ( int id );
        Task<IEnumerable<Category>> SearchAsync ( string keyword );
        Task<bool> ExistsByNameAsync ( string name );
        Task<bool> ExistsByCodeAsync ( string code );
        Task SaveChangesAsync ( );
    }
}
