using BookCatalogService.Domain.Entities;
using BookCatalogService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookCatalogService.Application.Services
{
    public class CategoryAppService : ICategoryAppService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryAppService ( ICategoryRepository categoryRepository )
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<Category>> GetAllAsync ( )
        {
            return await _categoryRepository.GetAllAsync ();
        }

        public async Task<Category?> GetByIdAsync ( int id )
        {
            return await _categoryRepository.GetByIdAsync ( id );
        }

        public async Task<Category> CreateAsync ( Category category )
        {
            // 1. Validate name
            if (string.IsNullOrWhiteSpace ( category.Name ) || category.Name.Length < 3)
                throw new Exception ( "Category name must be at least 3 characters." );

            // 2. Validate description
            if (string.IsNullOrWhiteSpace ( category.Description ) || category.Description.Length < 10)
                throw new Exception ( "Description must be at least 10 characters." );

            // 3. Normalize code
            category.Code = category.Code.Trim ().ToUpper ();

            if (category.Code.Length < 2 || category.Code.Length > 10)
                throw new Exception ( "Category code must be between 2 and 10 characters." );

            // 4. Unique name & code
            if (await _categoryRepository.ExistsByNameAsync ( category.Name ))
                throw new Exception ( "A category with this name already exists." );

            if (await _categoryRepository.ExistsByCodeAsync ( category.Code ))
                throw new Exception ( "A category with this code already exists." );

            // 5. Validate parent category
            if (category.ParentCategoryId.HasValue)
            {
                if (!await _categoryRepository.ExistsAsync ( category.ParentCategoryId.Value ))
                    throw new Exception ( "Parent category does not exist." );

                if (category.ParentCategoryId == category.Id)
                    throw new Exception ( "Category cannot be its own parent." );
            }

            category.CreatedAt = DateTime.UtcNow;
            category.UpdatedAt = DateTime.UtcNow;

            await _categoryRepository.AddAsync ( category );
            await _categoryRepository.SaveChangesAsync ();

            return category;
        }

        public async Task<Category?> UpdateAsync ( int id, Category updated )
        {
            var existing = await _categoryRepository.GetByIdAsync ( id );
            if (existing == null)
                return null;

            // Validate name
            if (string.IsNullOrWhiteSpace ( updated.Name ) || updated.Name.Length < 3)
                throw new Exception ( "Category name must be at least 3 characters." );

            // Check duplicate name if changed
            if (!string.Equals ( existing.Name, updated.Name, StringComparison.OrdinalIgnoreCase ))
            {
                if (await _categoryRepository.ExistsByNameAsync ( updated.Name ))
                    throw new Exception ( "Another category with this name already exists." );
            }

            // Validate description
            if (string.IsNullOrWhiteSpace ( updated.Description ) || updated.Description.Length < 10)
                throw new Exception ( "Description must be at least 10 characters." );

            // Normalize & validate code
            updated.Code = updated.Code.Trim ().ToUpper ();

            if (!string.Equals ( existing.Code, updated.Code, StringComparison.OrdinalIgnoreCase ))
            {
                if (await _categoryRepository.ExistsByCodeAsync ( updated.Code ))
                    throw new Exception ( "Another category with this code already exists." );
            }

            if (updated.Code.Length < 2 || updated.Code.Length > 10)
                throw new Exception ( "Category code must be between 2 and 10 characters." );

            // Validate parent category
            if (updated.ParentCategoryId.HasValue)
            {
                if (!await _categoryRepository.ExistsAsync ( updated.ParentCategoryId.Value ))
                    throw new Exception ( "Parent category does not exist." );

                if (updated.ParentCategoryId == id)
                    throw new Exception ( "Category cannot be its own parent." );
            }

            existing.Name = updated.Name;
            existing.Description = updated.Description;
            existing.Code = updated.Code;
            existing.ParentCategoryId = updated.ParentCategoryId;
            existing.IsActive = updated.IsActive;
            existing.UpdatedAt = DateTime.UtcNow;

            await _categoryRepository.UpdateAsync ( existing );
            await _categoryRepository.SaveChangesAsync ();

            return existing;
        }

        public async Task<bool> DeleteAsync ( int id )
        {
            if (!await _categoryRepository.ExistsAsync ( id ))
                return false;

            await _categoryRepository.DeleteAsync ( id );
            await _categoryRepository.SaveChangesAsync ();

            return true;
        }

        public async Task<IEnumerable<Category>> SearchAsync ( string keyword )
        {
            return await _categoryRepository.SearchAsync ( keyword );
        }
    }
}
