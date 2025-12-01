using BookCatalogService.Application.DTOs;
using BookCatalogService.Application.Services;
using BookCatalogService.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BookCatalogService.API.Controllers
{
    [Route ( "api/[controller]" )]
    [ApiController]
    public class PublishersController : ControllerBase
    {
        private readonly IPublisherAppService _publisherService;

        public PublishersController ( IPublisherAppService publisherService )
        {
            _publisherService = publisherService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll ( )
        {
            var data = await _publisherService.GetAllAsync ();
            return Ok ( data.Select ( MapToResponse ) );
        }

        [HttpGet ( "{id}" )]
        public async Task<IActionResult> Get ( int id )
        {
            var publisher = await _publisherService.GetByIdAsync ( id );
            if (publisher == null)
                return NotFound ();

            return Ok ( MapToResponse ( publisher ) );
        }

        [HttpPost]
        public async Task<IActionResult> Create ( [FromBody] CreatePublisherDto dto )
        {
            var entity = MapToEntity ( dto );
            var created = await _publisherService.CreateAsync ( entity );

            return CreatedAtAction ( nameof ( Get ), new { id = created.Id }, MapToResponse ( created ) );
        }

        [HttpPut ( "{id}" )]
        public async Task<IActionResult> Update ( int id, [FromBody] UpdatePublisherDto dto )
        {
            var entity = MapToEntity ( dto );
            var updated = await _publisherService.UpdateAsync ( id, entity );

            if (updated == null)
                return NotFound ();

            return Ok ( MapToResponse ( updated ) );
        }

        [HttpDelete ( "{id}" )]
        public async Task<IActionResult> Delete ( int id )
        {
            var deleted = await _publisherService.DeleteAsync ( id );
            return deleted ? NoContent () : NotFound ();
        }

        [HttpGet ( "search" )]
        public async Task<IActionResult> Search ( string keyword )
        {
            var data = await _publisherService.SearchAsync ( keyword );
            return Ok ( data.Select ( MapToResponse ) );
        }


        // -----------------------
        // Mapping Helpers
        // -----------------------

        private Publisher MapToEntity ( CreatePublisherDto dto )
        {
            return new Publisher
            {
                Name = dto.Name,
                Website = dto.Website,
                Email = dto.Email,
                Address = dto.Address,
                Country = dto.Country
            };
        }

        private Publisher MapToEntity ( UpdatePublisherDto dto )
        {
            return new Publisher
            {
                Name = dto.Name,
                Website = dto.Website,
                Email = dto.Email,
                Address = dto.Address,
                Country = dto.Country,
                IsActive = dto.IsActive
            };
        }

        private PublisherResponseDto MapToResponse ( Publisher p )
        {
            return new PublisherResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Website = p.Website,
                Email = p.Email,
                Address = p.Address,
                Country = p.Country,
                IsActive = p.IsActive
            };
        }
    }
}
