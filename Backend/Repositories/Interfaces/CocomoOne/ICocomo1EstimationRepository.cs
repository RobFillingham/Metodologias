using Backend.Models.Entities.CocomoOne;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Repositories.Interfaces.CocomoOne;

public interface ICocomo1EstimationRepository
{
    Task<IEnumerable<Cocomo1Estimation>> GetByProjectIdAsync(int projectId);
    Task<Cocomo1Estimation?> GetByIdAsync(int cocomo1EstimationId);
    Task<Cocomo1Estimation> CreateAsync(Cocomo1Estimation estimation);
    Task<Cocomo1Estimation> UpdateAsync(Cocomo1Estimation estimation);
    Task<bool> DeleteAsync(int cocomo1EstimationId);
}
