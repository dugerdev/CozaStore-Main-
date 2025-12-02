namespace CozaStore.Core.DTOs;

public class ContactDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime? ReadDate { get; set; }
    public DateTime CreatedDate { get; set; }
}



