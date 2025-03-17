namespace TollFeeCalculator;

public class TollRuleSet
{
    public DateOnly ValidFrom { get; init; }
    public decimal MaxDailyTollSek { get; init; }
    public TimeSpan TollFreeWindowDuration { get; init; }
    public HashSet<DateOnly> PublicHolidays { get; init; } = [];
    public HashSet<VehicleType> TollFreeVehicleTypes { get; init; } = [];
    public List<TollRule> TollRules { get; init; } = [];
}