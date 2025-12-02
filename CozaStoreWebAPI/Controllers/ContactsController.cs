using CozaStore.Business.Contracts;
using CozaStore.Core.DTOs;
using CozaStore.Entities.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CozaStoreWebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactsController : ControllerBase
{
    private readonly IContactService _contactService;
    private readonly ILogger<ContactsController> _logger;

    public ContactsController(IContactService contactService, ILogger<ContactsController> logger)
    {
        _contactService = contactService;
        _logger = logger;
    }

    /// <summary>
    /// Tüm iletişim mesajlarını getirir (Admin)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var result = await _contactService.GetAllAsync();
            
            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            var contacts = result.Data.Select(c => new ContactDto
            {
                Id = c.Id,
                Email = c.Email,
                Message = c.Message,
                IsRead = c.IsRead,
                ReadDate = c.ReadDate,
                CreatedDate = c.CreatedDate
            }).ToList();

            return Ok(contacts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all contacts");
            return StatusCode(500, new { message = "Bir hata oluştu." });
        }
    }

    /// <summary>
    /// Okunmamış iletişim mesajlarını getirir (Admin)
    /// </summary>
    [HttpGet("unread")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUnread()
    {
        try
        {
            var result = await _contactService.GetUnreadAsync();
            
            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            var contacts = result.Data.Select(c => new ContactDto
            {
                Id = c.Id,
                Email = c.Email,
                Message = c.Message,
                IsRead = c.IsRead,
                ReadDate = c.ReadDate,
                CreatedDate = c.CreatedDate
            }).ToList();

            return Ok(contacts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting unread contacts");
            return StatusCode(500, new { message = "Bir hata oluştu." });
        }
    }

    /// <summary>
    /// ID'ye göre iletişim mesajını getirir (Admin)
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var result = await _contactService.GetByIdAsync(id);
            
            if (!result.Success || result.Data == null)
            {
                return NotFound(new { message = "İletişim mesajı bulunamadı." });
            }

            var contact = new ContactDto
            {
                Id = result.Data.Id,
                Email = result.Data.Email,
                Message = result.Data.Message,
                IsRead = result.Data.IsRead,
                ReadDate = result.Data.ReadDate,
                CreatedDate = result.Data.CreatedDate
            };

            return Ok(contact);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting contact by id: {Id}", id);
            return StatusCode(500, new { message = "Bir hata oluştu." });
        }
    }

    /// <summary>
    /// Yeni iletişim mesajı oluşturur (Public)
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateContactRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _contactService.SendMessageAsync(request.Email, request.Message);
            
            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(new { message = "Mesajınız başarıyla gönderildi. En kısa sürede size dönüş yapacağız." });
        }
        catch (FluentValidation.ValidationException validationEx)
        {
            // FluentValidation hatalarını kullanıcı dostu mesajlara çevir
            var errors = validationEx.Errors.Select(e => e.ErrorMessage).ToList();
            var errorMessage = string.Join(" ", errors);
            _logger.LogWarning("Contact validation failed: {Errors}", errorMessage);
            return BadRequest(new { message = errorMessage });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating contact message");
            return StatusCode(500, new { message = "Mesaj gönderilirken bir hata oluştu." });
        }
    }

    /// <summary>
    /// İletişim mesajını okundu olarak işaretler (Admin)
    /// </summary>
    [HttpPut("{id}/mark-as-read")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        try
        {
            var result = await _contactService.MarkAsReadAsync(id);
            
            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(new { message = "Mesaj okundu olarak işaretlendi." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking contact as read: {Id}", id);
            return StatusCode(500, new { message = "Bir hata oluştu." });
        }
    }

    /// <summary>
    /// İletişim mesajını siler (Admin)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var result = await _contactService.DeleteAsync(id);
            
            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting contact: {Id}", id);
            return StatusCode(500, new { message = "Bir hata oluştu." });
        }
    }
}


