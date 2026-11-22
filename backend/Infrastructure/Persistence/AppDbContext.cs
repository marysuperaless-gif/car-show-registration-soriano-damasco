namespace Infrastructure.Persistence;

using Application.Abstractions.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

public sealed class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Registration> Registrations => Set<Registration>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Registration>(entity =>
        {
            entity.ToTable("Registrations");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.RegistrationNumber).IsRequired().HasMaxLength(20);

            // SQLite returns DateTime with Kind=Unspecified; force UTC so JSON ends with "Z".
            entity.Property(e => e.RegistrationDate)
                .IsRequired()
                .HasConversion(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

            entity.Property(e => e.ParticipantName).IsRequired().HasMaxLength(150);
            entity.Property(e => e.ContactNumber).IsRequired().HasMaxLength(11);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Address).IsRequired().HasMaxLength(300);
            entity.Property(e => e.CarMakeModel).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PlateNumber).IsRequired().HasMaxLength(15);

            // Enums are stored as readable text ("Competitor", "Pending", ...)
            entity.Property(e => e.RegistrationType).HasConversion<string>().HasMaxLength(20).IsRequired();
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(20).IsRequired();

            entity.Property(e => e.RegistrationFee).HasPrecision(18, 2);
            entity.Property(e => e.AmountPaid).HasPrecision(18, 2);

            // PaymentStatus is computed, not stored.
            entity.Ignore(e => e.PaymentStatus);

            // One car = one registration; numbers are unique.
            entity.HasIndex(e => e.PlateNumber).IsUnique();
            entity.HasIndex(e => e.RegistrationNumber).IsUnique();
        });
    }
}
