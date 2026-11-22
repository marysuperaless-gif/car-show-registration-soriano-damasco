namespace Application.Features.Registrations.GetRegistrations;

using Application.Abstractions.Messaging;
using Domain.Common;
using Domain.Enums;

// type and status are strings so invalid values give a clean 400 from the validator.
public sealed record GetRegistrationsQuery(
    string? ParticipantName,
    string? Type,
    string? Status,
    int Page = 1,
    int PageSize = 10) : IQuery<Result<GetRegistrationsResponse>>;

public sealed record RegistrationItem(
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

public sealed record GetRegistrationsResponse(
    IReadOnlyList<RegistrationItem> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);
