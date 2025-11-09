using Microsoft.EntityFrameworkCore;
using Backend.Models.Entities;
using Backend.Models.Entities.CocomoThree;
using Backend.Models.Entities.CocomoOne;
using Backend.Models.Entities.CocomoTwoStageOne;

namespace Backend.Data.Context;

/// <summary>
/// Application database context
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<ParameterSet> ParameterSets { get; set; }
    public DbSet<Language> Languages { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Estimation> Estimations { get; set; }
    public DbSet<EstimationFunction> EstimationFunctions { get; set; }
    public DbSet<Cocomo1Estimation> Cocomo1Estimations { get; set; }
    public DbSet<KlocEstimation> KlocEstimations { get; set; }
    public DbSet<FunctionPointEstimation> FunctionPointEstimations { get; set; }
    public DbSet<FunctionPointCharacteristic> FunctionPointCharacteristics { get; set; }
    public DbSet<UseCasePointEstimation> UseCasePointEstimations { get; set; }
    public DbSet<UseCaseTechnicalFactor> UseCaseTechnicalFactors { get; set; }
    public DbSet<UseCaseEnvironmentFactor> UseCaseEnvironmentFactors { get; set; }
    
    // COCOMO 2 Stage 1 (Composition Model)
    public DbSet<ParameterSetCocomo2Stage1> ParameterSetsCocomo2Stage1 { get; set; }
    public DbSet<EstimationCocomo2Stage1> EstimationsCocomo2Stage1 { get; set; }
    public DbSet<EstimationComponentCocomo2Stage1> EstimationComponentsCocomo2Stage1 { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User entity configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(255);
            
            entity.HasIndex(e => e.Email)
                .IsUnique();
            
            entity.Property(e => e.PasswordHash)
                .IsRequired()
                .HasMaxLength(500);
            
            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(e => e.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            
            entity.Property(e => e.UpdatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6)");
            
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValue(true);
        });

        // ParameterSet entity configuration
        modelBuilder.Entity<ParameterSet>(entity =>
        {
            entity.HasKey(e => e.ParamSetId);

            entity.Property(e => e.SetName)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.IsDefault)
                .IsRequired()
                .HasDefaultValue(false);

            // COCOMO Constants with defaults and precision
            entity.Property(e => e.ConstA)
                .HasPrecision(10, 4)
                .HasDefaultValue(2.94m);

            entity.Property(e => e.ConstB)
                .HasPrecision(10, 4)
                .HasDefaultValue(0.91m);

            entity.Property(e => e.ConstC)
                .HasPrecision(10, 4)
                .HasDefaultValue(3.67m);

            entity.Property(e => e.ConstD)
                .HasPrecision(10, 4)
                .HasDefaultValue(0.28m);

            // Scale Factors with precision
            entity.Property(e => e.SfPrecVlo).HasPrecision(10, 4);
            entity.Property(e => e.SfPrecLo).HasPrecision(10, 4);
            entity.Property(e => e.SfPrecNom).HasPrecision(10, 4);
            entity.Property(e => e.SfPrecHi).HasPrecision(10, 4);
            entity.Property(e => e.SfPrecVhi).HasPrecision(10, 4);
            entity.Property(e => e.SfPrecXhi).HasPrecision(10, 4);

            entity.Property(e => e.SfFlexVlo).HasPrecision(10, 4);
            entity.Property(e => e.SfFlexLo).HasPrecision(10, 4);
            entity.Property(e => e.SfFlexNom).HasPrecision(10, 4);
            entity.Property(e => e.SfFlexHi).HasPrecision(10, 4);
            entity.Property(e => e.SfFlexVhi).HasPrecision(10, 4);
            entity.Property(e => e.SfFlexXhi).HasPrecision(10, 4);

            entity.Property(e => e.SfReslVlo).HasPrecision(10, 4);
            entity.Property(e => e.SfReslLo).HasPrecision(10, 4);
            entity.Property(e => e.SfReslNom).HasPrecision(10, 4);
            entity.Property(e => e.SfReslHi).HasPrecision(10, 4);
            entity.Property(e => e.SfReslVhi).HasPrecision(10, 4);
            entity.Property(e => e.SfReslXhi).HasPrecision(10, 4);

            entity.Property(e => e.SfTeamVlo).HasPrecision(10, 4);
            entity.Property(e => e.SfTeamLo).HasPrecision(10, 4);
            entity.Property(e => e.SfTeamNom).HasPrecision(10, 4);
            entity.Property(e => e.SfTeamHi).HasPrecision(10, 4);
            entity.Property(e => e.SfTeamVhi).HasPrecision(10, 4);
            entity.Property(e => e.SfTeamXhi).HasPrecision(10, 4);

            entity.Property(e => e.SfPmatVlo).HasPrecision(10, 4);
            entity.Property(e => e.SfPmatLo).HasPrecision(10, 4);
            entity.Property(e => e.SfPmatNom).HasPrecision(10, 4);
            entity.Property(e => e.SfPmatHi).HasPrecision(10, 4);
            entity.Property(e => e.SfPmatVhi).HasPrecision(10, 4);
            entity.Property(e => e.SfPmatXhi).HasPrecision(10, 4);

            // Effort Multipliers with precision
            entity.Property(e => e.EmPersXlo).HasPrecision(10, 4);
            entity.Property(e => e.EmPersVlo).HasPrecision(10, 4);
            entity.Property(e => e.EmPersLo).HasPrecision(10, 4);
            entity.Property(e => e.EmPersNom).HasPrecision(10, 4);
            entity.Property(e => e.EmPersHi).HasPrecision(10, 4);
            entity.Property(e => e.EmPersVhi).HasPrecision(10, 4);
            entity.Property(e => e.EmPersXhi).HasPrecision(10, 4);

            entity.Property(e => e.EmRcpxXlo).HasPrecision(10, 4);
            entity.Property(e => e.EmRcpxVlo).HasPrecision(10, 4);
            entity.Property(e => e.EmRcpxLo).HasPrecision(10, 4);
            entity.Property(e => e.EmRcpxNom).HasPrecision(10, 4);
            entity.Property(e => e.EmRcpxHi).HasPrecision(10, 4);
            entity.Property(e => e.EmRcpxVhi).HasPrecision(10, 4);
            entity.Property(e => e.EmRcpxXhi).HasPrecision(10, 4);

            entity.Property(e => e.EmPdifXlo).HasPrecision(10, 4);
            entity.Property(e => e.EmPdifVlo).HasPrecision(10, 4);
            entity.Property(e => e.EmPdifLo).HasPrecision(10, 4);
            entity.Property(e => e.EmPdifNom).HasPrecision(10, 4);
            entity.Property(e => e.EmPdifHi).HasPrecision(10, 4);
            entity.Property(e => e.EmPdifVhi).HasPrecision(10, 4);
            entity.Property(e => e.EmPdifXhi).HasPrecision(10, 4);

            entity.Property(e => e.EmPrexXlo).HasPrecision(10, 4);
            entity.Property(e => e.EmPrexVlo).HasPrecision(10, 4);
            entity.Property(e => e.EmPrexLo).HasPrecision(10, 4);
            entity.Property(e => e.EmPrexNom).HasPrecision(10, 4);
            entity.Property(e => e.EmPrexHi).HasPrecision(10, 4);
            entity.Property(e => e.EmPrexVhi).HasPrecision(10, 4);
            entity.Property(e => e.EmPrexXhi).HasPrecision(10, 4);

            entity.Property(e => e.EmRuseXlo).HasPrecision(10, 4);
            entity.Property(e => e.EmRuseVlo).HasPrecision(10, 4);
            entity.Property(e => e.EmRuseLo).HasPrecision(10, 4);
            entity.Property(e => e.EmRuseNom).HasPrecision(10, 4);
            entity.Property(e => e.EmRuseHi).HasPrecision(10, 4);
            entity.Property(e => e.EmRuseVhi).HasPrecision(10, 4);
            entity.Property(e => e.EmRuseXhi).HasPrecision(10, 4);

            entity.Property(e => e.EmFcilXlo).HasPrecision(10, 4);
            entity.Property(e => e.EmFcilVlo).HasPrecision(10, 4);
            entity.Property(e => e.EmFcilLo).HasPrecision(10, 4);
            entity.Property(e => e.EmFcilNom).HasPrecision(10, 4);
            entity.Property(e => e.EmFcilHi).HasPrecision(10, 4);
            entity.Property(e => e.EmFcilVhi).HasPrecision(10, 4);
            entity.Property(e => e.EmFcilXhi).HasPrecision(10, 4);

            entity.Property(e => e.EmScedXlo).HasPrecision(10, 4);
            entity.Property(e => e.EmScedVlo).HasPrecision(10, 4);
            entity.Property(e => e.EmScedLo).HasPrecision(10, 4);
            entity.Property(e => e.EmScedNom).HasPrecision(10, 4);
            entity.Property(e => e.EmScedHi).HasPrecision(10, 4);
            entity.Property(e => e.EmScedVhi).HasPrecision(10, 4);
            entity.Property(e => e.EmScedXhi).HasPrecision(10, 4);

            // Foreign key relationship
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Explicit column mappings to match the SQL schema
            entity.Property(e => e.ParamSetId).HasColumnName("param_set_id");
            entity.Property(e => e.UserId).HasColumnName("UserId");
            entity.Property(e => e.SetName).HasColumnName("set_name");
            entity.Property(e => e.IsDefault).HasColumnName("is_default");

            entity.Property(e => e.ConstA).HasColumnName("const_A");
            entity.Property(e => e.ConstB).HasColumnName("const_B");
            entity.Property(e => e.ConstC).HasColumnName("const_C");
            entity.Property(e => e.ConstD).HasColumnName("const_D");

            // Scale Factors
            entity.Property(e => e.SfPrecVlo).HasColumnName("sf_prec_vlo");
            entity.Property(e => e.SfPrecLo).HasColumnName("sf_prec_lo");
            entity.Property(e => e.SfPrecNom).HasColumnName("sf_prec_nom");
            entity.Property(e => e.SfPrecHi).HasColumnName("sf_prec_hi");
            entity.Property(e => e.SfPrecVhi).HasColumnName("sf_prec_vhi");
            entity.Property(e => e.SfPrecXhi).HasColumnName("sf_prec_xhi");

            entity.Property(e => e.SfFlexVlo).HasColumnName("sf_flex_vlo");
            entity.Property(e => e.SfFlexLo).HasColumnName("sf_flex_lo");
            entity.Property(e => e.SfFlexNom).HasColumnName("sf_flex_nom");
            entity.Property(e => e.SfFlexHi).HasColumnName("sf_flex_hi");
            entity.Property(e => e.SfFlexVhi).HasColumnName("sf_flex_vhi");
            entity.Property(e => e.SfFlexXhi).HasColumnName("sf_flex_xhi");

            entity.Property(e => e.SfReslVlo).HasColumnName("sf_resl_vlo");
            entity.Property(e => e.SfReslLo).HasColumnName("sf_resl_lo");
            entity.Property(e => e.SfReslNom).HasColumnName("sf_resl_nom");
            entity.Property(e => e.SfReslHi).HasColumnName("sf_resl_hi");
            entity.Property(e => e.SfReslVhi).HasColumnName("sf_resl_vhi");
            entity.Property(e => e.SfReslXhi).HasColumnName("sf_resl_xhi");

            entity.Property(e => e.SfTeamVlo).HasColumnName("sf_team_vlo");
            entity.Property(e => e.SfTeamLo).HasColumnName("sf_team_lo");
            entity.Property(e => e.SfTeamNom).HasColumnName("sf_team_nom");
            entity.Property(e => e.SfTeamHi).HasColumnName("sf_team_hi");
            entity.Property(e => e.SfTeamVhi).HasColumnName("sf_team_vhi");
            entity.Property(e => e.SfTeamXhi).HasColumnName("sf_team_xhi");

            entity.Property(e => e.SfPmatVlo).HasColumnName("sf_pmat_vlo");
            entity.Property(e => e.SfPmatLo).HasColumnName("sf_pmat_lo");
            entity.Property(e => e.SfPmatNom).HasColumnName("sf_pmat_nom");
            entity.Property(e => e.SfPmatHi).HasColumnName("sf_pmat_hi");
            entity.Property(e => e.SfPmatVhi).HasColumnName("sf_pmat_vhi");
            entity.Property(e => e.SfPmatXhi).HasColumnName("sf_pmat_xhi");

            // Effort Multipliers
            entity.Property(e => e.EmPersXlo).HasColumnName("em_pers_xlo");
            entity.Property(e => e.EmPersVlo).HasColumnName("em_pers_vlo");
            entity.Property(e => e.EmPersLo).HasColumnName("em_pers_lo");
            entity.Property(e => e.EmPersNom).HasColumnName("em_pers_nom");
            entity.Property(e => e.EmPersHi).HasColumnName("em_pers_hi");
            entity.Property(e => e.EmPersVhi).HasColumnName("em_pers_vhi");
            entity.Property(e => e.EmPersXhi).HasColumnName("em_pers_xhi");

            entity.Property(e => e.EmRcpxXlo).HasColumnName("em_rcpx_xlo");
            entity.Property(e => e.EmRcpxVlo).HasColumnName("em_rcpx_vlo");
            entity.Property(e => e.EmRcpxLo).HasColumnName("em_rcpx_lo");
            entity.Property(e => e.EmRcpxNom).HasColumnName("em_rcpx_nom");
            entity.Property(e => e.EmRcpxHi).HasColumnName("em_rcpx_hi");
            entity.Property(e => e.EmRcpxVhi).HasColumnName("em_rcpx_vhi");
            entity.Property(e => e.EmRcpxXhi).HasColumnName("em_rcpx_xhi");

            entity.Property(e => e.EmPdifXlo).HasColumnName("em_pdif_xlo");
            entity.Property(e => e.EmPdifVlo).HasColumnName("em_pdif_vlo");
            entity.Property(e => e.EmPdifLo).HasColumnName("em_pdif_lo");
            entity.Property(e => e.EmPdifNom).HasColumnName("em_pdif_nom");
            entity.Property(e => e.EmPdifHi).HasColumnName("em_pdif_hi");
            entity.Property(e => e.EmPdifVhi).HasColumnName("em_pdif_vhi");
            entity.Property(e => e.EmPdifXhi).HasColumnName("em_pdif_xhi");

            entity.Property(e => e.EmPrexXlo).HasColumnName("em_prex_xlo");
            entity.Property(e => e.EmPrexVlo).HasColumnName("em_prex_vlo");
            entity.Property(e => e.EmPrexLo).HasColumnName("em_prex_lo");
            entity.Property(e => e.EmPrexNom).HasColumnName("em_prex_nom");
            entity.Property(e => e.EmPrexHi).HasColumnName("em_prex_hi");
            entity.Property(e => e.EmPrexVhi).HasColumnName("em_prex_vhi");
            entity.Property(e => e.EmPrexXhi).HasColumnName("em_prex_xhi");

            entity.Property(e => e.EmRuseXlo).HasColumnName("em_ruse_xlo");
            entity.Property(e => e.EmRuseVlo).HasColumnName("em_ruse_vlo");
            entity.Property(e => e.EmRuseLo).HasColumnName("em_ruse_lo");
            entity.Property(e => e.EmRuseNom).HasColumnName("em_ruse_nom");
            entity.Property(e => e.EmRuseHi).HasColumnName("em_ruse_hi");
            entity.Property(e => e.EmRuseVhi).HasColumnName("em_ruse_vhi");
            entity.Property(e => e.EmRuseXhi).HasColumnName("em_ruse_xhi");

            entity.Property(e => e.EmFcilXlo).HasColumnName("em_fcil_xlo");
            entity.Property(e => e.EmFcilVlo).HasColumnName("em_fcil_vlo");
            entity.Property(e => e.EmFcilLo).HasColumnName("em_fcil_lo");
            entity.Property(e => e.EmFcilNom).HasColumnName("em_fcil_nom");
            entity.Property(e => e.EmFcilHi).HasColumnName("em_fcil_hi");
            entity.Property(e => e.EmFcilVhi).HasColumnName("em_fcil_vhi");
            entity.Property(e => e.EmFcilXhi).HasColumnName("em_fcil_xhi");

            entity.Property(e => e.EmScedXlo).HasColumnName("em_sced_xlo");
            entity.Property(e => e.EmScedVlo).HasColumnName("em_sced_vlo");
            entity.Property(e => e.EmScedLo).HasColumnName("em_sced_lo");
            entity.Property(e => e.EmScedNom).HasColumnName("em_sced_nom");
            entity.Property(e => e.EmScedHi).HasColumnName("em_sced_hi");
            entity.Property(e => e.EmScedVhi).HasColumnName("em_sced_vhi");
            entity.Property(e => e.EmScedXhi).HasColumnName("em_sced_xhi");
        });

        // Language entity configuration
        modelBuilder.Entity<Language>(entity =>
        {
            entity.HasKey(e => e.LanguageId);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.SlocPerUfp)
                .IsRequired()
                .HasPrecision(10, 2);
        });

        // Project entity configuration
        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.ProjectId);

            entity.Property(e => e.ProjectName)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.Description)
                .HasColumnType("TEXT");

            entity.Property(e => e.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

            // Foreign key relationship
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Estimation entity configuration
        modelBuilder.Entity<Estimation>(entity =>
        {
            entity.HasKey(e => e.EstimationId);

            entity.Property(e => e.EstimationName)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

            // Selected ratings - SF
            entity.Property(e => e.SelectedSfPrec)
                .IsRequired()
                .HasMaxLength(10)
                .HasDefaultValue("NOM");

            entity.Property(e => e.SelectedSfFlex)
                .IsRequired()
                .HasMaxLength(10)
                .HasDefaultValue("NOM");

            entity.Property(e => e.SelectedSfResl)
                .IsRequired()
                .HasMaxLength(10)
                .HasDefaultValue("NOM");

            entity.Property(e => e.SelectedSfTeam)
                .IsRequired()
                .HasMaxLength(10)
                .HasDefaultValue("NOM");

            entity.Property(e => e.SelectedSfPmat)
                .IsRequired()
                .HasMaxLength(10)
                .HasDefaultValue("NOM");

            // Selected ratings - EM
            entity.Property(e => e.SelectedEmPers)
                .IsRequired()
                .HasMaxLength(10)
                .HasDefaultValue("NOM");

            entity.Property(e => e.SelectedEmRcpx)
                .IsRequired()
                .HasMaxLength(10)
                .HasDefaultValue("NOM");

            entity.Property(e => e.SelectedEmPdif)
                .IsRequired()
                .HasMaxLength(10)
                .HasDefaultValue("NOM");

            entity.Property(e => e.SelectedEmPrex)
                .IsRequired()
                .HasMaxLength(10)
                .HasDefaultValue("NOM");

            entity.Property(e => e.SelectedEmRuse)
                .IsRequired()
                .HasMaxLength(10)
                .HasDefaultValue("NOM");

            entity.Property(e => e.SelectedEmFcil)
                .IsRequired()
                .HasMaxLength(10)
                .HasDefaultValue("NOM");

            entity.Property(e => e.SelectedEmSced)
                .IsRequired()
                .HasMaxLength(10)
                .HasDefaultValue("NOM");

            // Calculated results with precision
            entity.Property(e => e.TotalUfp).HasPrecision(10, 2);
            entity.Property(e => e.Sloc).HasPrecision(10, 2);
            entity.Property(e => e.Ksloc).HasPrecision(10, 4);
            entity.Property(e => e.SumSf).HasPrecision(10, 4);
            entity.Property(e => e.ExponentE).HasPrecision(10, 4);
            entity.Property(e => e.Eaf).HasPrecision(10, 4);
            entity.Property(e => e.EffortPm).HasPrecision(10, 2);
            entity.Property(e => e.TdevMonths).HasPrecision(10, 2);
            entity.Property(e => e.AvgTeamSize).HasPrecision(10, 2);

            // Actual results with precision
            entity.Property(e => e.ActualEffortPm).HasPrecision(10, 2);
            entity.Property(e => e.ActualTdevMonths).HasPrecision(10, 2);
            entity.Property(e => e.ActualSloc).HasPrecision(10, 2);

            // Foreign key relationships
            entity.HasOne(e => e.Project)
                .WithMany(p => p.Estimations)
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.ParameterSet)
                .WithMany()
                .HasForeignKey(e => e.ParamSetId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Language)
                .WithMany()
                .HasForeignKey(e => e.LanguageId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // EstimationFunction entity configuration
        modelBuilder.Entity<EstimationFunction>(entity =>
        {
            entity.HasKey(e => e.FunctionId);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.Type)
                .IsRequired()
                .HasMaxLength(10);

            entity.Property(e => e.Complexity)
                .HasMaxLength(20);

            entity.Property(e => e.CalculatedPoints)
                .HasPrecision(10, 2);

            // Foreign key relationship
            entity.HasOne(e => e.Estimation)
                .WithMany(est => est.EstimationFunctions)
                .HasForeignKey(e => e.EstimationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // KlocEstimation entity configuration
        modelBuilder.Entity<KlocEstimation>(entity =>
        {
            entity.HasKey(e => e.KlocEstimationId);

            entity.Property(e => e.EstimationName)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.LinesOfCode)
                .IsRequired();

            entity.Property(e => e.EstimatedEffort)
                .HasPrecision(10, 2);

            entity.Property(e => e.EstimatedCost)
                .HasPrecision(10, 2);

            entity.Property(e => e.EstimatedTime)
                .HasPrecision(10, 2);

            entity.Property(e => e.Notes)
                .HasColumnType("TEXT");

            entity.Property(e => e.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

            entity.Property(e => e.UpdatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6)");
        });

        // FunctionPointEstimation entity configuration
        modelBuilder.Entity<FunctionPointEstimation>(entity =>
        {
            entity.HasKey(e => e.FpEstimationId);

            entity.Property(e => e.EstimationName)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.ComplexityLevel)
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.Property(e => e.UnadjustedFp)
                .HasPrecision(10, 2);

            entity.Property(e => e.ValueAdjustmentFactor)
                .HasPrecision(10, 4);

            entity.Property(e => e.AdjustedFp)
                .HasPrecision(10, 2);

            entity.Property(e => e.EstimatedEffort)
                .HasPrecision(10, 2);

            entity.Property(e => e.EstimatedCost)
                .HasPrecision(10, 2);

            entity.Property(e => e.EstimatedTime)
                .HasPrecision(10, 2);

            entity.Property(e => e.Notes)
                .HasColumnType("TEXT");

            entity.Property(e => e.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

            entity.Property(e => e.UpdatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6)");
        });

        // FunctionPointCharacteristic entity configuration
        modelBuilder.Entity<FunctionPointCharacteristic>(entity =>
        {
            entity.HasKey(e => e.FpCharId);

            entity.Property(e => e.CharacteristicName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.InfluenceLevel)
                .HasMaxLength(20);

            entity.Property(e => e.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

            entity.Property(e => e.UpdatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6)");

            // Foreign key relationship
            entity.HasOne(e => e.FunctionPointEstimation)
                .WithMany(fp => fp.Characteristics)
                .HasForeignKey(e => e.FpEstimationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // UseCasePointEstimation entity configuration
        modelBuilder.Entity<UseCasePointEstimation>(entity =>
        {
            entity.HasKey(e => e.UcpEstimationId);

            entity.Property(e => e.EstimationName)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.UnadjustedUcp)
                .HasPrecision(10, 2);

            entity.Property(e => e.TechnicalComplexityFactor)
                .HasPrecision(10, 4);

            entity.Property(e => e.EnvironmentFactor)
                .HasPrecision(10, 4);

            entity.Property(e => e.AdjustedUcp)
                .HasPrecision(10, 2);

            entity.Property(e => e.EstimatedEffort)
                .HasPrecision(10, 2);

            entity.Property(e => e.EstimatedEffortPm)
                .HasPrecision(10, 2);

            entity.Property(e => e.EstimatedCost)
                .HasPrecision(10, 2);

            entity.Property(e => e.EstimatedTime)
                .HasPrecision(10, 2);

            entity.Property(e => e.Notes)
                .HasColumnType("TEXT");

            entity.Property(e => e.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

            entity.Property(e => e.UpdatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6)");
        });

        // UseCaseTechnicalFactor entity configuration
        modelBuilder.Entity<UseCaseTechnicalFactor>(entity =>
        {
            entity.HasKey(e => e.UcpTechFactorId);

            entity.Property(e => e.FactorName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

            entity.Property(e => e.UpdatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6)");

            // Foreign key relationship
            entity.HasOne(e => e.UseCasePointEstimation)
                .WithMany(ucp => ucp.TechnicalFactors)
                .HasForeignKey(e => e.UcpEstimationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // UseCaseEnvironmentFactor entity configuration
        modelBuilder.Entity<UseCaseEnvironmentFactor>(entity =>
        {
            entity.HasKey(e => e.UcpEnvFactorId);

            entity.Property(e => e.FactorName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

            entity.Property(e => e.UpdatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6)");

            // Foreign key relationship
            entity.HasOne(e => e.UseCasePointEstimation)
                .WithMany(ucp => ucp.EnvironmentFactors)
                .HasForeignKey(e => e.UcpEstimationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // COCOMO 2 Stage 1 entity configurations
        // ParameterSetCocomo2Stage1 configuration
        modelBuilder.Entity<Models.Entities.CocomoTwoStageOne.ParameterSetCocomo2Stage1>(entity =>
        {
            entity.HasKey(e => e.ParamSetId);

                entity.Property(e => e.SetName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.IsDefault)
                    .IsRequired()
                    .HasDefaultValue(false);

                // Constants
                entity.Property(e => e.ConstA)
                    .HasPrecision(10, 4)
                    .HasDefaultValue(2.45m);

                entity.Property(e => e.ConstB)
                    .HasPrecision(10, 4)
                    .HasDefaultValue(1.10m);

                // AEXP multipliers
                entity.Property(e => e.AexpVeryLow).HasPrecision(10, 4);
                entity.Property(e => e.AexpLow).HasPrecision(10, 4);
                entity.Property(e => e.AexpNominal).HasPrecision(10, 4);
                entity.Property(e => e.AexpHigh).HasPrecision(10, 4);

                // PEXP multipliers
                entity.Property(e => e.PexpVeryLow).HasPrecision(10, 4);
                entity.Property(e => e.PexpLow).HasPrecision(10, 4);
                entity.Property(e => e.PexpNominal).HasPrecision(10, 4);
                entity.Property(e => e.PexpHigh).HasPrecision(10, 4);

                // PREC multipliers
                entity.Property(e => e.PrecVeryLow).HasPrecision(10, 4);
                entity.Property(e => e.PrecLow).HasPrecision(10, 4);
                entity.Property(e => e.PrecNominal).HasPrecision(10, 4);
                entity.Property(e => e.PrecHigh).HasPrecision(10, 4);

                // RELY multipliers
                entity.Property(e => e.RelyLow).HasPrecision(10, 4);
                entity.Property(e => e.RelyNominal).HasPrecision(10, 4);
                entity.Property(e => e.RelyHigh).HasPrecision(10, 4);

                // TMSP multipliers
                entity.Property(e => e.TmspLow).HasPrecision(10, 4);
                entity.Property(e => e.TmspNominal).HasPrecision(10, 4);
                entity.Property(e => e.TmspHigh).HasPrecision(10, 4);

                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

                // Foreign key
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // EstimationCocomo2Stage1 configuration
            modelBuilder.Entity<Models.Entities.CocomoTwoStageOne.EstimationCocomo2Stage1>(entity =>
            {
                entity.HasKey(e => e.EstimationId);

                entity.Property(e => e.EstimationName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.SelectedAexp)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValue("nominal");

                entity.Property(e => e.SelectedPexp)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValue("nominal");

                entity.Property(e => e.SelectedPrec)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValue("nominal");

                entity.Property(e => e.SelectedRely)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValue("nominal");

                entity.Property(e => e.SelectedTmsp)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValue("nominal");

                // Calculated results
                entity.Property(e => e.TotalFp).HasPrecision(10, 2);
                entity.Property(e => e.Sloc).HasPrecision(10, 2);
                entity.Property(e => e.Ksloc).HasPrecision(10, 4);
                entity.Property(e => e.AexpMultiplier).HasPrecision(10, 4);
                entity.Property(e => e.PexpMultiplier).HasPrecision(10, 4);
                entity.Property(e => e.PrecMultiplier).HasPrecision(10, 4);
                entity.Property(e => e.RelyMultiplier).HasPrecision(10, 4);
                entity.Property(e => e.TmspMultiplier).HasPrecision(10, 4);
                entity.Property(e => e.Eaf).HasPrecision(10, 4);
                entity.Property(e => e.EffortPm).HasPrecision(10, 2);
                entity.Property(e => e.EffortHours).HasPrecision(10, 2);
                entity.Property(e => e.ActualEffortPm).HasPrecision(10, 2);
                entity.Property(e => e.ActualEffortHours).HasPrecision(10, 2);
                entity.Property(e => e.ActualSloc).HasPrecision(10, 2);

                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

                entity.Property(e => e.UpdatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6)");

                // Foreign keys
                entity.HasOne(e => e.Project)
                    .WithMany()
                    .HasForeignKey(e => e.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.ParameterSet)
                    .WithMany()
                    .HasForeignKey(e => e.ParamSetId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Language)
                    .WithMany()
                    .HasForeignKey(e => e.LanguageId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // EstimationComponentCocomo2Stage1 configuration
            modelBuilder.Entity<Models.Entities.CocomoTwoStageOne.EstimationComponentCocomo2Stage1>(entity =>
            {
                entity.HasKey(e => e.ComponentId);

                entity.Property(e => e.ComponentName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.ComponentType)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValue("new");

                entity.Property(e => e.SizeFp)
                    .HasPrecision(10, 2);

                entity.Property(e => e.Description)
                    .HasColumnType("TEXT");

                entity.Property(e => e.Notes)
                    .HasColumnType("TEXT");

                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

                entity.Property(e => e.UpdatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6)");

                // Foreign key
                entity.HasOne(e => e.Estimation)
                    .WithMany(est => est.Components)
                    .HasForeignKey(e => e.EstimationId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
    }
}
