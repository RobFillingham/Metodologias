using System.ComponentModel.DataAnnotations;

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

/// <summary>
/// DTO for creating a new language
/// </summary>
public class CreateLanguageDto
{
    /// <summary>
    /// The name of the programming language
    /// </summary>
    [Required(ErrorMessage = "Language name is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Language name must be between 1 and 100 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Source Lines of Code per Unadjusted Function Point
    /// </summary>
    [Required(ErrorMessage = "SLOC per UFP is required")]
    [Range(0.1, 10000, ErrorMessage = "SLOC per UFP must be between 0.1 and 10000")]
    public decimal SlocPerUfp { get; set; }
}

/// <summary>
/// DTO for updating an existing language
/// </summary>
public class UpdateLanguageDto
{
    /// <summary>
    /// The name of the programming language
    /// </summary>
    [Required(ErrorMessage = "Language name is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Language name must be between 1 and 100 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Source Lines of Code per Unadjusted Function Point
    /// </summary>
    [Required(ErrorMessage = "SLOC per UFP is required")]
    [Range(0.1, 10000, ErrorMessage = "SLOC per UFP must be between 0.1 and 10000")]
    public decimal SlocPerUfp { get; set; }
}


