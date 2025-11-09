namespace Backend.Models.DTOs.CocomoIIStage3;

/// <summary>
/// DTO for updating an existing COCOMO II Stage 3 ParameterSet
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
    // RELY (Required Software Reliability)
    public decimal? EmRelyXlo { get; set; }
    public decimal? EmRelyVlo { get; set; }
    public decimal? EmRelyLo { get; set; }
    public decimal? EmRelyNom { get; set; }
    public decimal? EmRelyHi { get; set; }
    public decimal? EmRelyVhi { get; set; }
    public decimal? EmRelyXhi { get; set; }

    // DATA (Database Size)
    public decimal? EmDataXlo { get; set; }
    public decimal? EmDataVlo { get; set; }
    public decimal? EmDataLo { get; set; }
    public decimal? EmDataNom { get; set; }
    public decimal? EmDataHi { get; set; }
    public decimal? EmDataVhi { get; set; }
    public decimal? EmDataXhi { get; set; }

    // CPLX (Product Complexity)
    public decimal? EmCplxXlo { get; set; }
    public decimal? EmCplxVlo { get; set; }
    public decimal? EmCplxLo { get; set; }
    public decimal? EmCplxNom { get; set; }
    public decimal? EmCplxHi { get; set; }
    public decimal? EmCplxVhi { get; set; }
    public decimal? EmCplxXhi { get; set; }

    // RUSE (Required Reusability)
    public decimal? EmRuseXlo { get; set; }
    public decimal? EmRuseVlo { get; set; }
    public decimal? EmRuseLo { get; set; }
    public decimal? EmRuseNom { get; set; }
    public decimal? EmRuseHi { get; set; }
    public decimal? EmRuseVhi { get; set; }
    public decimal? EmRuseXhi { get; set; }

    // DOCU (Documentation Match to Life-Cycle Needs)
    public decimal? EmDocuXlo { get; set; }
    public decimal? EmDocuVlo { get; set; }
    public decimal? EmDocuLo { get; set; }
    public decimal? EmDocuNom { get; set; }
    public decimal? EmDocuHi { get; set; }
    public decimal? EmDocuVhi { get; set; }
    public decimal? EmDocuXhi { get; set; }

    // TIME (Execution Time Constraint)
    public decimal? EmTimeXlo { get; set; }
    public decimal? EmTimeVlo { get; set; }
    public decimal? EmTimeLo { get; set; }
    public decimal? EmTimeNom { get; set; }
    public decimal? EmTimeHi { get; set; }
    public decimal? EmTimeVhi { get; set; }
    public decimal? EmTimeXhi { get; set; }

    // STOR (Main Storage Constraint)
    public decimal? EmStorXlo { get; set; }
    public decimal? EmStorVlo { get; set; }
    public decimal? EmStorLo { get; set; }
    public decimal? EmStorNom { get; set; }
    public decimal? EmStorHi { get; set; }
    public decimal? EmStorVhi { get; set; }
    public decimal? EmStorXhi { get; set; }

    // PVOL (Platform Volatility)
    public decimal? EmPvolXlo { get; set; }
    public decimal? EmPvolVlo { get; set; }
    public decimal? EmPvolLo { get; set; }
    public decimal? EmPvolNom { get; set; }
    public decimal? EmPvolHi { get; set; }
    public decimal? EmPvolVhi { get; set; }
    public decimal? EmPvolXhi { get; set; }

    // ACAP (Analyst Capability)
    public decimal? EmAcapXlo { get; set; }
    public decimal? EmAcapVlo { get; set; }
    public decimal? EmAcapLo { get; set; }
    public decimal? EmAcapNom { get; set; }
    public decimal? EmAcapHi { get; set; }
    public decimal? EmAcapVhi { get; set; }
    public decimal? EmAcapXhi { get; set; }

    // PCAP (Programmer Capability)
    public decimal? EmPcapXlo { get; set; }
    public decimal? EmPcapVlo { get; set; }
    public decimal? EmPcapLo { get; set; }
    public decimal? EmPcapNom { get; set; }
    public decimal? EmPcapHi { get; set; }
    public decimal? EmPcapVhi { get; set; }
    public decimal? EmPcapXhi { get; set; }

    // PCON (Personnel Continuity)
    public decimal? EmPconXlo { get; set; }
    public decimal? EmPconVlo { get; set; }
    public decimal? EmPconLo { get; set; }
    public decimal? EmPconNom { get; set; }
    public decimal? EmPconHi { get; set; }
    public decimal? EmPconVhi { get; set; }
    public decimal? EmPconXhi { get; set; }

    // APEX (Applications Experience)
    public decimal? EmApexXlo { get; set; }
    public decimal? EmApexVlo { get; set; }
    public decimal? EmApexLo { get; set; }
    public decimal? EmApexNom { get; set; }
    public decimal? EmApexHi { get; set; }
    public decimal? EmApexVhi { get; set; }
    public decimal? EmApexXhi { get; set; }

    // PLEX (Platform Experience)
    public decimal? EmPlexXlo { get; set; }
    public decimal? EmPlexVlo { get; set; }
    public decimal? EmPlexLo { get; set; }
    public decimal? EmPlexNom { get; set; }
    public decimal? EmPlexHi { get; set; }
    public decimal? EmPlexVhi { get; set; }
    public decimal? EmPlexXhi { get; set; }

    // LTEX (Language and Tool Experience)
    public decimal? EmLtexXlo { get; set; }
    public decimal? EmLtexVlo { get; set; }
    public decimal? EmLtexLo { get; set; }
    public decimal? EmLtexNom { get; set; }
    public decimal? EmLtexHi { get; set; }
    public decimal? EmLtexVhi { get; set; }
    public decimal? EmLtexXhi { get; set; }

    // TOOL (Use of Software Tools)
    public decimal? EmToolXlo { get; set; }
    public decimal? EmToolVlo { get; set; }
    public decimal? EmToolLo { get; set; }
    public decimal? EmToolNom { get; set; }
    public decimal? EmToolHi { get; set; }
    public decimal? EmToolVhi { get; set; }
    public decimal? EmToolXhi { get; set; }

    // SITE (Multisite Development)
    public decimal? EmSiteXlo { get; set; }
    public decimal? EmSiteVlo { get; set; }
    public decimal? EmSiteLo { get; set; }
    public decimal? EmSiteNom { get; set; }
    public decimal? EmSiteHi { get; set; }
    public decimal? EmSiteVhi { get; set; }
    public decimal? EmSiteXhi { get; set; }

    // SCED (Schedule)
    public decimal? EmScedXlo { get; set; }
    public decimal? EmScedVlo { get; set; }
    public decimal? EmScedLo { get; set; }
    public decimal? EmScedNom { get; set; }
    public decimal? EmScedHi { get; set; }
    public decimal? EmScedVhi { get; set; }
    public decimal? EmScedXhi { get; set; }
}