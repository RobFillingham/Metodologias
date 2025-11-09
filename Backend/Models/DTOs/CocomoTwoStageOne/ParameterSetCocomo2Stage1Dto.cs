namespace Backend.Models.DTOs.CocomoTwoStageOne;

/// <summary>
/// DTO for Parameter Set (COCOMO 2 Stage 1)
/// </summary>
public class ParameterSetCocomo2Stage1Dto
{
    public int ParamSetId { get; set; }
    public int? UserId { get; set; }
    public string SetName { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public DateTime CreatedAt { get; set; }

    // Constants
    public decimal ConstA { get; set; }
    public decimal ConstB { get; set; }

    // AEXP
    public decimal? AexpVeryLow { get; set; }
    public decimal? AexpLow { get; set; }
    public decimal? AexpNominal { get; set; }
    public decimal? AexpHigh { get; set; }

    // PEXP
    public decimal? PexpVeryLow { get; set; }
    public decimal? PexpLow { get; set; }
    public decimal? PexpNominal { get; set; }
    public decimal? PexpHigh { get; set; }

    // PREC
    public decimal? PrecVeryLow { get; set; }
    public decimal? PrecLow { get; set; }
    public decimal? PrecNominal { get; set; }
    public decimal? PrecHigh { get; set; }

    // RELY
    public decimal? RelyLow { get; set; }
    public decimal? RelyNominal { get; set; }
    public decimal? RelyHigh { get; set; }

    // TMSP
    public decimal? TmspLow { get; set; }
    public decimal? TmspNominal { get; set; }
    public decimal? TmspHigh { get; set; }
}

/// <summary>
/// DTO for creating a new Parameter Set
/// </summary>
public class CreateParameterSetCocomo2Stage1Dto
{
    public string SetName { get; set; } = string.Empty;
    public decimal ConstA { get; set; } = 2.45m;
    public decimal ConstB { get; set; } = 1.10m;

    // AEXP
    public decimal? AexpVeryLow { get; set; }
    public decimal? AexpLow { get; set; }
    public decimal? AexpNominal { get; set; }
    public decimal? AexpHigh { get; set; }

    // PEXP
    public decimal? PexpVeryLow { get; set; }
    public decimal? PexpLow { get; set; }
    public decimal? PexpNominal { get; set; }
    public decimal? PexpHigh { get; set; }

    // PREC
    public decimal? PrecVeryLow { get; set; }
    public decimal? PrecLow { get; set; }
    public decimal? PrecNominal { get; set; }
    public decimal? PrecHigh { get; set; }

    // RELY
    public decimal? RelyLow { get; set; }
    public decimal? RelyNominal { get; set; }
    public decimal? RelyHigh { get; set; }

    // TMSP
    public decimal? TmspLow { get; set; }
    public decimal? TmspNominal { get; set; }
    public decimal? TmspHigh { get; set; }
}
