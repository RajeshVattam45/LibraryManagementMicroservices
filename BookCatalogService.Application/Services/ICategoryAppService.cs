using BookCatalogService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookCatalogService.Application.Services
{
    public interface ICategoryAppService
    {
        Task<IEnumerable<Category>> GetAllAsync ( );
        Task<Category?> GetByIdAsync ( int id );
        Task<Category> CreateAsync ( Category category );
        Task<Category?> UpdateAsync ( int id, Category updated );
        Task<bool> DeleteAsync ( int id );
        Task<IEnumerable<Category>> SearchAsync ( string keyword );
    }
}
