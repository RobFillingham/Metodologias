using System.ComponentModel.DataAnnotations;

namespace Backend.Models.DTOs.CocomoThree;

/// <summary>
/// DTO for updating ratings of an Estimation
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
    public string? SelectedEmPers { get; set; }

    [StringLength(10)]
    public string? SelectedEmRcpx { get; set; }

    [StringLength(10)]
    public string? SelectedEmPdif { get; set; }

    [StringLength(10)]
    public string? SelectedEmPrex { get; set; }

    [StringLength(10)]
    public string? SelectedEmRuse { get; set; }

    [StringLength(10)]
    public string? SelectedEmFcil { get; set; }

    [StringLength(10)]
    public string? SelectedEmSced { get; set; }
}
