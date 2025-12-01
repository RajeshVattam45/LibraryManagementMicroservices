using BookCatalogService.Domain.Entities;
using BookCatalogService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookCatalogService.Application.Services
{
    public class BookAppService : IBookAppService
    {
        private readonly IBookRepository _bookRepository;

        public BookAppService ( IBookRepository bookRepository )
        {
            _bookRepository = bookRepository;
        }

        public async Task<IEnumerable<Book>> GetAllAsync ( )
        {
            return await _bookRepository.GetAllAsync ();
        }

        public async Task<Book?> GetByIdAsync ( int id )
        {
            return await _bookRepository.GetByIdAsync ( id );
        }

        public async Task<Book> CreateAsync ( Book book )
        {
            // Business rule: default available copies = total copies
            if (book.AvailableCopies == 0)
                book.AvailableCopies = book.TotalCopies;

            await _bookRepository.AddAsync ( book );
            await _bookRepository.SaveChangesAsync ();

            return book;
        }

        public async Task<Book?> UpdateAsync ( int id, Book updatedBook )
        {
            var existing = await _bookRepository.GetByIdAsync ( id );
            if (existing == null)
                return null;

            // Update fields (auto-mapper can do this too)
            existing.Title = updatedBook.Title;
            existing.Subtitle = updatedBook.Subtitle;
            existing.Description = updatedBook.Description;
            existing.CategoryId = updatedBook.CategoryId;
            existing.AuthorId = updatedBook.AuthorId;
            existing.PublisherId = updatedBook.PublisherId;
            existing.Edition = updatedBook.Edition;
            existing.Price = updatedBook.Price;
            existing.TotalCopies = updatedBook.TotalCopies;
            existing.AvailableCopies = updatedBook.AvailableCopies;
            existing.Tags = updatedBook.Tags;
            existing.Format = updatedBook.Format;
            existing.Language = updatedBook.Language;
            existing.CoverImageUrl = updatedBook.CoverImageUrl;
            existing.UpdatedAt = DateTime.UtcNow;

            await _bookRepository.UpdateAsync ( existing );
            await _bookRepository.SaveChangesAsync ();

            return existing;
        }

        public async Task<bool> DeleteAsync ( int id )
        {
            if (!await _bookRepository.ExistsAsync ( id ))
                return false;

            await _bookRepository.DeleteAsync ( id );
            await _bookRepository.SaveChangesAsync ();

            return true;
        }

        public async Task<IEnumerable<Book>> SearchAsync ( string keyword )
        {
            return await _bookRepository.SearchAsync ( keyword );
        }
    }
}
