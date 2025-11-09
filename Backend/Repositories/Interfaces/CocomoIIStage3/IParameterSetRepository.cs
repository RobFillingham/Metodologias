using Backend.Models.Entities.CocomoIIStage3;

namespace Backend.Repositories.Interfaces.CocomoIIStage3;

/// <summary>
/// Interface for ParameterSet repository operations
/// </summary>
public interface IParameterSetRepository
{
    /// <summary>
    /// Get all parameter sets for a specific user
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <returns>List of parameter sets</returns>
    Task<IEnumerable<ParameterSet>> GetByUserIdAsync(int userId);

    /// <summary>
    /// Get all default (system) parameter sets
    /// </summary>
    /// <returns>List of default parameter sets</returns>
    Task<IEnumerable<ParameterSet>> GetDefaultParameterSetsAsync();

    /// <summary>
    /// Get a parameter set by ID
    /// </summary>
    /// <param name="paramSetId">The parameter set ID</param>
    /// <returns>The parameter set or null if not found</returns>
    Task<ParameterSet?> GetByIdAsync(int paramSetId);

    /// <summary>
    /// Create a new parameter set
    /// </summary>
    /// <param name="parameterSet">The parameter set to create</param>
    /// <returns>The created parameter set</returns>
    Task<ParameterSet> CreateAsync(ParameterSet parameterSet);

    /// <summary>
    /// Update an existing parameter set
    /// </summary>
    /// <param name="parameterSet">The parameter set to update</param>
    /// <returns>The updated parameter set</returns>
    Task<ParameterSet> UpdateAsync(ParameterSet parameterSet);

    /// <summary>
    /// Delete a parameter set
    /// </summary>
    /// <param name="paramSetId">The parameter set ID</param>
    /// <returns>True if deleted, false if not found</returns>
    Task<bool> DeleteAsync(int paramSetId);

    /// <summary>
    /// Check if a parameter set exists
    /// </summary>
    /// <param name="paramSetId">The parameter set ID</param>
    /// <returns>True if exists</returns>
    Task<bool> ExistsAsync(int paramSetId);

    /// <summary>
    /// Check if a parameter set exists and belongs to a specific user
    /// </summary>
    /// <param name="paramSetId">The parameter set ID</param>
    /// <param name="userId">The user ID</param>
    /// <returns>True if exists and belongs to user</returns>
    Task<bool> ExistsAndBelongsToUserAsync(int paramSetId, int userId);
}