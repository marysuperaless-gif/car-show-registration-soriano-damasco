namespace Application.Features.Registrations.CreateRegistration;

using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

public sealed class CreateRegistrationCommandHandler(IAppDbContext dbContext)
    : ICommandHandler<CreateRegistrationCommand, Result<CreateRegistrationResponse>>
{
    private const string NumberPrefix = "CS27-";

    public async Task<Result<CreateRegistrationResponse>> HandleAsync(
        CreateRegistrationCommand command,
        CancellationToken cancellationToken = default)
    {
        var plate = command.PlateNumber.Trim().ToUpperInvariant();

        var plateExists = await dbContext.Registrations
            .AnyAsync(r => r.PlateNumber == plate, cancellationToken);
        if (plateExists)
        {
            return Result<CreateRegistrationResponse>.Failure(Error.Conflict(
                "Registration.Conflict",
                $"A registration with plate number '{plate}' already exists."));
        }

        // Sequential number: CS27-0001, CS27-0002, ...
        var lastNumber = await dbContext.Registrations
            .OrderByDescending(r => r.RegistrationNumber)
            .Select(r => r.RegistrationNumber)
            .FirstOrDefaultAsync(cancellationToken);

        var nextSequence = 1;
        if (lastNumber is not null && int.TryParse(lastNumber.AsSpan(NumberPrefix.Length), out var last))
        {
            nextSequence = last + 1;
        }

        var registration = new Registration
        {
            Id = Guid.NewGuid(),
            RegistrationNumber = $"{NumberPrefix}{nextSequence:D4}",
            RegistrationDate = DateTime.UtcNow,
            ParticipantName = command.ParticipantName.Trim(),
            ContactNumber = command.ContactNumber.Trim(),
            Email = command.Email.Trim(),
            Address = command.Address.Trim(),
            CarMakeModel = command.CarMakeModel.Trim(),
            PlateNumber = plate,
            RegistrationType = command.RegistrationType,
            Status = RegistrationStatus.Pending,
            RegistrationFee = RegistrationFees.For(command.RegistrationType),
            AmountPaid = command.AmountPaid
        };

        dbContext.Registrations.Add(registration);

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            // The unique index on PlateNumber / RegistrationNumber caught a race condition.
            return Result<CreateRegistrationResponse>.Failure(Error.Conflict(
                "Registration.Conflict",
                "The registration could not be saved because the plate number or registration number already exists. Please try again."));
        }

        return Result<CreateRegistrationResponse>.Success(new CreateRegistrationResponse(
            registration.Id,
            registration.RegistrationNumber,
            registration.RegistrationDate,
            registration.ParticipantName,
            registration.ContactNumber,
            registration.Email,
            registration.Address,
            registration.CarMakeModel,
            registration.PlateNumber,
            registration.RegistrationType,
            registration.Status,
            registration.RegistrationFee,
            registration.AmountPaid,
            registration.PaymentStatus));
    }
}
