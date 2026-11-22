namespace Application.Features.Registrations.CreateRegistration;

using Application.Abstractions.Messaging;
using Domain.Common;
using Domain.Enums;

// The client NEVER sends number, date, fee or status. Extra JSON fields are simply ignored.
public sealed record CreateRegistrationCommand(
    string ParticipantName,
    string ContactNumber,
    string Email,
    string Address,
    string CarMakeModel,
    string PlateNumber,
    RegistrationType RegistrationType,
    decimal AmountPaid) : ICommand<Result<CreateRegistrationResponse>>;

public sealed record CreateRegistrationResponse(
    Guid Id,
    string RegistrationNumber,
    DateTime RegistrationDate,
    string ParticipantName,
    string ContactNumber,
    string Email,
    string Address,
    string CarMakeModel,
    string PlateNumber,
    RegistrationType RegistrationType,
    RegistrationStatus Status,
    decimal RegistrationFee,
    decimal AmountPaid,
    PaymentStatus PaymentStatus);
