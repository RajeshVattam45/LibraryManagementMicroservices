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
