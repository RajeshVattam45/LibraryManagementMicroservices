using BookCatalogService.Application.DTOs;
using BookCatalogService.Application.Services;
using BookCatalogService.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BookCatalogService.API.Controllers
{
    [Route ( "api/[controller]" )]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorAppService _authorService;

        public AuthorsController ( IAuthorAppService authorService )
        {
            _authorService = authorService;
        }

        // GET: api/authors
        [HttpGet]
        public async Task<IActionResult> GetAll ( )
        {
            var authors = await _authorService.GetAllAsync ();
            return Ok ( authors.Select ( MapToResponse ) );
        }

        // GET: api/authors/{id}
        [HttpGet ( "{id}" )]
        public async Task<IActionResult> Get ( int id )
        {
            var author = await _authorService.GetByIdAsync ( id );
            if (author == null)
                return NotFound ();

            return Ok ( MapToResponse ( author ) );
        }

        // POST: api/authors
        [HttpPost]
        public async Task<IActionResult> Create ( [FromBody] CreateAuthorDto dto )
        {
            var author = MapToEntity ( dto );
            var created = await _authorService.CreateAsync ( author );

            return CreatedAtAction ( nameof ( Get ), new { id = created.Id }, MapToResponse ( created ) );
        }

        // PUT: api/authors/{id}
        [HttpPut ( "{id}" )]
        public async Task<IActionResult> Update ( int id, [FromBody] UpdateAuthorDto dto )
        {
            var updatedAuthor = MapToEntity ( dto );
            var updated = await _authorService.UpdateAsync ( id, updatedAuthor );

            if (updated == null)
                return NotFound ();

            return Ok ( MapToResponse ( updated ) );
        }

        // DELETE: api/authors/{id}
        [HttpDelete ( "{id}" )]
        public async Task<IActionResult> Delete ( int id )
        {
            var deleted = await _authorService.DeleteAsync ( id );
            return deleted ? NoContent () : NotFound ();
        }

        // GET: api/authors/search?keyword=xyz
        [HttpGet ( "search" )]
        public async Task<IActionResult> Search ( string keyword )
        {
            var authors = await _authorService.SearchAsync ( keyword );
            return Ok ( authors.Select ( MapToResponse ) );
        }


        // -----------------------
        // Mapping Methods
        // -----------------------

        private Author MapToEntity ( CreateAuthorDto dto )
        {
            return new Author
            {
                Name = dto.Name,
                Biography = dto.Biography,
                BirthDate = dto.BirthDate,
                DeathDate = dto.DeathDate,
                Nationality = dto.Nationality,
                ProfileImageUrl = dto.ProfileImageUrl
            };
        }

        private Author MapToEntity ( UpdateAuthorDto dto )
        {
            return new Author
            {
                Name = dto.Name,
                Biography = dto.Biography,
                BirthDate = dto.BirthDate,
                DeathDate = dto.DeathDate,
                Nationality = dto.Nationality,
                ProfileImageUrl = dto.ProfileImageUrl,
                IsActive = dto.IsActive
            };
        }

        private AuthorResponseDto MapToResponse ( Author a )
        {
            return new AuthorResponseDto
            {
                Id = a.Id,
                Name = a.Name,
                Biography = a.Biography,
                BirthDate = a.BirthDate,
                DeathDate = a.DeathDate,
                Nationality = a.Nationality,
                ProfileImageUrl = a.ProfileImageUrl,
                IsActive = a.IsActive
            };
        }
    }
}
