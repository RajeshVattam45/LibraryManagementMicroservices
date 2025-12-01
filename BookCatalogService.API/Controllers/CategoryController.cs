using BookCatalogService.Application.DTOs;
using BookCatalogService.Application.Services;
using BookCatalogService.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BookCatalogService.API.Controllers
{
    [Route ( "api/[controller]" )]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryAppService _categoryService;

        public CategoriesController ( ICategoryAppService categoryService )
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll ( )
        {
            var categories = await _categoryService.GetAllAsync ();
            return Ok ( categories.Select ( MapToResponse ) );
        }

        [HttpGet ( "{id}" )]
        public async Task<IActionResult> Get ( int id )
        {
            var category = await _categoryService.GetByIdAsync ( id );
            if (category == null)
                return NotFound ();

            return Ok ( MapToResponse ( category ) );
        }

        [HttpPost]
        public async Task<IActionResult> Create ( [FromBody] CreateCategoryDto dto )
        {
            var category = MapToEntity ( dto );

            var created = await _categoryService.CreateAsync ( category );

            return CreatedAtAction ( nameof ( Get ), new { id = created.Id }, MapToResponse ( created ) );
        }

        [HttpPut ( "{id}" )]
        public async Task<IActionResult> Update ( int id, [FromBody] UpdateCategoryDto dto )
        {
            var entity = MapToEntity ( dto );

            var updated = await _categoryService.UpdateAsync ( id, entity );

            if (updated == null)
                return NotFound ();

            return Ok ( MapToResponse ( updated ) );
        }

        [HttpDelete ( "{id}" )]
        public async Task<IActionResult> Delete ( int id )
        {
            var deleted = await _categoryService.DeleteAsync ( id );
            return deleted ? NoContent () : NotFound ();
        }

        [HttpGet ( "search" )]
        public async Task<IActionResult> Search ( string keyword )
        {
            var categories = await _categoryService.SearchAsync ( keyword );
            return Ok ( categories.Select ( MapToResponse ) );
        }


        // -----------------------
        // Mapping helpers
        // -----------------------

        private Category MapToEntity ( CreateCategoryDto dto )
        {
            return new Category
            {
                Name = dto.Name,
                Description = dto.Description,
                Code = dto.Code,
                ParentCategoryId = dto.ParentCategoryId
            };
        }

        private Category MapToEntity ( UpdateCategoryDto dto )
        {
            return new Category
            {
                Name = dto.Name,
                Description = dto.Description,
                Code = dto.Code,
                ParentCategoryId = dto.ParentCategoryId,
                IsActive = dto.IsActive
            };
        }

        private CategoryResponseDto MapToResponse ( Category c )
        {
            return new CategoryResponseDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Code = c.Code,
                IsActive = c.IsActive,
                ParentCategoryId = c.ParentCategoryId,
                ParentCategoryName = c.ParentCategory?.Name ?? ""
            };
        }
    }
}
