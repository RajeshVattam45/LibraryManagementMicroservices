using BookCatalogService.Domain.Entities;
using BookCatalogService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookCatalogService.Application.Services
{
    public class AuthorAppService : IAuthorAppService
    {
        private readonly IAuthorRepository _authorRepository;

        public AuthorAppService ( IAuthorRepository authorRepository )
        {
            _authorRepository = authorRepository;
        }

        public async Task<IEnumerable<Author>> GetAllAsync ( )
        {
            return await _authorRepository.GetAllAsync ();
        }

        public async Task<Author?> GetByIdAsync ( int id )
        {
            return await _authorRepository.GetByIdAsync ( id );
        }

        public async Task<Author> CreateAsync ( Author author )
        {
            // Validating if the birth date is not in the future(Universal Coordinator Time(UTC))
            if (author.BirthDate > DateTime.UtcNow)
                throw new Exception ( "Birth date cannot be in the future." );

            // Checking for duplicate author names
            if (await _authorRepository.ExistsByNameAsync ( author.Name ))
                throw new Exception ( "An author with this name already exists." );

            // Validating death date if provided
            if (author.DeathDate.HasValue && author.DeathDate < author.BirthDate)
                throw new Exception ( "Death date cannot be before birth date." );

            // Validating death date is not in the future
            if (author.DeathDate.HasValue && author.DeathDate > DateTime.UtcNow)
                throw new Exception ( "Death date cannot be in the future." );

            if (string.IsNullOrWhiteSpace ( author.Nationality ))
                throw new Exception ( "Nationality is required." );

            // Validating author name length
            if (author.Name.Length < 3)
                throw new Exception ( "Author name must contain at least 3 characters." );

            author.CreatedAt = DateTime.UtcNow;
            author.UpdatedAt = DateTime.UtcNow;


            await _authorRepository.AddAsync ( author );
            await _authorRepository.SaveChangesAsync ();

            return author;
        }

        public async Task<Author?> UpdateAsync ( int id, Author updated )
        {
            var existing = await _authorRepository.GetByIdAsync ( id );
            if (existing == null)
                return null;

            // Validate name
            if (string.IsNullOrWhiteSpace ( updated.Name ) || updated.Name.Length < 3)
                throw new Exception ( "Author name must be at least 3 characters." );

            // Check duplicate name (if changed)
            if (!string.Equals ( existing.Name, updated.Name, StringComparison.OrdinalIgnoreCase ))
            {
                if (await _authorRepository.ExistsByNameAsync ( updated.Name ))
                    throw new Exception ( "Another author with this name already exists." );
            }

            // Validate birthdate
            if (updated.BirthDate > DateTime.UtcNow)
                throw new Exception ( "Birth date cannot be in the future." );

            // Validate deathdate
            if (updated.DeathDate.HasValue)
            {
                if (updated.DeathDate < updated.BirthDate)
                    throw new Exception ( "Death date cannot be before birth date." );

                if (updated.DeathDate > DateTime.UtcNow)
                    throw new Exception ( "Death date cannot be in the future." );
            }

            // Nationality check
            if (string.IsNullOrWhiteSpace ( updated.Nationality ))
                throw new Exception ( "Nationality is required." );

            // Update only what I explicitly allow to be updated
            existing.Name = updated.Name;
            existing.Biography = updated.Biography;
            existing.BirthDate = updated.BirthDate;
            existing.DeathDate = updated.DeathDate;
            existing.Nationality = updated.Nationality;
            existing.ProfileImageUrl = updated.ProfileImageUrl;
            existing.IsActive = updated.IsActive;
            existing.UpdatedAt = DateTime.UtcNow;

            await _authorRepository.UpdateAsync ( existing );
            await _authorRepository.SaveChangesAsync ();

            return existing;
        }

        public async Task<bool> DeleteAsync ( int id )
        {
            if (!await _authorRepository.ExistsAsync ( id ))
                return false;

            await _authorRepository.DeleteAsync ( id );
            await _authorRepository.SaveChangesAsync ();

            return true;
        }

        public async Task<IEnumerable<Author>> SearchAsync ( string keyword )
        {
            return await _authorRepository.SearchAsync ( keyword );
        }
    }
}
