namespace Application.Features.Registrations.GetRegistrations;

using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Common;
using Domain.Common;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

public sealed class GetRegistrationsQueryHandler(IAppDbContext dbContext)
    : IQueryHandler<GetRegistrationsQuery, Result<GetRegistrationsResponse>>
{
    public async Task<Result<GetRegistrationsResponse>> HandleAsync(
        GetRegistrationsQuery query,
        CancellationToken cancellationToken = default)
    {
        var registrations = dbContext.Registrations.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.ParticipantName))
        {
            // LIKE is case-insensitive in SQLite. Escape wildcard characters typed by the user.
            var term = query.ParticipantName.Trim()
                .Replace("\\", "\\\\")
                .Replace("%", "\\%")
                .Replace("_", "\\_");
            var pattern = $"%{term}%";
            registrations = registrations.Where(r => EF.Functions.Like(r.ParticipantName, pattern, "\\"));
        }

        if (EnumNames.TryParse<RegistrationType>(query.Type, out var type))
        {
            registrations = registrations.Where(r => r.RegistrationType == type);
        }

        if (EnumNames.TryParse<RegistrationStatus>(query.Status, out var status))
        {
            registrations = registrations.Where(r => r.Status == status);
        }

        var totalCount = await registrations.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize);

        var rows = await registrations
            .OrderByDescending(r => r.RegistrationDate)
            .ThenByDescending(r => r.RegistrationNumber)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        var items = rows.Select(r => new RegistrationItem(
            r.Id,
            r.RegistrationNumber,
            r.RegistrationDate,
            r.ParticipantName,
            r.ContactNumber,
            r.Email,
            r.Address,
            r.CarMakeModel,
            r.PlateNumber,
            r.RegistrationType,
            r.Status,
            r.RegistrationFee,
            r.AmountPaid,
            r.PaymentStatus)).ToList();

        return Result<GetRegistrationsResponse>.Success(
            new GetRegistrationsResponse(items, query.Page, query.PageSize, totalCount, totalPages));
    }
}
