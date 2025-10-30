namespace Backend.Models.DTOs.CocomoThree;

/// <summary>
/// DTO for Language entity
/// </summary>
public class LanguageDto
{
    public int LanguageId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal SlocPerUfp { get; set; }
}
