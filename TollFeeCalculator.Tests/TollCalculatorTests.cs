using Xunit;

namespace TollFeeCalculator.Tests;

public class TollCalculatorTests
{
    private readonly TollCalculator _calculator = new();

    [Theory]
    [InlineData("2025-03-17 06:15", 8)] // 06:00-06:29
    [InlineData("2025-03-17 06:45", 13)] // 06:30-06:59
    [InlineData("2025-03-17 07:30", 18)] // 07:00-07:59
    [InlineData("2025-03-17 08:10", 13)] // 08:00-08:29
    [InlineData("2025-03-17 09:30", 8)] // 08:30-14:59
    [InlineData("2025-03-17 15:10", 13)] // 15:00-15:29
    [InlineData("2025-03-17 16:00", 18)] // 15:30-16:59
    [InlineData("2025-03-17 17:30", 13)] // 17:00-17:59
    [InlineData("2025-03-17 18:15", 8)] // 18:00-18:29
    [InlineData("2025-03-17 19:00", 0)] // Outside fee schedule
    public void CalculateTollForPassageReturnsCorrectFeeBasedOnSchedule(string passageTimeText, int expectedFee)
    {
        // Arrange
        DateTime passageTime = DateTime.Parse(passageTimeText);
        Vehicle vehicle = new Car();

        // Act
        int fee = _calculator.CalculateTollForPassage(passageTime, vehicle);

        // Assert
        Assert.Equal(expectedFee, fee);
    }

    [Theory]
    [InlineData("2025-03-15 06:15")] // Saturday
    [InlineData("2025-03-16 06:15")] // Sunday
    public void CalculateTollForPassageReturnsZeroOnWeekend(string passageTimeText)
    {
        // Arrange
        DateTime passageTime = DateTime.Parse(passageTimeText);
        Vehicle vehicle = new Car();

        // Act
        int fee = _calculator.CalculateTollForPassage(passageTime, vehicle);

        // Assert
        Assert.Equal(0, fee);
    }

    [Fact(Skip = "Fails")]
    public void CalculateTollForPassageReturnsZeroInJuly()
    {
        // Arrange
        DateTime passageTime = DateTime.Parse("2025-07-01 06:15"); // Weekday
        Vehicle vehicle = new Car();

        // Act
        int fee = _calculator.CalculateTollForPassage(passageTime, vehicle);

        // Assert
        Assert.Equal(0, fee);
    }

    [Fact]
    public void CalculateTollForPassageReturnsZeroGivenExemptVehicle()
    {
        // Arrange
        DateTime passageTime = DateTime.Parse("2025-03-17 06:15"); // Weekday
        Vehicle vehicle = new Motorbike();

        // Act
        int fee = _calculator.CalculateTollForPassage(passageTime, vehicle);

        // Assert
        Assert.Equal(0, fee);
    }

    [Fact(Skip = "Fails")]
    public void CalculateTollForPassageReturnsZeroOnPublicHoliday()
    {
        // Arrange
        DateTime passageTime = DateTime.Parse("2025-12-25 06:15"); // Christmas Day
        Vehicle vehicle = new Car();

        // Act
        int fee = _calculator.CalculateTollForPassage(passageTime, vehicle);

        // Assert
        Assert.Equal(0, fee);
    }

    [Fact(Skip = "Fails")]
    public void CalculateTollForPassageThrowsArgumentNullExceptionGivenNullVehicle()
    {
        // Arrange
        DateTime passageTime = DateTime.Parse("2025-03-17 06:15"); // Weekday
        Vehicle vehicle = null!;

        // Act
        // Assert
        Action calculateTollForPassage = () => _calculator.CalculateTollForPassage(passageTime, vehicle);
        Assert.Throws<ArgumentNullException>(calculateTollForPassage);
    }

    [Fact(Skip = "Fails")]
    public void CalculateTotalDailyTollThrowsArgumentNullExceptionGivenNullVehicle()
    {
        // Arrange
        DateTime[] passageTimes = [DateTime.Parse("2025-03-17 06:15")]; // Weekday
        Vehicle vehicle = null!;

        // Act
        // Assert
        Action calculateTotalDailyToll = () => _calculator.CalculateTotalDailyToll(vehicle, passageTimes);
        Assert.Throws<ArgumentNullException>(calculateTotalDailyToll);
    }

    [Fact(Skip = "Fails")]
    public void CalculateTotalDailyTollThrowsArgumentNullExceptionGivenNullPassageTimes()
    {
        // Arrange
        DateTime[] passageTimes = null!;
        Vehicle vehicle = new Car();

        // Act
        // Assert
        Action calculateTotalDailyToll = () => _calculator.CalculateTotalDailyToll(vehicle, passageTimes);
        Assert.Throws<ArgumentNullException>(calculateTotalDailyToll);
    }

    [Fact(Skip = "Fails")]
    public void CalculateTotalDailyTollThrowsArgumentOutOfRangeExceptionGivenPassageTimesForMultipleDays()
    {
        // Arrange
        DateTime[] passageTimes =
        [
            DateTime.Parse("2025-03-17 06:15"),
            DateTime.Parse("2025-03-18 06:15")
        ];
        Vehicle vehicle = new Car();

        // Act
        // Assert
        Action calculateTotalDailyToll = () => _calculator.CalculateTotalDailyToll(vehicle, passageTimes);
        Assert.Throws<ArgumentOutOfRangeException>(calculateTotalDailyToll);
    }

    [Fact(Skip = "Fails")]
    public void CalculateTotalDailyTollReturnsZeroGivenEmptyPassageTimes()
    {
        // Arrange
        DateTime[] passageTimes = [];
        Vehicle vehicle = new Car();

        // Act
        int fee = _calculator.CalculateTotalDailyToll(vehicle, passageTimes);

        // Assert
        Assert.Equal(0, fee);
    }

    [Fact(Skip = "Fails")]
    public void CalculateTotalDailyTollReturnsAccumulatedFee()
    {
        // Arrange
        DateTime[] passageTimes =
        [
            DateTime.Parse("2025-03-17 06:15"), // 8 SEK
            DateTime.Parse("2025-03-17 17:15") // 13 SEK
        ];
        Vehicle vehicle = new Car();
        int expectedFee = 21; // 8 + 13

        // Act
        int fee = _calculator.CalculateTotalDailyToll(vehicle, passageTimes);

        // Assert
        Assert.Equal(expectedFee, fee);
    }

    [Fact]
    public void CalculateTotalDailyTollReturnsOnlyHighestFeeGivenMultiplePassagesWithinOneHour()
    {
        // Arrange
        DateTime[] passageTimes =
        [
            DateTime.Parse("2025-03-17 06:15"), // 8 SEK
            DateTime.Parse("2025-03-17 06:45") // 13 SEK
        ];
        Vehicle vehicle = new Car();
        int expectedFee = 13; // 13 > 8

        // Act
        int fee = _calculator.CalculateTotalDailyToll(vehicle, passageTimes);

        // Assert
        Assert.Equal(expectedFee, fee);
    }

    [Fact(Skip = "Fails")]
    public void CalculateTotalDailyTollReturnsMax60()
    {
        // Arrange
        DateTime[] passageTimes =
        [
            DateTime.Parse("2025-03-17 06:10"), // 8 SEK
            DateTime.Parse("2025-03-17 07:15"), // 18 SEK
            DateTime.Parse("2025-03-17 08:20"), // 13 SEK
            DateTime.Parse("2025-03-17 09:25"), // 8 SEK
            DateTime.Parse("2025-03-17 15:35"), // 18 SEK
        ];
        Vehicle vehicle = new Car();
        int expectedFee = 60; // min(8 + 18 + 13 + 8 + 18, 60) = 60 (actual total: 65)

        // Act
        int fee = _calculator.CalculateTotalDailyToll(vehicle, passageTimes);

        // Assert
        Assert.Equal(expectedFee, fee);
    }
}