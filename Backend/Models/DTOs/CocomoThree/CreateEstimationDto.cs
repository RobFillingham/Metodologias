using System.ComponentModel.DataAnnotations;

namespace Backend.Models.DTOs.CocomoThree;

/// <summary>
/// DTO for creating a new Estimation
/// </summary>
public class CreateEstimationDto
{
    [Required(ErrorMessage = "Estimation name is required")]
    [StringLength(255, ErrorMessage = "Estimation name cannot exceed 255 characters")]
    public string EstimationName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Parameter set ID is required")]
    public int ParamSetId { get; set; }

    [Required(ErrorMessage = "Language ID is required")]
    public int LanguageId { get; set; }

    // Selected ratings - SF (optional, defaults to "NOM")
    [StringLength(10)]
    public string SelectedSfPrec { get; set; } = "NOM";

    [StringLength(10)]
    public string SelectedSfFlex { get; set; } = "NOM";

    [StringLength(10)]
    public string SelectedSfResl { get; set; } = "NOM";

    [StringLength(10)]
    public string SelectedSfTeam { get; set; } = "NOM";

    [StringLength(10)]
    public string SelectedSfPmat { get; set; } = "NOM";

    // Selected ratings - EM (optional, defaults to "NOM")
    [StringLength(10)]
    public string SelectedEmPers { get; set; } = "NOM";

    [StringLength(10)]
    public string SelectedEmRcpx { get; set; } = "NOM";

    [StringLength(10)]
    public string SelectedEmPdif { get; set; } = "NOM";

    [StringLength(10)]
    public string SelectedEmPrex { get; set; } = "NOM";

    [StringLength(10)]
    public string SelectedEmRuse { get; set; } = "NOM";

    [StringLength(10)]
    public string SelectedEmFcil { get; set; } = "NOM";

    [StringLength(10)]
    public string SelectedEmSced { get; set; } = "NOM";
}
