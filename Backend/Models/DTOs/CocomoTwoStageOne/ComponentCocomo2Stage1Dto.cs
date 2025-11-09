namespace Backend.Models.DTOs.CocomoTwoStageOne;

/// <summary>
/// DTO for Component (COCOMO 2 Stage 1)
/// </summary>
public class ComponentCocomo2Stage1Dto
{
    public int ComponentId { get; set; }
    public int EstimationId { get; set; }
    public string ComponentName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ComponentType { get; set; } = "new";
    public decimal SizeFp { get; set; }
    public int? ReusePercent { get; set; }
    public int? ChangePercent { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// DTO for creating a new Component
/// </summary>
public class CreateComponentCocomo2Stage1Dto
{
    public string ComponentName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ComponentType { get; set; } = "new";
    public decimal SizeFp { get; set; }
    public int? ReusePercent { get; set; }
    public int? ChangePercent { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// DTO for updating a Component
/// </summary>
public class UpdateComponentCocomo2Stage1Dto
{
    public int ComponentId { get; set; }
    public string ComponentName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ComponentType { get; set; } = "new";
    public decimal SizeFp { get; set; }
    public int? ReusePercent { get; set; }
    public int? ChangePercent { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// DTO for batch creating components
/// </summary>
public class CreateBatchComponentsCocomo2Stage1Dto
{
    public List<CreateComponentCocomo2Stage1Dto> Components { get; set; } = new();
}
