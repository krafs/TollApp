namespace TollFeeCalculator;

public class TollCalculator
{
    private readonly TollCalculatorConfig _config;

    public TollCalculator(TollCalculatorConfig config)
    {
        _config = config;
    }

    /// <summary>
    /// Calculate the total toll fee for one day
    /// </summary>
    /// <param name="vehicle">the vehicle</param>
    /// <param name="passageDate">date of all passes on one day</param>
    /// <param name="passageTimes">times of all passes</param>
    /// <returns>the total toll fee for that day, in SEK</returns>
    public decimal CalculateTotalDailyToll(Vehicle vehicle, DateOnly passageDate, IEnumerable<TimeOnly> passageTimes)
    {
        ArgumentNullException.ThrowIfNull(vehicle);
        ArgumentNullException.ThrowIfNull(passageTimes);
        TollRuleSet ruleSet = _config.RuleSets.Last(x => x.ValidFrom <= passageDate);

        if (IsTollFreeVehicle(vehicle, ruleSet))
        {
            return 0M;
        }

        if (IsTollFreeDate(passageDate, ruleSet))
        {
            return 0M;
        }

        IEnumerable<TimeOnly> orderedPassageTimes = passageTimes
            .Distinct()
            .Order();

        // Toll calculations apply a sliding window of time within which only the top toll value is considered.
        // E.g. if a car passes an 13 SEK gate and an 8 SEK gate within the sliding window, only the top value counts (13 SEK) 
        decimal totalTollSek = 0M;
        TimeOnly? currentWindowStartTime = null;
        TimeOnly currentWindowEndTime = default;
        decimal currentWindowTopTollSek = 0M;

        foreach (TimeOnly passageTime in orderedPassageTimes)
        {
            if (currentWindowStartTime is null || passageTime > currentWindowEndTime)
            {
                // Start a new toll window
                totalTollSek += currentWindowTopTollSek;
                currentWindowStartTime = passageTime;
                currentWindowEndTime = currentWindowStartTime.Value.Add(ruleSet.TollFreeWindowDuration);
                currentWindowTopTollSek = 0M;
            }

            decimal tollSek = CalculateTollForPassage(vehicle, passageDate, passageTime, ruleSet);
            currentWindowTopTollSek = Math.Max(tollSek, currentWindowTopTollSek);
        }

        // Last window's toll is added here because that's not done in the loop.
        totalTollSek += currentWindowTopTollSek;

        decimal clampedTotalTollSek = Math.Min(totalTollSek, ruleSet.MaxDailyTollSek);

        return clampedTotalTollSek;
    }

    private bool IsTollFreeVehicle(Vehicle vehicle, TollRuleSet ruleSet)
    {
        return ruleSet.TollFreeVehicleTypes.Contains(vehicle.VehicleType);
    }

    /// <summary>
    /// Calculate the toll fee for one single pass
    /// </summary>
    /// <param name="vehicle">the vehicle</param>
    /// <param name="passageDateTime">date and time for the pass</param>
    /// <returns>the toll fee for that time, in SEK</returns>
    public decimal CalculateTollForPassage(Vehicle vehicle, DateTime passageDateTime)
    {
        ArgumentNullException.ThrowIfNull(vehicle);

        DateOnly passageDate = DateOnly.FromDateTime(passageDateTime);
        TollRuleSet ruleSet = _config.RuleSets.Last(x => x.ValidFrom <= passageDate);
        if (IsTollFreeVehicle(vehicle, ruleSet))
        {
            return 0M;
        }

        if (IsTollFreeDate(passageDate, ruleSet))
        {
            return 0M;
        }

        TimeOnly passageTime = TimeOnly.FromDateTime(passageDateTime);
        return CalculateTollForPassage(vehicle, passageDate, passageTime, ruleSet);
    }

    private decimal CalculateTollForPassage(Vehicle vehicle, DateOnly passageDate, TimeOnly passageTime, TollRuleSet ruleSet)
    {
        return ruleSet.TollRules
            .First(x => passageTime.IsBetween(x.ValidFrom, x.ValidTo))
            .TollSek;
    }

    private static bool IsTollFreeDate(DateOnly date, TollRuleSet ruleSet)
    {
        if (date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
        {
            return true;
        }

        bool isJuly = date.Month == 7;
        if (isJuly)
        {
            return true;
        }

        bool isDatePublicHoliday = ruleSet.PublicHolidays.Contains(date);
        if (isDatePublicHoliday)
        {
            return true;
        }

        DateOnly dayAfterPassageDate = date.AddDays(1);
        bool isDateDayBeforePublicHoliday = ruleSet.PublicHolidays.Contains(dayAfterPassageDate);
        if (isDateDayBeforePublicHoliday)
        {
            return true;
        }

        return false;
    }
}