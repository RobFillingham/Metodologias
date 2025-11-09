using System.ComponentModel.DataAnnotations;

namespace Backend.Models.DTOs.CocomoIIStage3;

/// <summary>
/// DTO for updating ratings of a COCOMO II Stage 3 Estimation
/// </summary>
public class UpdateEstimationRatingsDto
{
    // Selected ratings - SF
    [StringLength(10)]
    public string? SelectedSfPrec { get; set; }

    [StringLength(10)]
    public string? SelectedSfFlex { get; set; }

    [StringLength(10)]
    public string? SelectedSfResl { get; set; }

    [StringLength(10)]
    public string? SelectedSfTeam { get; set; }

    [StringLength(10)]
    public string? SelectedSfPmat { get; set; }

    // Selected ratings - EM
    [StringLength(10)]
    public string? SelectedEmRely { get; set; }

    [StringLength(10)]
    public string? SelectedEmData { get; set; }

    [StringLength(10)]
    public string? SelectedEmCplx { get; set; }

    [StringLength(10)]
    public string? SelectedEmRuse { get; set; }

    [StringLength(10)]
    public string? SelectedEmDocu { get; set; }

    [StringLength(10)]
    public string? SelectedEmTime { get; set; }

    [StringLength(10)]
    public string? SelectedEmStor { get; set; }

    [StringLength(10)]
    public string? SelectedEmPvol { get; set; }

    [StringLength(10)]
    public string? SelectedEmAcap { get; set; }

    [StringLength(10)]
    public string? SelectedEmPcap { get; set; }

    [StringLength(10)]
    public string? SelectedEmPcon { get; set; }

    [StringLength(10)]
    public string? SelectedEmApex { get; set; }

    [StringLength(10)]
    public string? SelectedEmPlex { get; set; }

    [StringLength(10)]
    public string? SelectedEmLtex { get; set; }

    [StringLength(10)]
    public string? SelectedEmTool { get; set; }

    [StringLength(10)]
    public string? SelectedEmSite { get; set; }

    [StringLength(10)]
    public string? SelectedEmSced { get; set; }
}