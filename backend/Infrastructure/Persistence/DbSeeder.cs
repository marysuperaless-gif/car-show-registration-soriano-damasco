namespace Infrastructure.Persistence;

using Domain.Entities;
using Domain.Enums;

public static class DbSeeder
{
    private static readonly (string Name, string Car, string Plate, RegistrationType Type, RegistrationStatus Status, decimal Paid)[] Rows =
    [
        ("Maria Santos",      "Honda Civic EK9",         "NAB 1201", RegistrationType.Exhibitor,  RegistrationStatus.Confirmed, 1500m),
        ("Pedro Reyes",       "Nissan Skyline R34",      "NBC 2302", RegistrationType.Competitor, RegistrationStatus.Confirmed, 1000m),
        ("Ana Lopez",         "Mazda RX-7 FD",           "ABD 3403", RegistrationType.VIP,        RegistrationStatus.Confirmed, 5000m),
        ("Jose Garcia",       "Subaru WRX STI",          "CDE 4504", RegistrationType.Competitor, RegistrationStatus.Pending,   0m),
        ("Carla Mendoza",     "Toyota AE86",             "DEF 5605", RegistrationType.Exhibitor,  RegistrationStatus.Pending,   500m),
        ("Miguel Torres",     "Mitsubishi Lancer Evo IX","EFG 6706", RegistrationType.VIP,        RegistrationStatus.Pending,   2500m),
        ("Liza Ramos",        "Ford Mustang GT",         "FGH 7807", RegistrationType.Competitor, RegistrationStatus.CheckedIn, 2500m),
        ("Ramon Bautista",    "Chevrolet Camaro SS",     "GHJ 8908", RegistrationType.Exhibitor,  RegistrationStatus.CheckedIn, 1500m),
        ("Ana Villanueva",    "Porsche 911 Carrera",     "HJK 9009", RegistrationType.VIP,        RegistrationStatus.CheckedIn, 5000m),
        ("Juan Cruz Jr.",     "Toyota Supra MK4",        "JKL 1010", RegistrationType.Competitor, RegistrationStatus.Cancelled, 1000m),
        ("Grace Aquino",      "Honda S2000",             "KLM 1111", RegistrationType.Exhibitor,  RegistrationStatus.Cancelled, 0m),
        ("Victor Navarro",    "BMW M3 E46",              "LMN 1212", RegistrationType.VIP,        RegistrationStatus.Cancelled, 5000m),
        ("Rosa Castillo",     "Nissan 370Z",             "MNP 1313", RegistrationType.Exhibitor,  RegistrationStatus.Confirmed, 750m),
        ("Daniel Flores",     "Mazda MX-5 NA",           "NPQ 1414", RegistrationType.Competitor, RegistrationStatus.Confirmed, 2500m),
        ("Elena Domingo",     "Lexus LFA",               "PQR 1515", RegistrationType.VIP,        RegistrationStatus.Confirmed, 3000m),
        ("Mark Fernandez",    "Honda Integra DC2",       "QRS 1616", RegistrationType.Exhibitor,  RegistrationStatus.Pending,   0m),
        ("Joy Pascual",       "Toyota GR86",             "RST 1717", RegistrationType.Competitor, RegistrationStatus.Pending,   0m),
        ("Allan Santiago",    "Audi R8",                 "STU 1818", RegistrationType.VIP,        RegistrationStatus.Pending,   0m),
        ("Nina Cortez",       "Suzuki Swift Sport",      "TUV 1919", RegistrationType.Exhibitor,  RegistrationStatus.CheckedIn, 1000m),
        ("Oscar Salazar",     "Subaru BRZ",              "UVW 2020", RegistrationType.Competitor, RegistrationStatus.CheckedIn, 1500m),
        ("Diana Morales",     "Mercedes-AMG GT",         "VWX 2121", RegistrationType.VIP,        RegistrationStatus.CheckedIn, 4000m),
        ("Ben Alcantara",     "Volkswagen Golf GTI",     "WXY 2222", RegistrationType.Exhibitor,  RegistrationStatus.Confirmed, 1500m),
        ("Tina Rivera",       "Mini Cooper S",           "XYZ 2323", RegistrationType.Competitor, RegistrationStatus.Confirmed, 500m),
        ("Rico Dizon",        "Hyundai Veloster N",      "YZA 2424", RegistrationType.Exhibitor,  RegistrationStatus.Pending,   1500m),
        ("Juan Dela Pena",    "Datsun 240Z",             "ZAB 2525", RegistrationType.Competitor, RegistrationStatus.Pending,   2500m),
    ];

    public static void Seed(AppDbContext db)
    {
        if (db.Registrations.Any()) return; // never seed twice

        var now = DateTime.UtcNow;

        for (var i = 0; i < Rows.Length; i++)
        {
            var row = Rows[i];
            var number = i + 1;

            db.Registrations.Add(new Registration
            {
                Id = Guid.NewGuid(),
                RegistrationNumber = $"CS27-{number:D4}",
                // older numbers = older dates, so new registrations always appear first
                RegistrationDate = now.AddDays(-(Rows.Length + 1 - number)),
                ParticipantName = row.Name,
                ContactNumber = $"0917{number:D7}",
                Email = $"{row.Name.ToLowerInvariant().Replace(" ", ".").Replace("..", ".").TrimEnd('.')}@example.com",
                Address = $"{number} Rizal St., Sorsogon City",
                CarMakeModel = row.Car,
                PlateNumber = row.Plate,
                RegistrationType = row.Type,
                Status = row.Status,
                RegistrationFee = RegistrationFees.For(row.Type),
                AmountPaid = row.Paid
            });
        }

        db.SaveChanges();
    }
}
