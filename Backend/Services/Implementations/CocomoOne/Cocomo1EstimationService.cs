using Backend.Models.DTOs.CocomoOne;
using Backend.Models.Entities.CocomoOne;
using Backend.Repositories.Interfaces.CocomoOne;
using Backend.Services.Interfaces.CocomoOne;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Services.Implementations.CocomoOne;

/// <summary>
/// Servicio para cálculos y operaciones de COCOMO 1 (Estadio I Básico)
/// 
/// Fórmulas COCOMO I:
/// Effort (PM) = a * (KLOC)^b
/// TDEV (meses) = c * (PM)^d
/// 
/// Parámetros por modo:
/// ORGANIC:        a=2.4,  b=1.05, c=2.5, d=0.38
/// SEMI_DETACHED:  a=3.0,  b=1.12, c=2.5, d=0.35
/// EMBEDDED:       a=3.6,  b=1.20, c=2.5, d=0.32
/// </summary>
public class Cocomo1EstimationService : ICocomo1EstimationService
{
    private readonly ICocomo1EstimationRepository _repository;

    public Cocomo1EstimationService(ICocomo1EstimationRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Cocomo1EstimationDto>> GetByProjectIdAsync(int projectId)
    {
        var estimations = await _repository.GetByProjectIdAsync(projectId);
        return estimations.Select(MapToDto);
    }

    public async Task<Cocomo1EstimationDto?> GetByIdAsync(int cocomo1EstimationId)
    {
        var estimation = await _repository.GetByIdAsync(cocomo1EstimationId);
        return estimation == null ? null : MapToDto(estimation);
    }

    public async Task<Cocomo1EstimationDto> CreateAsync(CreateCocomo1EstimationDto createDto)
    {
        var estimation = new Cocomo1Estimation
        {
            ProjectId = createDto.ProjectId,
            EstimationName = createDto.EstimationName,
            Kloc = createDto.Kloc,
            Mode = createDto.Mode,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Calcular esfuerzo y tiempo
        CalculateCocomo1(estimation);

        var created = await _repository.CreateAsync(estimation);
        return MapToDto(created);
    }

    public async Task<Cocomo1EstimationDto> UpdateAsync(int cocomo1EstimationId, CreateCocomo1EstimationDto updateDto)
    {
        var estimation = await _repository.GetByIdAsync(cocomo1EstimationId);
        if (estimation == null)
            throw new KeyNotFoundException($"COCOMO 1 Estimation with ID {cocomo1EstimationId} not found");

        estimation.EstimationName = updateDto.EstimationName;
        estimation.Kloc = updateDto.Kloc;
        estimation.Mode = updateDto.Mode;

        // Recalcular
        CalculateCocomo1(estimation);

        var updated = await _repository.UpdateAsync(estimation);
        return MapToDto(updated);
    }

    public async Task<bool> DeleteAsync(int cocomo1EstimationId)
    {
        return await _repository.DeleteAsync(cocomo1EstimationId);
    }

    /// <summary>
    /// Calcula esfuerzo y tiempo de desarrollo usando COCOMO 1
    /// </summary>
    private void CalculateCocomo1(Cocomo1Estimation estimation)
    {
        decimal a, b, c, d;

        switch (estimation.Mode.ToUpper())
        {
            case "ORGANIC":
                a = 2.4m;
                b = 1.05m;
                c = 2.5m;
                d = 0.38m;
                break;
            case "SEMI_DETACHED":
                a = 3.0m;
                b = 1.12m;
                c = 2.5m;
                d = 0.35m;
                break;
            case "EMBEDDED":
                a = 3.6m;
                b = 1.20m;
                c = 2.5m;
                d = 0.32m;
                break;
            default:
                a = 2.4m;
                b = 1.05m;
                c = 2.5m;
                d = 0.38m;
                break;
        }

        // Effort = a * (KLOC)^b
        estimation.EffortPm = a * (decimal)Math.Pow((double)estimation.Kloc, (double)b);

        // TDEV = c * (PM)^d
        var effort = estimation.EffortPm ?? 0m;
        estimation.TdevMonths = c * (decimal)Math.Pow((double)effort, (double)d);
    }

    private static Cocomo1EstimationDto MapToDto(Cocomo1Estimation e) => new Cocomo1EstimationDto
    {
        Cocomo1EstimationId = e.Cocomo1EstimationId,
        ProjectId = e.ProjectId,
        EstimationName = e.EstimationName,
        Kloc = e.Kloc,
        Mode = e.Mode,
        EffortPm = e.EffortPm,
        TdevMonths = e.TdevMonths,
        CreatedAt = e.CreatedAt,
        UpdatedAt = e.UpdatedAt
    };
}
