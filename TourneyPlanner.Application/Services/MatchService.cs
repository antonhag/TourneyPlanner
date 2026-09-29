using TourneyPlanner.Application.Interfaces.Repositories;
using TourneyPlanner.Application.Interfaces.Services;

namespace TourneyPlanner.Application.Services;

public class MatchService : IMatchService
{
    private readonly IMatchRepository _matchRepository;
    
    public  MatchService(IMatchRepository matchRepository)
    {
        _matchRepository = matchRepository;
    }
}