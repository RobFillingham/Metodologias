namespace Backend.Models.DTOs.CocomoOne;

public class CreateCocomo1EstimationDto
{
    public int ProjectId { get; set; }
    public string EstimationName { get; set; } = string.Empty;
    public decimal Kloc { get; set; }
    public string Mode { get; set; } = "ORGANIC"; // ORGANIC, SEMI_DETACHED, EMBEDDED
}
