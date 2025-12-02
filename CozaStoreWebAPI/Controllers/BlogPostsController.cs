using CozaStore.Business.Contracts;
using CozaStore.Core.DTOs;
using CozaStore.Entities.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;
using System.Security.Claims;

namespace CozaStoreWebAPI.Controllers;

/// <summary>
/// Blog gönderileri işlemlerini yöneten API controller'ı.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class BlogPostsController : ControllerBase
{
    private readonly IBlogPostService _blogPostService;

    public BlogPostsController(IBlogPostService blogPostService)
    {
        _blogPostService = blogPostService;
    }

    /// <summary>
    /// GET /api/blogposts
    /// Tüm blog gönderilerini listeler (Admin için).
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _blogPostService.GetAllAsync();
        
        if (!result.Success)
        {
            return BadRequest(new { message = result.Message });
        }

        var blogPosts = result.Data.Select(b => new BlogPostDto
        {
            Id = b.Id,
            Title = b.Title,
            Content = b.Content,
            ImageUrl = b.ImageUrl,
            IsPublished = b.IsPublished,
            AuthorId = b.AuthorId,
            AuthorName = b.Author?.UserName,
            CreatedDate = b.CreatedDate,
            UpdatedDate = b.UpdatedDate,
            IsActive = b.IsActive
        }).ToList();

        return Ok(blogPosts);
    }

    /// <summary>
    /// GET /api/blogposts/published
    /// Yayınlanmış blog gönderilerini listeler (Public).
    /// </summary>
    [HttpGet("published")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPublished()
    {
        var result = await _blogPostService.GetPublishedAsync();
        
        if (!result.Success)
        {
            return BadRequest(new { message = result.Message });
        }

        var blogPosts = result.Data.Select(b => new BlogPostDto
        {
            Id = b.Id,
            Title = b.Title,
            Content = b.Content,
            ImageUrl = b.ImageUrl,
            IsPublished = b.IsPublished,
            AuthorId = b.AuthorId,
            AuthorName = b.Author?.UserName ?? "Admin",
            CreatedDate = b.CreatedDate,
            UpdatedDate = b.UpdatedDate,
            IsActive = b.IsActive
        }).ToList();

        return Ok(blogPosts);
    }

    /// <summary>
    /// GET /api/blogposts/{id}
    /// Belirli bir blog gönderisini ID'ye göre getirir.
    /// </summary>
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _blogPostService.GetByIdAsync(id);
        
        if (!result.Success || result.Data == null)
        {
            return NotFound(new { message = result.Message });
        }

        var blogPost = result.Data;
        
        // Eğer yayınlanmamışsa ve kullanıcı Admin değilse, erişim reddedilir
        if (!blogPost.IsPublished && !User.IsInRole("Admin"))
        {
            return NotFound(new { message = "Blog gönderisi bulunamadı." });
        }

        var blogPostDto = new BlogPostDto
        {
            Id = blogPost.Id,
            Title = blogPost.Title,
            Content = blogPost.Content,
            ImageUrl = blogPost.ImageUrl,
            IsPublished = blogPost.IsPublished,
            AuthorId = blogPost.AuthorId,
            AuthorName = blogPost.Author?.UserName ?? "Admin",
            CreatedDate = blogPost.CreatedDate,
            UpdatedDate = blogPost.UpdatedDate,
            IsActive = blogPost.IsActive
        };

        return Ok(blogPostDto);
    }

    /// <summary>
    /// POST /api/blogposts
    /// Yeni blog gönderisi oluşturur. (Admin yetkisi gerekli)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateBlogPostRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // AuthorId'yi mevcut kullanıcıdan al
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized(new { message = "Kullanıcı bilgisi bulunamadı." });
        }

        // DTO'dan entity oluştur
        var blogPost = new BlogPost
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Content = request.Content,
            ImageUrl = request.ImageUrl,
            IsPublished = request.IsPublished,
            AuthorId = userId,
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };

        var result = await _blogPostService.AddAsync(blogPost);
        
        if (!result.Success)
        {
            return BadRequest(new { message = result.Message });
        }

        // Oluşturulan blog gönderisini getir
        var createdResult = await _blogPostService.GetByIdAsync(blogPost.Id);
        if (!createdResult.Success || createdResult.Data == null)
        {
            return StatusCode(500, new { message = "Blog gönderisi oluşturuldu ancak getirilemedi." });
        }

        var createdBlogPost = createdResult.Data;
        var blogPostDto = new BlogPostDto
        {
            Id = createdBlogPost.Id,
            Title = createdBlogPost.Title,
            Content = createdBlogPost.Content,
            ImageUrl = createdBlogPost.ImageUrl,
            IsPublished = createdBlogPost.IsPublished,
            AuthorId = createdBlogPost.AuthorId,
            AuthorName = createdBlogPost.Author?.UserName,
            CreatedDate = createdBlogPost.CreatedDate,
            UpdatedDate = createdBlogPost.UpdatedDate,
            IsActive = createdBlogPost.IsActive
        };

        return CreatedAtAction(nameof(GetById), new { id = blogPostDto.Id }, blogPostDto);
    }

    /// <summary>
    /// PUT /api/blogposts/{id}
    /// Blog gönderisini günceller. (Admin yetkisi gerekli)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBlogPostRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Mevcut blog gönderisini getir
        var existingResult = await _blogPostService.GetByIdAsync(id);
        if (!existingResult.Success || existingResult.Data == null)
        {
            return NotFound(new { message = "Blog gönderisi bulunamadı." });
        }

        var existingBlogPost = existingResult.Data;

        // Mevcut tracked entity'nin property'lerini güncelle (yeni entity oluşturma - tracking sorunu önlenir)
        existingBlogPost.Title = request.Title;
        existingBlogPost.Content = request.Content;
        existingBlogPost.ImageUrl = request.ImageUrl;
        existingBlogPost.IsPublished = request.IsPublished;
        existingBlogPost.UpdatedDate = DateTime.UtcNow;
        // AuthorId, IsActive, CreatedDate değiştirilmemeli - zaten mevcut entity'de doğru değerler var

        var result = await _blogPostService.UpdateAsync(existingBlogPost);
        
        if (!result.Success)
        {
            return BadRequest(new { message = result.Message });
        }

        return NoContent();
    }

    /// <summary>
    /// DELETE /api/blogposts/{id}
    /// Blog gönderisini siler. (Admin yetkisi gerekli)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _blogPostService.DeleteAsync(id);
        
        if (!result.Success)
        {
            return BadRequest(new { message = result.Message });
        }

        return NoContent();
    }
}
