using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models.Entities.CocomoIIStage3;

/// <summary>
/// ParameterSet entity representing COCOMO II Stage 3 parameter configurations
/// </summary>
[Table("parametersetcocomoIIstage3")]
public class ParameterSet
{
    [Column("param_set_id")]
    public int ParamSetId { get; set; }

    [Column("UserId")]
    public int? UserId { get; set; } // NULL for system default sets

    [Column("set_name")]
    public string SetName { get; set; } = string.Empty;

    [Column("is_default")]
    public bool IsDefault { get; set; } = false;

    // COCOMO Constants
    [Column("const_A")]
    public decimal ConstA { get; set; } = 2.94m;

    [Column("const_B")]
    public decimal ConstB { get; set; } = 0.91m;

    [Column("const_C")]
    public decimal ConstC { get; set; } = 3.67m;

    [Column("const_D")]
    public decimal ConstD { get; set; } = 0.28m;

    // Scale Factors (SF) - All ratings for each factor
    // PREC (Precedentedness)
    [Column("sf_prec_vlo")]
    public decimal? SfPrecVlo { get; set; }

    [Column("sf_prec_lo")]
    public decimal? SfPrecLo { get; set; }

    [Column("sf_prec_nom")]
    public decimal? SfPrecNom { get; set; }

    [Column("sf_prec_hi")]
    public decimal? SfPrecHi { get; set; }

    [Column("sf_prec_vhi")]
    public decimal? SfPrecVhi { get; set; }

    [Column("sf_prec_xhi")]
    public decimal? SfPrecXhi { get; set; }

    // FLEX (Development Flexibility)
    [Column("sf_flex_vlo")]
    public decimal? SfFlexVlo { get; set; }

    [Column("sf_flex_lo")]
    public decimal? SfFlexLo { get; set; }

    [Column("sf_flex_nom")]
    public decimal? SfFlexNom { get; set; }

    [Column("sf_flex_hi")]
    public decimal? SfFlexHi { get; set; }

    [Column("sf_flex_vhi")]
    public decimal? SfFlexVhi { get; set; }

    [Column("sf_flex_xhi")]
    public decimal? SfFlexXhi { get; set; }

    // RESL (Architecture/Risk Resolution)
    [Column("sf_resl_vlo")]
    public decimal? SfReslVlo { get; set; }

    [Column("sf_resl_lo")]
    public decimal? SfReslLo { get; set; }

    [Column("sf_resl_nom")]
    public decimal? SfReslNom { get; set; }

    [Column("sf_resl_hi")]
    public decimal? SfReslHi { get; set; }

    [Column("sf_resl_vhi")]
    public decimal? SfReslVhi { get; set; }

    [Column("sf_resl_xhi")]
    public decimal? SfReslXhi { get; set; }

    // TEAM (Team Cohesion)
    [Column("sf_team_vlo")]
    public decimal? SfTeamVlo { get; set; }

    [Column("sf_team_lo")]
    public decimal? SfTeamLo { get; set; }

    [Column("sf_team_nom")]
    public decimal? SfTeamNom { get; set; }

    [Column("sf_team_hi")]
    public decimal? SfTeamHi { get; set; }

    [Column("sf_team_vhi")]
    public decimal? SfTeamVhi { get; set; }

    [Column("sf_team_xhi")]
    public decimal? SfTeamXhi { get; set; }

    // PMAT (Process Maturity)
    [Column("sf_pmat_vlo")]
    public decimal? SfPmatVlo { get; set; }

    [Column("sf_pmat_lo")]
    public decimal? SfPmatLo { get; set; }

    [Column("sf_pmat_nom")]
    public decimal? SfPmatNom { get; set; }

    [Column("sf_pmat_hi")]
    public decimal? SfPmatHi { get; set; }

    [Column("sf_pmat_vhi")]
    public decimal? SfPmatVhi { get; set; }

    [Column("sf_pmat_xhi")]
    public decimal? SfPmatXhi { get; set; }

    // Effort Multipliers (EM) - All ratings for each factor
    // RELY (Required Software Reliability)
    [Column("em_rely_xlo")]
    public decimal? EmRelyXlo { get; set; }

    [Column("em_rely_vlo")]
    public decimal? EmRelyVlo { get; set; }

    [Column("em_rely_lo")]
    public decimal? EmRelyLo { get; set; }

    [Column("em_rely_nom")]
    public decimal? EmRelyNom { get; set; }

    [Column("em_rely_hi")]
    public decimal? EmRelyHi { get; set; }

    [Column("em_rely_vhi")]
    public decimal? EmRelyVhi { get; set; }

    [Column("em_rely_xhi")]
    public decimal? EmRelyXhi { get; set; }

    // DATA (Database Size)
    [Column("em_data_xlo")]
    public decimal? EmDataXlo { get; set; }

    [Column("em_data_vlo")]
    public decimal? EmDataVlo { get; set; }

    [Column("em_data_lo")]
    public decimal? EmDataLo { get; set; }

    [Column("em_data_nom")]
    public decimal? EmDataNom { get; set; }

    [Column("em_data_hi")]
    public decimal? EmDataHi { get; set; }

    [Column("em_data_vhi")]
    public decimal? EmDataVhi { get; set; }

    [Column("em_data_xhi")]
    public decimal? EmDataXhi { get; set; }

    // CPLX (Product Complexity)
    [Column("em_cplx_xlo")]
    public decimal? EmCplxXlo { get; set; }

    [Column("em_cplx_vlo")]
    public decimal? EmCplxVlo { get; set; }

    [Column("em_cplx_lo")]
    public decimal? EmCplxLo { get; set; }

    [Column("em_cplx_nom")]
    public decimal? EmCplxNom { get; set; }

    [Column("em_cplx_hi")]
    public decimal? EmCplxHi { get; set; }

    [Column("em_cplx_vhi")]
    public decimal? EmCplxVhi { get; set; }

    [Column("em_cplx_xhi")]
    public decimal? EmCplxXhi { get; set; }

    // RUSE (Required Reusability)
    [Column("em_ruse_xlo")]
    public decimal? EmRuseXlo { get; set; }

    [Column("em_ruse_vlo")]
    public decimal? EmRuseVlo { get; set; }

    [Column("em_ruse_lo")]
    public decimal? EmRuseLo { get; set; }

    [Column("em_ruse_nom")]
    public decimal? EmRuseNom { get; set; }

    [Column("em_ruse_hi")]
    public decimal? EmRuseHi { get; set; }

    [Column("em_ruse_vhi")]
    public decimal? EmRuseVhi { get; set; }

    [Column("em_ruse_xhi")]
    public decimal? EmRuseXhi { get; set; }

    // DOCU (Documentation Match to Life-Cycle Needs)
    [Column("em_docu_xlo")]
    public decimal? EmDocuXlo { get; set; }

    [Column("em_docu_vlo")]
    public decimal? EmDocuVlo { get; set; }

    [Column("em_docu_lo")]
    public decimal? EmDocuLo { get; set; }

    [Column("em_docu_nom")]
    public decimal? EmDocuNom { get; set; }

    [Column("em_docu_hi")]
    public decimal? EmDocuHi { get; set; }

    [Column("em_docu_vhi")]
    public decimal? EmDocuVhi { get; set; }

    [Column("em_docu_xhi")]
    public decimal? EmDocuXhi { get; set; }

    // TIME (Execution Time Constraint)
    [Column("em_time_xlo")]
    public decimal? EmTimeXlo { get; set; }

    [Column("em_time_vlo")]
    public decimal? EmTimeVlo { get; set; }

    [Column("em_time_lo")]
    public decimal? EmTimeLo { get; set; }

    [Column("em_time_nom")]
    public decimal? EmTimeNom { get; set; }

    [Column("em_time_hi")]
    public decimal? EmTimeHi { get; set; }

    [Column("em_time_vhi")]
    public decimal? EmTimeVhi { get; set; }

    [Column("em_time_xhi")]
    public decimal? EmTimeXhi { get; set; }

    // STOR (Main Storage Constraint)
    [Column("em_stor_xlo")]
    public decimal? EmStorXlo { get; set; }

    [Column("em_stor_vlo")]
    public decimal? EmStorVlo { get; set; }

    [Column("em_stor_lo")]
    public decimal? EmStorLo { get; set; }

    [Column("em_stor_nom")]
    public decimal? EmStorNom { get; set; }

    [Column("em_stor_hi")]
    public decimal? EmStorHi { get; set; }

    [Column("em_stor_vhi")]
    public decimal? EmStorVhi { get; set; }

    [Column("em_stor_xhi")]
    public decimal? EmStorXhi { get; set; }

    // PVOL (Platform Volatility)
    [Column("em_pvol_xlo")]
    public decimal? EmPvolXlo { get; set; }

    [Column("em_pvol_vlo")]
    public decimal? EmPvolVlo { get; set; }

    [Column("em_pvol_lo")]
    public decimal? EmPvolLo { get; set; }

    [Column("em_pvol_nom")]
    public decimal? EmPvolNom { get; set; }

    [Column("em_pvol_hi")]
    public decimal? EmPvolHi { get; set; }

    [Column("em_pvol_vhi")]
    public decimal? EmPvolVhi { get; set; }

    [Column("em_pvol_xhi")]
    public decimal? EmPvolXhi { get; set; }

    // ACAP (Analyst Capability)
    [Column("em_acap_xlo")]
    public decimal? EmAcapXlo { get; set; }

    [Column("em_acap_vlo")]
    public decimal? EmAcapVlo { get; set; }

    [Column("em_acap_lo")]
    public decimal? EmAcapLo { get; set; }

    [Column("em_acap_nom")]
    public decimal? EmAcapNom { get; set; }

    [Column("em_acap_hi")]
    public decimal? EmAcapHi { get; set; }

    [Column("em_acap_vhi")]
    public decimal? EmAcapVhi { get; set; }

    [Column("em_acap_xhi")]
    public decimal? EmAcapXhi { get; set; }

    // PCAP (Programmer Capability)
    [Column("em_pcap_xlo")]
    public decimal? EmPcapXlo { get; set; }

    [Column("em_pcap_vlo")]
    public decimal? EmPcapVlo { get; set; }

    [Column("em_pcap_lo")]
    public decimal? EmPcapLo { get; set; }

    [Column("em_pcap_nom")]
    public decimal? EmPcapNom { get; set; }

    [Column("em_pcap_hi")]
    public decimal? EmPcapHi { get; set; }

    [Column("em_pcap_vhi")]
    public decimal? EmPcapVhi { get; set; }

    [Column("em_pcap_xhi")]
    public decimal? EmPcapXhi { get; set; }

    // PCON (Personnel Continuity)
    [Column("em_pcon_xlo")]
    public decimal? EmPconXlo { get; set; }

    [Column("em_pcon_vlo")]
    public decimal? EmPconVlo { get; set; }

    [Column("em_pcon_lo")]
    public decimal? EmPconLo { get; set; }

    [Column("em_pcon_nom")]
    public decimal? EmPconNom { get; set; }

    [Column("em_pcon_hi")]
    public decimal? EmPconHi { get; set; }

    [Column("em_pcon_vhi")]
    public decimal? EmPconVhi { get; set; }

    [Column("em_pcon_xhi")]
    public decimal? EmPconXhi { get; set; }

    // APEX (Applications Experience)
    [Column("em_apex_xlo")]
    public decimal? EmApexXlo { get; set; }

    [Column("em_apex_vlo")]
    public decimal? EmApexVlo { get; set; }

    [Column("em_apex_lo")]
    public decimal? EmApexLo { get; set; }

    [Column("em_apex_nom")]
    public decimal? EmApexNom { get; set; }

    [Column("em_apex_hi")]
    public decimal? EmApexHi { get; set; }

    [Column("em_apex_vhi")]
    public decimal? EmApexVhi { get; set; }

    [Column("em_apex_xhi")]
    public decimal? EmApexXhi { get; set; }

    // PLEX (Platform Experience)
    [Column("em_plex_xlo")]
    public decimal? EmPlexXlo { get; set; }

    [Column("em_plex_vlo")]
    public decimal? EmPlexVlo { get; set; }

    [Column("em_plex_lo")]
    public decimal? EmPlexLo { get; set; }

    [Column("em_plex_nom")]
    public decimal? EmPlexNom { get; set; }

    [Column("em_plex_hi")]
    public decimal? EmPlexHi { get; set; }

    [Column("em_plex_vhi")]
    public decimal? EmPlexVhi { get; set; }

    [Column("em_plex_xhi")]
    public decimal? EmPlexXhi { get; set; }

    // LTEX (Language and Tool Experience)
    [Column("em_ltex_xlo")]
    public decimal? EmLtexXlo { get; set; }

    [Column("em_ltex_vlo")]
    public decimal? EmLtexVlo { get; set; }

    [Column("em_ltex_lo")]
    public decimal? EmLtexLo { get; set; }

    [Column("em_ltex_nom")]
    public decimal? EmLtexNom { get; set; }

    [Column("em_ltex_hi")]
    public decimal? EmLtexHi { get; set; }

    [Column("em_ltex_vhi")]
    public decimal? EmLtexVhi { get; set; }

    [Column("em_ltex_xhi")]
    public decimal? EmLtexXhi { get; set; }

    // TOOL (Use of Software Tools)
    [Column("em_tool_xlo")]
    public decimal? EmToolXlo { get; set; }

    [Column("em_tool_vlo")]
    public decimal? EmToolVlo { get; set; }

    [Column("em_tool_lo")]
    public decimal? EmToolLo { get; set; }

    [Column("em_tool_nom")]
    public decimal? EmToolNom { get; set; }

    [Column("em_tool_hi")]
    public decimal? EmToolHi { get; set; }

    [Column("em_tool_vhi")]
    public decimal? EmToolVhi { get; set; }

    [Column("em_tool_xhi")]
    public decimal? EmToolXhi { get; set; }

    // SITE (Multisite Development)
    [Column("em_site_xlo")]
    public decimal? EmSiteXlo { get; set; }

    [Column("em_site_vlo")]
    public decimal? EmSiteVlo { get; set; }

    [Column("em_site_lo")]
    public decimal? EmSiteLo { get; set; }

    [Column("em_site_nom")]
    public decimal? EmSiteNom { get; set; }

    [Column("em_site_hi")]
    public decimal? EmSiteHi { get; set; }

    [Column("em_site_vhi")]
    public decimal? EmSiteVhi { get; set; }

    [Column("em_site_xhi")]
    public decimal? EmSiteXhi { get; set; }

    // SCED (Schedule)
    [Column("em_sced_xlo")]
    public decimal? EmScedXlo { get; set; }

    [Column("em_sced_vlo")]
    public decimal? EmScedVlo { get; set; }

    [Column("em_sced_lo")]
    public decimal? EmScedLo { get; set; }

    [Column("em_sced_nom")]
    public decimal? EmScedNom { get; set; }

    [Column("em_sced_hi")]
    public decimal? EmScedHi { get; set; }

    [Column("em_sced_vhi")]
    public decimal? EmScedVhi { get; set; }

    [Column("em_sced_xhi")]
    public decimal? EmScedXhi { get; set; }

    // Navigation property
    public User? User { get; set; }
}