using BookCatalogService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookCatalogService.Application.Services
{
    public interface IAuthorAppService
    {
        Task<IEnumerable<Author>> GetAllAsync ( );
        Task<Author?> GetByIdAsync ( int id );
        Task<Author> CreateAsync ( Author author );
        Task<Author?> UpdateAsync ( int id, Author updated );
        Task<bool> DeleteAsync ( int id );
        Task<IEnumerable<Author>> SearchAsync ( string keyword );
    }
}
