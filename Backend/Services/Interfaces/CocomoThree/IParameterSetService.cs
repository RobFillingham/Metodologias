using Backend.Models.DTOs.CocomoThree;
using Backend.Models.Entities.CocomoThree;

namespace Backend.Services.Interfaces.CocomoThree;

/// <summary>
/// Interface for ParameterSet service operations
/// </summary>
public interface IParameterSetService
{
    /// <summary>
    /// Get all parameter sets for a specific user
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <returns>List of parameter sets</returns>
    Task<IEnumerable<ParameterSetDto>> GetUserParameterSetsAsync(int userId);

    /// <summary>
    /// Get all default (system) parameter sets
    /// </summary>
    /// <returns>List of default parameter sets</returns>
    Task<IEnumerable<ParameterSetDto>> GetDefaultParameterSetsAsync();

    /// <summary>
    /// Get a parameter set by ID
    /// </summary>
    /// <param name="paramSetId">The parameter set ID</param>
    /// <param name="userId">The user ID (for authorization)</param>
    /// <returns>The parameter set or null if not found</returns>
    Task<ParameterSetDto?> GetParameterSetByIdAsync(int paramSetId, int userId);

    /// <summary>
    /// Create a new parameter set
    /// </summary>
    /// <param name="createDto">The create DTO</param>
    /// <param name="userId">The user ID</param>
    /// <returns>The created parameter set</returns>
    Task<ParameterSetDto> CreateParameterSetAsync(CreateParameterSetDto createDto, int userId);

    /// <summary>
    /// Update an existing parameter set
    /// </summary>
    /// <param name="updateDto">The update DTO</param>
    /// <param name="userId">The user ID (for authorization)</param>
    /// <returns>The updated parameter set</returns>
    Task<ParameterSetDto> UpdateParameterSetAsync(UpdateParameterSetDto updateDto, int userId);

    /// <summary>
    /// Delete a parameter set
    /// </summary>
    /// <param name="paramSetId">The parameter set ID</param>
    /// <param name="userId">The user ID (for authorization)</param>
    /// <returns>True if deleted, false if not found or not authorized</returns>
    Task<bool> DeleteParameterSetAsync(int paramSetId, int userId);

    /// <summary>
    /// Check if a parameter set exists and belongs to the user
    /// </summary>
    /// <param name="paramSetId">The parameter set ID</param>
    /// <param name="userId">The user ID</param>
    /// <returns>True if exists and belongs to user</returns>
    Task<bool> ParameterSetExistsAndBelongsToUserAsync(int paramSetId, int userId);
}