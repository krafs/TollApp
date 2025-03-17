using TollFeeCalculator;

public class TollCalculator
{
    /// <summary>
    /// Calculate the total toll fee for one day
    /// </summary>
    /// <param name="vehicle">the vehicle</param>
    /// <param name="passageDate">date of all passes on one day</param>
    /// <param name="passageTimes">times of all passes</param>
    /// <returns>the total toll fee for that day, in SEK</returns>
    public int CalculateTotalDailyToll(Vehicle vehicle, DateOnly passageDate, IEnumerable<TimeOnly> passageTimes)
    {
        ArgumentNullException.ThrowIfNull(vehicle);
        ArgumentNullException.ThrowIfNull(passageTimes);
        
        TimeSpan tollWindowDuration = TimeSpan.FromHours(1);
        int maxDailyTollSek = 60;
        
        if (IsTollFreeVehicle(vehicle))
        {
            return 0;
        }
        
        if (IsTollFreeDate(passageDate))
        {
            return 0;
        }
        
        IEnumerable<TimeOnly> orderedPassageTimes = passageTimes
            .Distinct()
            .Order();

        // Toll calculations apply a sliding window of time within which only the top toll value is considered.
        // E.g. if a car passes an 13 SEK gate and an 8 SEK gate within the sliding window, only the top value counts (13 SEK) 
        int totalTollSek = 0;
        TimeOnly? currentWindowStartTime = null;
        TimeOnly currentWindowEndTime = default;
        int currentWindowTopTollSek = 0;

        foreach (TimeOnly passageTime in orderedPassageTimes)
        {
            if (currentWindowStartTime is null || passageTime > currentWindowEndTime)
            {
                // Start a new toll window
                totalTollSek += currentWindowTopTollSek;
                currentWindowStartTime = passageTime;
                currentWindowEndTime = currentWindowStartTime.Value.Add(tollWindowDuration);
                currentWindowTopTollSek = 0;
            }

            int tollSek = CalculateTollForPassage(vehicle, passageDate, passageTime);
            currentWindowTopTollSek = Math.Max(tollSek, currentWindowTopTollSek);
        }

        // Last window's toll is added here because that's not done in the loop.
        totalTollSek += currentWindowTopTollSek;
        
        int clampedTotalTollSek = Math.Min(totalTollSek, maxDailyTollSek);
        
        return clampedTotalTollSek;
    }

    private bool IsTollFreeVehicle(Vehicle vehicle)
    {
        VehicleType vehicleType = vehicle.VehicleType;
        return vehicleType.Equals(VehicleType.Motorbike) ||
               vehicleType.Equals(VehicleType.Tractor) ||
               vehicleType.Equals(VehicleType.Emergency) ||
               vehicleType.Equals(VehicleType.Diplomat) ||
               vehicleType.Equals(VehicleType.Foreign) ||
               vehicleType.Equals(VehicleType.Military);
    }
    
    /// <summary>
    /// Calculate the toll fee for one single pass
    /// </summary>
    /// <param name="vehicle">the vehicle</param>
    /// <param name="passageDateTime">date and time for the pass</param>
    /// <returns>the toll fee for that time, in SEK</returns>
    public int CalculateTollForPassage(Vehicle vehicle, DateTime passageDateTime)
    {
        ArgumentNullException.ThrowIfNull(vehicle);
        
        if (IsTollFreeVehicle(vehicle))
        {
            return 0;
        }
        
        DateOnly passageDate = DateOnly.FromDateTime(passageDateTime);
        if (IsTollFreeDate(passageDate))
        {
            return 0;
        }
        
        int hour = passageDateTime.Hour;
        int minute = passageDateTime.Minute;

        if (hour == 6 && minute >= 0 && minute <= 29) return 8;
        else if (hour == 6 && minute >= 30 && minute <= 59) return 13;
        else if (hour == 7 && minute >= 0 && minute <= 59) return 18;
        else if (hour == 8 && minute >= 0 && minute <= 29) return 13;
        else if (hour >= 8 && hour <= 14 && minute >= 30 && minute <= 59) return 8;
        else if (hour == 15 && minute >= 0 && minute <= 29) return 13;
        else if (hour == 15 && minute >= 0 || hour == 16 && minute <= 59) return 18;
        else if (hour == 17 && minute >= 0 && minute <= 59) return 13;
        else if (hour == 18 && minute >= 0 && minute <= 29) return 8;
        else return 0;
    }
    private int CalculateTollForPassage(Vehicle vehicle, DateOnly passageDate, TimeOnly passageTime)
    {
        int hour = passageTime.Hour;
        int minute = passageTime.Minute;

        if (hour == 6 && minute >= 0 && minute <= 29) return 8;
        else if (hour == 6 && minute >= 30 && minute <= 59) return 13;
        else if (hour == 7 && minute >= 0 && minute <= 59) return 18;
        else if (hour == 8 && minute >= 0 && minute <= 29) return 13;
        else if (hour >= 8 && hour <= 14 && minute >= 30 && minute <= 59) return 8;
        else if (hour == 15 && minute >= 0 && minute <= 29) return 13;
        else if (hour == 15 && minute >= 0 || hour == 16 && minute <= 59) return 18;
        else if (hour == 17 && minute >= 0 && minute <= 59) return 13;
        else if (hour == 18 && minute >= 0 && minute <= 29) return 8;
        else return 0;
    }
    private bool IsTollFreeDate(DateOnly date)
    {
        int year = date.Year;
        int month = date.Month;
        int day = date.Day;

        if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday) return true;

        if (year == 2013)
        {
            if (month == 1 && day == 1 ||
                month == 3 && (day == 28 || day == 29) ||
                month == 4 && (day == 1 || day == 30) ||
                month == 5 && (day == 1 || day == 8 || day == 9) ||
                month == 6 && (day == 5 || day == 6 || day == 21) ||
                month == 7 ||
                month == 11 && day == 1 ||
                month == 12 && (day == 24 || day == 25 || day == 26 || day == 31))
            {
                return true;
            }
        }
        return false;
    }
}