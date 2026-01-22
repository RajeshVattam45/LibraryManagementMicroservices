using BookCatalogService.Domain.Entities;
using BookCatalogService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            // 1. Validate name
            if (string.IsNullOrWhiteSpace ( publisher.Name ) || publisher.Name.Length < 3)
                throw new Exception ( "Publisher name must be at least 3 characters." );

            // 2. Prevent duplicate names
            if (await _publisherRepository.ExistsByNameAsync ( publisher.Name ))
                throw new Exception ( "A publisher with this name already exists." );

            // 3. Validate website
            if (!Uri.IsWellFormedUriString ( publisher.Website, UriKind.Absolute ))
                throw new Exception ( "Invalid website URL format." );

            // 4. Validate email
            if (!Regex.IsMatch ( publisher.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$" ))
                throw new Exception ( "Invalid email address format." );

            // 5. Validate address
            if (string.IsNullOrWhiteSpace ( publisher.Address ))
                throw new Exception ( "Address is required." );

            // 6. Validate country
            if (string.IsNullOrWhiteSpace ( publisher.Country ))
                throw new Exception ( "Country is required." );
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

            // Validate name
            if (string.IsNullOrWhiteSpace ( updated.Name ) || updated.Name.Length < 3)
                throw new Exception ( "Publisher name must be at least 3 characters." );

            // Check duplicate name if changed
            if (!string.Equals ( existing.Name, updated.Name, StringComparison.OrdinalIgnoreCase ))
            {
                if (await _publisherRepository.ExistsByNameAsync ( updated.Name ))
                    throw new Exception ( "Another publisher with this name already exists." );
            }

            // Validate URL
            if (!Uri.IsWellFormedUriString ( updated.Website, UriKind.Absolute ))
                throw new Exception ( "Invalid website URL format." );

            // Validate email
            if (!Regex.IsMatch ( updated.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$" ))
                throw new Exception ( "Invalid email address format." );

            // Validate address & country
            if (string.IsNullOrWhiteSpace ( updated.Address ))
                throw new Exception ( "Address is required." );
            if (string.IsNullOrWhiteSpace ( updated.Country ))
                throw new Exception ( "Country is required." );

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
