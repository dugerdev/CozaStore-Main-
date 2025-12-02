using CozaStore.Business.Contracts;
using CozaStore.Core.DTOs;
using CozaStore.Entities.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;

namespace CozaStoreWebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _categoryService.GetAllAsync();
        
        if (!result.Success)
        {
            return BadRequest(new { message = result.Message });
        }

        var categories = result.Data.Select(c => new CategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            ImageUrl = c.ImageUrl,
            CreatedDate = c.CreatedDate,
            IsActive = c.IsActive
        }).ToList();

        return Ok(categories);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _categoryService.GetByIdAsync(id);
        
        if (!result.Success || result.Data == null)
        {
            return NotFound(new { message = result.Message });
        }

        var category = result.Data;
        var categoryDto = new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            ImageUrl = category.ImageUrl,
            CreatedDate = category.CreatedDate,
            IsActive = category.IsActive
        };

        return Ok(categoryDto);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] Category category)
    {
        // ModelState'den navigation property hatalarını temizle
        ModelState.Remove("Products");
        
        // Model validation kontrolü
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .SelectMany(x => x.Value?.Errors ?? Enumerable.Empty<ModelError>())
                .Select(x => x.ErrorMessage)
                .ToList();
            
            if (errors.Any())
            {
                return BadRequest(new { message = string.Join(", ", errors) });
            }
        }
        
        // Navigation property'leri null yap
        category.Products = null!;
        
        var result = await _categoryService.AddAsync(category);
        
        if (!result.Success)
        {
            return BadRequest(new { message = result.Message });
        }

        // CategoryDto formatında döndür
        var categoryDto = new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            ImageUrl = category.ImageUrl,
            CreatedDate = category.CreatedDate,
            IsActive = category.IsActive
        };

        return CreatedAtAction(nameof(GetById), new { id = category.Id }, categoryDto);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Category category)
    {
        if (id != category.Id)
        {
            return BadRequest(new { message = "ID uyuşmazlığı." });
        }

        var result = await _categoryService.UpdateAsync(category);
        
        if (!result.Success)
        {
            return BadRequest(new { message = result.Message });
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _categoryService.DeleteAsync(id);
        
        if (!result.Success)
        {
            return BadRequest(new { message = result.Message });
        }

        return NoContent();
    }
}
