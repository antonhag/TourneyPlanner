using System.Collections;
using TourneyPlanner.Application.DTOs;

namespace Application.Tests.TestData;

public static class CreateTournamentTestData
{
    public static IEnumerable<object[]> Valid() // Skapar giltigt testdata
    {
        var tomorrow = DateTime.Today.AddDays(1);
        
        yield return new object[] { new CreateTournamentDto { Name = "Padel 2026", StartDate = tomorrow } };
        yield return new object[] { new CreateTournamentDto { Name = "Padelcupen 2026", StartDate = tomorrow } };
        yield return new object[] { new CreateTournamentDto { Name = "Padel 2026", StartDate = DateTime.Today } };
    }

    public static IEnumerable<object[]> Invalid() // Skapar ogiltigt testadata
    {
        var tomorrow = DateTime.Today.AddDays(1);
        
        yield return new object[] { new CreateTournamentDto { Name = "", StartDate = tomorrow } };
        yield return new object[] { new CreateTournamentDto { Name = "Sommarcupen 2026", StartDate = tomorrow } };
        yield return new object[] { new CreateTournamentDto { Name = "Padel 2026", StartDate = DateTime.Today.AddDays(-1) } };
    }
}