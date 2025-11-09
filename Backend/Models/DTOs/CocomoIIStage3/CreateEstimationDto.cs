using System.ComponentModel.DataAnnotations;

namespace Backend.Models.DTOs.CocomoIIStage3;

/// <summary>
/// DTO for creating a new COCOMO II Stage 3 Estimation
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
    public string SelectedEmRely { get; set; } = "NOM";

    [StringLength(10)]
    public string SelectedEmData { get; set; } = "NOM";

    [StringLength(10)]
    public string SelectedEmCplx { get; set; } = "NOM";

    [StringLength(10)]
    public string SelectedEmRuse { get; set; } = "NOM";

    [StringLength(10)]
    public string SelectedEmDocu { get; set; } = "NOM";

    [StringLength(10)]
    public string SelectedEmTime { get; set; } = "NOM";

    [StringLength(10)]
    public string SelectedEmStor { get; set; } = "NOM";

    [StringLength(10)]
    public string SelectedEmPvol { get; set; } = "NOM";

    [StringLength(10)]
    public string SelectedEmAcap { get; set; } = "NOM";

    [StringLength(10)]
    public string SelectedEmPcap { get; set; } = "NOM";

    [StringLength(10)]
    public string SelectedEmPcon { get; set; } = "NOM";

    [StringLength(10)]
    public string SelectedEmApex { get; set; } = "NOM";

    [StringLength(10)]
    public string SelectedEmPlex { get; set; } = "NOM";

    [StringLength(10)]
    public string SelectedEmLtex { get; set; } = "NOM";

    [StringLength(10)]
    public string SelectedEmTool { get; set; } = "NOM";

    [StringLength(10)]
    public string SelectedEmSite { get; set; } = "NOM";

    [StringLength(10)]
    public string SelectedEmSced { get; set; } = "NOM";
}