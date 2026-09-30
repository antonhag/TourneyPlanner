using System.Collections;
using TourneyPlanner.Application.DTOs;

namespace Application.Tests.TestData;

public static class CreateTournamentTestData
{
    public static IEnumerable<object[]> Valid() // Skapar giltigt testdata
    {
        var tomorrow = DateTime.Today.AddDays(1);
        var dayAfter = DateTime.Today.AddDays(2);
        
        yield return new object[] { new CreateTournamentDto { Name = "Padel 2026", StartDate = tomorrow, EndDate = dayAfter, Size = 4 } };
        yield return new object[] { new CreateTournamentDto { Name = "Padelcupen 2026", StartDate = tomorrow, EndDate = dayAfter, Size = 4 } };
        yield return new object[] { new CreateTournamentDto { Name = "Padel 2026", StartDate = DateTime.Today, EndDate = dayAfter, Size = 4 } };
    }

    public static IEnumerable<object[]> Invalid() // Skapar ogiltigt testadata
    {
        var tomorrow = DateTime.Today.AddDays(1);
        var  dayAfter = DateTime.Today.AddDays(2);
        
        yield return new object[] { new CreateTournamentDto { Name = "", StartDate = tomorrow, EndDate = dayAfter, Size = 4 } };
        yield return new object[] { new CreateTournamentDto { Name = "Sommarcupen 2026", StartDate = tomorrow, EndDate = dayAfter, Size = 4 } };
        yield return new object[] { new CreateTournamentDto { Name = "Padel 2026", StartDate = DateTime.Today.AddDays(-1), EndDate = dayAfter, Size = 4 } };
        
        yield return new object[] { new CreateTournamentDto { Name = "Padel 2026", StartDate = DateTime.Today.AddDays(3), EndDate = dayAfter, Size = 4 } }; // slutdatum före startdatum
        yield return new object[] { new CreateTournamentDto { Name = "Padel 2026", StartDate = tomorrow, EndDate = dayAfter, Size = 1 } };
        yield return new object[] { new CreateTournamentDto { Name = "Padel 2026", StartDate = tomorrow, EndDate = dayAfter, Size = 0 } };
    }
}