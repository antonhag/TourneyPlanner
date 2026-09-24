using TourneyPlanner.Domain.Entities;

namespace Domain.Tests;

public class ParticipantTests
{
    [Theory]
    [InlineData("Anton", true)]
    [InlineData("Maximilian Andersson-Berg", true)]
    [InlineData("Maximilian Andersson-Bergh", false)]
    [InlineData("", false)]
    public void NewParticipant_NameValidation(string name, bool expected)
    {
        // Arrange
        var sut = new Participant { Name = name };

        // Act
        var actual = TestHelpers.IsValid(sut);

        // Assert
        Assert.Equal(expected, actual);
    }
}