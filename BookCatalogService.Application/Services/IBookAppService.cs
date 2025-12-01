using BookCatalogService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookCatalogService.Application.Services
{
    public interface IBookAppService
    {
        Task<IEnumerable<Book>> GetAllAsync ( );
        Task<Book?> GetByIdAsync ( int id );
        Task<Book> CreateAsync ( Book book );
        Task<Book?> UpdateAsync ( int id, Book updatedBook );
        Task<bool> DeleteAsync ( int id );
        Task<IEnumerable<Book>> SearchAsync ( string keyword );
    }
}
