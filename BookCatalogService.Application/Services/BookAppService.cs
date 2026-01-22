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
        private readonly IAuthorRepository _authorRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IPublisherRepository _publisherRepository;

        public BookAppService (
            IBookRepository bookRepository,
            IAuthorRepository authorRepository,
            ICategoryRepository categoryRepository,
            IPublisherRepository publisherRepository )
        {
            _bookRepository = bookRepository;
            _authorRepository = authorRepository;
            _categoryRepository = categoryRepository;
            _publisherRepository = publisherRepository;
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
            // 1. Validate ISBN formats
            if (string.IsNullOrWhiteSpace ( book.ISBN ) || book.ISBN.Length != 10)
                throw new Exception ( "ISBN must be exactly 10 characters." );

            if (string.IsNullOrWhiteSpace ( book.ISBN13 ) || book.ISBN13.Length != 13)
                throw new Exception ( "ISBN-13 must be exactly 13 digits." );

            if (!book.ISBN13.StartsWith ( "978" ) && !book.ISBN13.StartsWith ( "979" ))
                throw new Exception ( "ISBN-13 must start with 978 or 979." );

            // 2. Prevent duplicate books
            if (await _bookRepository.ExistsByISBNAsync ( book.ISBN13 ))
                throw new Exception ( $"Book with ISBN-13 {book.ISBN13} already exists." );

            // 3. Validate foreign keys
            if (!await _authorRepository.ExistsAsync ( book.AuthorId ))
                throw new Exception ( "Author does not exist." );

            if (!await _categoryRepository.ExistsAsync ( book.CategoryId ))
                throw new Exception ( "Category does not exist." );

            if (!await _publisherRepository.ExistsAsync ( book.PublisherId ))
                throw new Exception ( "Publisher does not exist." );

            // 4. Year validation
            if (book.PublishedYear < 1450 || book.PublishedYear > DateTime.Now.Year)
                throw new Exception ( "Invalid published year." );

            // 5. Price validation
            if (book.Price <= 0)
                throw new Exception ( "Price must be greater than zero." );

            // 6. Auto-assign available copies on creation
            book.AvailableCopies = book.TotalCopies;

            // 7. Normalize tags
            if (!string.IsNullOrWhiteSpace ( book.Tags ))
            {
                book.Tags = string.Join ( ",",
                    book.Tags.Split ( ',' )
                             .Select ( t => t.Trim ().ToLower () )
                             .Where ( t => !string.IsNullOrWhiteSpace ( t ) )
                             .Distinct () );
            }

            // 8. Save to repo
            await _bookRepository.AddAsync ( book );
            await _bookRepository.SaveChangesAsync ();

            return book;
        }

        public async Task<Book?> UpdateAsync ( int id, Book updatedBook )
        {
            var existing = await _bookRepository.GetByIdAsync ( id );
            if (existing == null)
                return null;

            // Validate foreign keys
            if (!await _authorRepository.ExistsAsync ( updatedBook.AuthorId ))
                throw new Exception ( "Author does not exist." );

            if (!await _categoryRepository.ExistsAsync ( updatedBook.CategoryId ))
                throw new Exception ( "Category does not exist." );

            if (!await _publisherRepository.ExistsAsync ( updatedBook.PublisherId ))
                throw new Exception ( "Publisher does not exist." );

            // Validate price
            if (updatedBook.Price <= 0)
                throw new Exception ( "Price must be greater than zero." );

            // Validate year
            if (updatedBook.PublishedYear < 1450 || updatedBook.PublishedYear > DateTime.Now.Year)
                throw new Exception ( "Invalid published year." );

            // Update book data
            existing.Title = updatedBook.Title;
            existing.Subtitle = updatedBook.Subtitle;
            existing.Description = updatedBook.Description;
            existing.CategoryId = updatedBook.CategoryId;
            existing.AuthorId = updatedBook.AuthorId;
            existing.PublisherId = updatedBook.PublisherId;
            existing.Edition = updatedBook.Edition;
            existing.Price = updatedBook.Price;
            existing.TotalCopies = updatedBook.TotalCopies;

            // AvailableCopies should not automatically update on update
            if (updatedBook.AvailableCopies >= 0)
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
