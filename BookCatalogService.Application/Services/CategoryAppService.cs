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
