using BookCatalogService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookCatalogService.Application.Services
{
    public interface IPublisherAppService
    {
        Task<IEnumerable<Publisher>> GetAllAsync ( );
        Task<Publisher?> GetByIdAsync ( int id );
        Task<Publisher> CreateAsync ( Publisher publisher );
        Task<Publisher?> UpdateAsync ( int id, Publisher updated );
        Task<bool> DeleteAsync ( int id );
        Task<IEnumerable<Publisher>> SearchAsync ( string keyword );
    }
}
