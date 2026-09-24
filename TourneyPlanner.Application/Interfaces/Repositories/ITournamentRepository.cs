using TourneyPlanner.Domain.Entities;

namespace TourneyPlanner.Application.Interfaces.Repositories;

public interface ITournamentRepository
{
    Task AddAsync(Tournament tournament);
    Task<List<Tournament>> GetAllAsync();
    
    Task UpdateAsync(Tournament tournament);
    Task DeleteAsync(Tournament tournament);
    
   
}