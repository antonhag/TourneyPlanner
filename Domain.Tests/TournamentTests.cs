using System.ComponentModel.DataAnnotations;
using TourneyPlanner.Domain.Entities;

namespace Domain.Tests;

public class TournamentTests
{
    [Fact]
    public void NewTournament_HasStatusDraft_ShouldReturnSuccess()
    {
        // Arrange
        var sut = new Tournament();

        // Assert
        Assert.Equal(Tournament.Status.Draft, sut.TournamentStatus);
    }

    [Theory]
    [InlineData("Padelturnering 2026", false)]
    [InlineData("Padel 2026", true)]
    [InlineData("", false)]
    [InlineData("Padelturnerings", true)]
    public void NewTournament_NameValidation(string name, bool expected)
    {   
        // Arrange
        var sut = new Tournament { Name = name };

        // Act
        var actual = TestHelpers.IsValid(sut);

        // Assert
        Assert.Equal(expected, actual);
        
    }
}