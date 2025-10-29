namespace Backend.Models.DTOs.CocomoThree;

/// <summary>
/// DTO for updating an existing ParameterSet
/// </summary>
public class UpdateParameterSetDto
{
    public int ParamSetId { get; set; }
    public string SetName { get; set; } = string.Empty;
    public bool IsDefault { get; set; } = false;

    // COCOMO Constants
    public decimal ConstA { get; set; } = 2.94m;
    public decimal ConstB { get; set; } = 0.91m;
    public decimal ConstC { get; set; } = 3.67m;
    public decimal ConstD { get; set; } = 0.28m;

    // Scale Factors (SF) - All ratings for each factor
    // PREC (Precedentedness)
    public decimal? SfPrecVlo { get; set; }
    public decimal? SfPrecLo { get; set; }
    public decimal? SfPrecNom { get; set; }
    public decimal? SfPrecHi { get; set; }
    public decimal? SfPrecVhi { get; set; }
    public decimal? SfPrecXhi { get; set; }

    // FLEX (Development Flexibility)
    public decimal? SfFlexVlo { get; set; }
    public decimal? SfFlexLo { get; set; }
    public decimal? SfFlexNom { get; set; }
    public decimal? SfFlexHi { get; set; }
    public decimal? SfFlexVhi { get; set; }
    public decimal? SfFlexXhi { get; set; }

    // RESL (Architecture/Risk Resolution)
    public decimal? SfReslVlo { get; set; }
    public decimal? SfReslLo { get; set; }
    public decimal? SfReslNom { get; set; }
    public decimal? SfReslHi { get; set; }
    public decimal? SfReslVhi { get; set; }
    public decimal? SfReslXhi { get; set; }

    // TEAM (Team Cohesion)
    public decimal? SfTeamVlo { get; set; }
    public decimal? SfTeamLo { get; set; }
    public decimal? SfTeamNom { get; set; }
    public decimal? SfTeamHi { get; set; }
    public decimal? SfTeamVhi { get; set; }
    public decimal? SfTeamXhi { get; set; }

    // PMAT (Process Maturity)
    public decimal? SfPmatVlo { get; set; }
    public decimal? SfPmatLo { get; set; }
    public decimal? SfPmatNom { get; set; }
    public decimal? SfPmatHi { get; set; }
    public decimal? SfPmatVhi { get; set; }
    public decimal? SfPmatXhi { get; set; }

    // Effort Multipliers (EM) - All ratings for each factor
    // PERS (Personnel Capability)
    public decimal? EmPersXlo { get; set; }
    public decimal? EmPersVlo { get; set; }
    public decimal? EmPersLo { get; set; }
    public decimal? EmPersNom { get; set; }
    public decimal? EmPersHi { get; set; }
    public decimal? EmPersVhi { get; set; }
    public decimal? EmPersXhi { get; set; }

    // RCPX (Reliability and Complexity)
    public decimal? EmRcpxXlo { get; set; }
    public decimal? EmRcpxVlo { get; set; }
    public decimal? EmRcpxLo { get; set; }
    public decimal? EmRcpxNom { get; set; }
    public decimal? EmRcpxHi { get; set; }
    public decimal? EmRcpxVhi { get; set; }
    public decimal? EmRcpxXhi { get; set; }

    // PDIF (Platform Difficulty)
    public decimal? EmPdifXlo { get; set; }
    public decimal? EmPdifVlo { get; set; }
    public decimal? EmPdifLo { get; set; }
    public decimal? EmPdifNom { get; set; }
    public decimal? EmPdifHi { get; set; }
    public decimal? EmPdifVhi { get; set; }
    public decimal? EmPdifXhi { get; set; }

    // PREX (Personnel Experience)
    public decimal? EmPrexXlo { get; set; }
    public decimal? EmPrexVlo { get; set; }
    public decimal? EmPrexLo { get; set; }
    public decimal? EmPrexNom { get; set; }
    public decimal? EmPrexHi { get; set; }
    public decimal? EmPrexVhi { get; set; }
    public decimal? EmPrexXhi { get; set; }

    // RUSE (Required Reusability)
    public decimal? EmRuseXlo { get; set; }
    public decimal? EmRuseVlo { get; set; }
    public decimal? EmRuseLo { get; set; }
    public decimal? EmRuseNom { get; set; }
    public decimal? EmRuseHi { get; set; }
    public decimal? EmRuseVhi { get; set; }
    public decimal? EmRuseXhi { get; set; }

    // FCIL (Facilities)
    public decimal? EmFcilXlo { get; set; }
    public decimal? EmFcilVlo { get; set; }
    public decimal? EmFcilLo { get; set; }
    public decimal? EmFcilNom { get; set; }
    public decimal? EmFcilHi { get; set; }
    public decimal? EmFcilVhi { get; set; }
    public decimal? EmFcilXhi { get; set; }

    // SCED (Schedule)
    public decimal? EmScedXlo { get; set; }
    public decimal? EmScedVlo { get; set; }
    public decimal? EmScedLo { get; set; }
    public decimal? EmScedNom { get; set; }
    public decimal? EmScedHi { get; set; }
    public decimal? EmScedVhi { get; set; }
    public decimal? EmScedXhi { get; set; }
}