using BookCatalogService.Domain.Entities;
using BookCatalogService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookCatalogService.Application.Services
{
    public class PublisherAppService : IPublisherAppService
    {
        private readonly IPublisherRepository _publisherRepository;

        public PublisherAppService ( IPublisherRepository publisherRepository )
        {
            _publisherRepository = publisherRepository;
        }

        public async Task<IEnumerable<Publisher>> GetAllAsync ( )
        {
            return await _publisherRepository.GetAllAsync ();
        }

        public async Task<Publisher?> GetByIdAsync ( int id )
        {
            return await _publisherRepository.GetByIdAsync ( id );
        }

        public async Task<Publisher> CreateAsync ( Publisher publisher )
        {
            publisher.CreatedAt = DateTime.UtcNow;
            publisher.UpdatedAt = DateTime.UtcNow;

            await _publisherRepository.AddAsync ( publisher );
            await _publisherRepository.SaveChangesAsync ();

            return publisher;
        }

        public async Task<Publisher?> UpdateAsync ( int id, Publisher updated )
        {
            var existing = await _publisherRepository.GetByIdAsync ( id );
            if (existing == null)
                return null;

            existing.Name = updated.Name;
            existing.Website = updated.Website;
            existing.Email = updated.Email;
            existing.Address = updated.Address;
            existing.Country = updated.Country;
            existing.IsActive = updated.IsActive;
            existing.UpdatedAt = DateTime.UtcNow;

            await _publisherRepository.UpdateAsync ( existing );
            await _publisherRepository.SaveChangesAsync ();

            return existing;
        }

        public async Task<bool> DeleteAsync ( int id )
        {
            if (!await _publisherRepository.ExistsAsync ( id ))
                return false;

            await _publisherRepository.DeleteAsync ( id );
            await _publisherRepository.SaveChangesAsync ();

            return true;
        }

        public async Task<IEnumerable<Publisher>> SearchAsync ( string keyword )
        {
            return await _publisherRepository.SearchAsync ( keyword );
        }
    }
}
