using BookCatalogService.Application.DTOs;
using BookCatalogService.Application.Services;
using BookCatalogService.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BookCatalogService.API.Controllers
{
    [Route ( "api/[controller]" )]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookAppService _bookService;

        public BooksController ( IBookAppService bookService )
        {
            _bookService = bookService;
        }

        // GET: api/books
        [HttpGet]
        public async Task<IActionResult> GetAll ( )
        {
            var books = await _bookService.GetAllAsync ();

            var response = books.Select ( b => MapToResponse ( b ) );

            return Ok ( response );
        }

        // GET: api/books/{id}
        [HttpGet ( "{id}" )]
        public async Task<IActionResult> Get ( int id )
        {
            var book = await _bookService.GetByIdAsync ( id );
            if (book == null)
                return NotFound ();

            return Ok ( MapToResponse ( book ) );
        }

        // POST: api/books
        [HttpPost]
        public async Task<IActionResult> Create ( [FromBody] CreateBookDto dto )
        {
            if (!ModelState.IsValid)
                return BadRequest ( ModelState );

            var book = MapToEntity ( dto );

            var created = await _bookService.CreateAsync ( book );

            return CreatedAtAction ( nameof ( Get ), new { id = created.Id }, MapToResponse ( created ) );
        }

        // PUT: api/books/{id}
        [HttpPut ( "{id}" )]
        public async Task<IActionResult> Update ( int id, [FromBody] UpdateBookDto dto )
        {
            if (!ModelState.IsValid)
                return BadRequest ( ModelState );

            var bookEntity = MapToEntity ( dto );
            bookEntity.Id = id;

            var updated = await _bookService.UpdateAsync ( id, bookEntity );

            if (updated == null)
                return NotFound ();

            return Ok ( MapToResponse ( updated ) );
        }

        // DELETE: api/books/{id}
        [HttpDelete ( "{id}" )]
        public async Task<IActionResult> Delete ( int id )
        {
            var deleted = await _bookService.DeleteAsync ( id );

            if (!deleted)
                return NotFound ();

            return NoContent ();
        }

        // GET: api/books/search?keyword=xyz
        [HttpGet ( "search" )]
        public async Task<IActionResult> Search ( string keyword )
        {
            var books = await _bookService.SearchAsync ( keyword );

            var response = books.Select ( b => MapToResponse ( b ) );

            return Ok ( response );
        }


        // ----------------------
        // Mapping Helpers
        // ----------------------

        private Book MapToEntity ( CreateBookDto dto )
        {
            return new Book
            {
                ISBN = dto.ISBN,
                ISBN13 = dto.ISBN13,
                Title = dto.Title,
                Subtitle = dto.Subtitle,
                Description = dto.Description,
                AuthorId = dto.AuthorId,
                CategoryId = dto.CategoryId,
                PublisherId = dto.PublisherId,
                PublishedYear = dto.PublishedYear,
                Edition = dto.Edition,
                Language = dto.Language,
                Format = dto.Format,
                Price = dto.Price,
                TotalCopies = dto.TotalCopies,
                AvailableCopies = dto.TotalCopies, // Auto-set
                ShelfLocation = dto.ShelfLocation,
                Tags = dto.Tags,
                CoverImageUrl = dto.CoverImageUrl
            };
        }

        private BookResponseDto MapToResponse ( Book b )
        {
            return new BookResponseDto
            {
                Id = b.Id,
                ISBN = b.ISBN,
                ISBN13 = b.ISBN13,
                Title = b.Title,
                Subtitle = b.Subtitle,
                Description = b.Description,
                AuthorName = b.Author?.Name ?? "",
                CategoryName = b.Category?.Name ?? "",
                PublisherName = b.Publisher?.Name ?? "",
                PublishedYear = b.PublishedYear,
                Edition = b.Edition,
                Language = b.Language,
                Format = b.Format,
                Price = b.Price,
                TotalCopies = b.TotalCopies,
                AvailableCopies = b.AvailableCopies,
                ShelfLocation = b.ShelfLocation,
                Tags = b.Tags,
                CoverImageUrl = b.CoverImageUrl
            };
        }

        private Book MapToEntity ( UpdateBookDto dto )
        {
            return new Book
            {
                ISBN = dto.ISBN,
                ISBN13 = dto.ISBN13,
                Title = dto.Title,
                Subtitle = dto.Subtitle,
                Description = dto.Description,
                AuthorId = dto.AuthorId,
                CategoryId = dto.CategoryId,
                PublisherId = dto.PublisherId,
                PublishedYear = dto.PublishedYear,
                Edition = dto.Edition,
                Language = dto.Language,
                Format = dto.Format,
                Price = dto.Price,
                TotalCopies = dto.TotalCopies,
                AvailableCopies = dto.AvailableCopies,
                ShelfLocation = dto.ShelfLocation,
                Tags = dto.Tags,
                CoverImageUrl = dto.CoverImageUrl
            };
        }
    }
}
