using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models.Entities.CocomoThree;

/// <summary>
/// ParameterSet entity representing COCOMO II parameter configurations
/// </summary>
[Table("parameterset")]
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
    // PERS (Personnel Capability)
    [Column("em_pers_xlo")]
    public decimal? EmPersXlo { get; set; }
    
    [Column("em_pers_vlo")]
    public decimal? EmPersVlo { get; set; }
    
    [Column("em_pers_lo")]
    public decimal? EmPersLo { get; set; }
    
    [Column("em_pers_nom")]
    public decimal? EmPersNom { get; set; }
    
    [Column("em_pers_hi")]
    public decimal? EmPersHi { get; set; }
    
    [Column("em_pers_vhi")]
    public decimal? EmPersVhi { get; set; }
    
    [Column("em_pers_xhi")]
    public decimal? EmPersXhi { get; set; }

    // RCPX (Reliability and Complexity)
    [Column("em_rcpx_xlo")]
    public decimal? EmRcpxXlo { get; set; }
    
    [Column("em_rcpx_vlo")]
    public decimal? EmRcpxVlo { get; set; }
    
    [Column("em_rcpx_lo")]
    public decimal? EmRcpxLo { get; set; }
    
    [Column("em_rcpx_nom")]
    public decimal? EmRcpxNom { get; set; }
    
    [Column("em_rcpx_hi")]
    public decimal? EmRcpxHi { get; set; }
    
    [Column("em_rcpx_vhi")]
    public decimal? EmRcpxVhi { get; set; }
    
    [Column("em_rcpx_xhi")]
    public decimal? EmRcpxXhi { get; set; }

    // PDIF (Platform Difficulty)
    [Column("em_pdif_xlo")]
    public decimal? EmPdifXlo { get; set; }
    
    [Column("em_pdif_vlo")]
    public decimal? EmPdifVlo { get; set; }
    
    [Column("em_pdif_lo")]
    public decimal? EmPdifLo { get; set; }
    
    [Column("em_pdif_nom")]
    public decimal? EmPdifNom { get; set; }
    
    [Column("em_pdif_hi")]
    public decimal? EmPdifHi { get; set; }
    
    [Column("em_pdif_vhi")]
    public decimal? EmPdifVhi { get; set; }
    
    [Column("em_pdif_xhi")]
    public decimal? EmPdifXhi { get; set; }

    // PREX (Personnel Experience)
    [Column("em_prex_xlo")]
    public decimal? EmPrexXlo { get; set; }
    
    [Column("em_prex_vlo")]
    public decimal? EmPrexVlo { get; set; }
    
    [Column("em_prex_lo")]
    public decimal? EmPrexLo { get; set; }
    
    [Column("em_prex_nom")]
    public decimal? EmPrexNom { get; set; }
    
    [Column("em_prex_hi")]
    public decimal? EmPrexHi { get; set; }
    
    [Column("em_prex_vhi")]
    public decimal? EmPrexVhi { get; set; }
    
    [Column("em_prex_xhi")]
    public decimal? EmPrexXhi { get; set; }

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

    // FCIL (Facilities)
    [Column("em_fcil_xlo")]
    public decimal? EmFcilXlo { get; set; }
    
    [Column("em_fcil_vlo")]
    public decimal? EmFcilVlo { get; set; }
    
    [Column("em_fcil_lo")]
    public decimal? EmFcilLo { get; set; }
    
    [Column("em_fcil_nom")]
    public decimal? EmFcilNom { get; set; }
    
    [Column("em_fcil_hi")]
    public decimal? EmFcilHi { get; set; }
    
    [Column("em_fcil_vhi")]
    public decimal? EmFcilVhi { get; set; }
    
    [Column("em_fcil_xhi")]
    public decimal? EmFcilXhi { get; set; }

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