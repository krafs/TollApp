namespace TollFeeCalculator;

public class Default2025RuleSet : TollRuleSet
{
    public Default2025RuleSet()
    {
        ValidFrom = new DateOnly(2025, 1, 1);
        MaxDailyTollSek = 60;
        TollFreeWindowDuration = TimeSpan.FromHours(1);
        PublicHolidays =
        [
            new DateOnly(2025, 1, 1), // Nyårsdagen
            new DateOnly(2025, 1, 6), // Trettondedag jul
            new DateOnly(2025, 4, 18), // Långfredagen
            new DateOnly(2025, 4, 19), // Påskafton
            new DateOnly(2025, 4, 20), // Påskdagen
            new DateOnly(2025, 4, 21), // Annandag påsk
            new DateOnly(2025, 4, 30), // Valborgsmässoafton
            new DateOnly(2025, 5, 1), // Första maj
            new DateOnly(2025, 5, 29), // Kristi himmelfärdsdag
            new DateOnly(2025, 6, 6), // Sveriges nationaldag
            new DateOnly(2025, 6, 7), // Pingstafton
            new DateOnly(2025, 6, 8), // Pingstdagen
            new DateOnly(2025, 6, 20), // Midsommarafton
            new DateOnly(2025, 6, 21), // Midsommardagen
            new DateOnly(2025, 11, 1), // Alla helgons dag
            new DateOnly(2025, 12, 24), // Julafton
            new DateOnly(2025, 12, 25), // Juldagen
            new DateOnly(2025, 12, 26), // Annandag jul
            new DateOnly(2025, 12, 31) // Nyårsafton
        ];
        TollFreeVehicleTypes =
        [
            VehicleType.Motorbike,
            VehicleType.Tractor,
            VehicleType.Emergency,
            VehicleType.Diplomat,
            VehicleType.Foreign,
            VehicleType.Military
        ];
        TollRules =
        [
            new TollRule(new TimeOnly(6, 0), new TimeOnly(6, 29), 8),
            new TollRule(new TimeOnly(6, 30), new TimeOnly(6, 59), 13),
            new TollRule(new TimeOnly(7, 0), new TimeOnly(7, 59), 18),
            new TollRule(new TimeOnly(8, 0), new TimeOnly(8, 29), 13),
            new TollRule(new TimeOnly(8, 30), new TimeOnly(14, 59), 8),
            new TollRule(new TimeOnly(15, 0), new TimeOnly(15, 29), 13),
            new TollRule(new TimeOnly(15, 30), new TimeOnly(16, 59), 18),
            new TollRule(new TimeOnly(17, 0), new TimeOnly(17, 59), 13),
            new TollRule(new TimeOnly(18, 0), new TimeOnly(18, 29), 8),
            new TollRule(new TimeOnly(18, 30), new TimeOnly(5, 59), 0)
        ];
    }
}