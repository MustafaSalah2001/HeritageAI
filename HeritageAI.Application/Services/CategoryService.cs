using HeritageAI.Application.Dto.Admin;
using HeritageAI.Application.Interfaces;
using HeritageAI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritageAI.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<List<CategoryDto>> GetAllAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return categories.Select(c => new CategoryDto { Id = c.Id, Name = c.Name }).ToList();
        }

        public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
        {
            var existing = await _categoryRepository.GetByNameAsync(dto.Name);

            if (existing is not null)
                throw new InvalidOperationException("Category already exists.");

            var category = new Category { Name = dto.Name };

            await _categoryRepository.AddAsync(category);
            await _categoryRepository.SaveChangesAsync();

            return new CategoryDto { Id = category.Id, Name = category.Name };
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);

            if (category is null)
                throw new KeyNotFoundException("Category not found.");

            _categoryRepository.Remove(category);
            await _categoryRepository.SaveChangesAsync();
        }
    }
}
