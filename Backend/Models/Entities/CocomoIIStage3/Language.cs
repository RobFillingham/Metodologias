using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models.Entities.CocomoIIStage3;

/// <summary>
/// Language entity representing programming languages with SLOC conversion factors
/// </summary>
[Table("languagecocomoIIstage3")]
public class Language
{
    [Column("language_id")]
    public int LanguageId { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("sloc_per_ufp")]
    public decimal SlocPerUfp { get; set; }
}