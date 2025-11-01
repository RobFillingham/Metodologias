using Backend.Models.DTOs.CocomoOne;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Services.Interfaces.CocomoOne;

public interface ICocomo1EstimationService
{
    Task<IEnumerable<Cocomo1EstimationDto>> GetByProjectIdAsync(int projectId);
    Task<Cocomo1EstimationDto?> GetByIdAsync(int cocomo1EstimationId);
    Task<Cocomo1EstimationDto> CreateAsync(CreateCocomo1EstimationDto createDto);
    Task<Cocomo1EstimationDto> UpdateAsync(int cocomo1EstimationId, CreateCocomo1EstimationDto updateDto);
    Task<bool> DeleteAsync(int cocomo1EstimationId);
}
