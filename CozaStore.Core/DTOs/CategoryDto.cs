namespace CozaStore.Core.DTOs;

/// <summary>
/// Kategori bilgilerini API'den döndürmek için kullanılan DTO.
/// </summary>
public class CategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime CreatedDate { get; set; }
    public bool IsActive { get; set; }
}


