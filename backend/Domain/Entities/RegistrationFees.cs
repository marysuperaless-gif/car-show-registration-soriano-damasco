namespace Domain.Entities;

using Domain.Enums;

public static class RegistrationFees
{
    public const decimal Exhibitor = 1500m;
    public const decimal Competitor = 2500m;
    public const decimal VIP = 5000m;

    public static decimal For(RegistrationType type) => type switch
    {
        RegistrationType.Exhibitor => Exhibitor,
        RegistrationType.Competitor => Competitor,
        RegistrationType.VIP => VIP,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unknown registration type.")
    };
}
