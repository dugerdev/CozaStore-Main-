using CozaStore.Business.Contracts;
using CozaStore.Core.DTOs;
using CozaStore.Entities.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CozaStoreWebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AddressesController : ControllerBase
{
    private readonly IAddressService _addressService;

    public AddressesController(IAddressService addressService)
    {
        _addressService = addressService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyAddresses()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var result = await _addressService.GetByUserAsync(userId);
        
        if (!result.Success)
        {
            return BadRequest(new { message = result.Message });
        }

        var addresses = result.Data.Select(a => new AddressDto
        {
            Id = a.Id,
            UserId = a.UserId,
            Title = a.Title,
            AddressLine1 = a.AddressLine1,
            AddressLine2 = a.AddressLine2,
            City = a.City,
            District = a.District,
            PostalCode = a.PostalCode,
            Country = a.Country,
            AddressType = a.AddressType,
            IsDefault = a.IsDefault
        }).ToList();

        return Ok(addresses);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _addressService.GetByIdAsync(id);
        
        if (!result.Success || result.Data == null)
        {
            return NotFound(new { message = result.Message });
        }

        var address = result.Data;
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (address.UserId != userId && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        var addressDto = new AddressDto
        {
            Id = address.Id,
            UserId = address.UserId,
            Title = address.Title,
            AddressLine1 = address.AddressLine1,
            AddressLine2 = address.AddressLine2,
            City = address.City,
            District = address.District,
            PostalCode = address.PostalCode,
            Country = address.Country,
            AddressType = address.AddressType,
            IsDefault = address.IsDefault
        };

        return Ok(addressDto);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Address address)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        address.UserId = userId;

        var result = await _addressService.AddAsync(address);
        
        if (!result.Success)
        {
            return BadRequest(new { message = result.Message });
        }

        return CreatedAtAction(nameof(GetById), new { id = address.Id }, address);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Address address)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        // Mevcut adresi kontrol et
        var existingResult = await _addressService.GetByIdAsync(id);
        if (!existingResult.Success || existingResult.Data == null)
        {
            return NotFound(new { message = "Adres bulunamadı." });
        }

        var existingAddress = existingResult.Data;

        if (existingAddress.UserId != userId && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        if (id != address.Id)
        {
            return BadRequest(new { message = "ID uyuşmazlığı." });
        }

        // Mevcut entity'yi güncelle (track edilen entity'yi kullan)
        // Yeni entity oluşturmak yerine mevcut entity'nin property'lerini güncelle
        existingAddress.Title = address.Title;
        existingAddress.AddressLine1 = address.AddressLine1;
        existingAddress.AddressLine2 = address.AddressLine2;
        existingAddress.City = address.City;
        existingAddress.District = address.District;
        existingAddress.PostalCode = address.PostalCode;
        existingAddress.Country = address.Country;
        existingAddress.AddressType = address.AddressType;
        existingAddress.IsDefault = address.IsDefault;
        // UserId zaten doğru, değiştirmiyoruz
        // BaseEntity property'leri (CreatedDate, IsActive, IsDeleted) değiştirilmiyor

        var result = await _addressService.UpdateAsync(existingAddress);
        
        if (!result.Success)
        {
            return BadRequest(new { message = result.Message });
        }

        return NoContent();
    }

    [HttpPut("{id}/set-default")]
    public async Task<IActionResult> SetDefault(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var result = await _addressService.SetDefaultAsync(userId, id);
        
        if (!result.Success)
        {
            return BadRequest(new { message = result.Message });
        }

        return Ok(new { message = "Varsayılan adres güncellendi." });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        // Mevcut adresi kontrol et
        var existingResult = await _addressService.GetByIdAsync(id);
        if (!existingResult.Success || existingResult.Data == null)
        {
            return NotFound(new { message = "Adres bulunamadı." });
        }

        if (existingResult.Data.UserId != userId && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        var result = await _addressService.DeleteAsync(id);
        
        if (!result.Success)
        {
            return BadRequest(new { message = result.Message });
        }

        return NoContent();
    }
}

