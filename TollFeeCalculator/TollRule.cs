namespace TollFeeCalculator;

public record TollRule(TimeOnly ValidFrom, TimeOnly ValidTo, decimal TollSek);