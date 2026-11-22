namespace Application.Features.Registrations.GetRegistrationSummary;

using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Common;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

public sealed class GetRegistrationSummaryQueryHandler(IAppDbContext dbContext)
    : IQueryHandler<GetRegistrationSummaryQuery, Result<GetRegistrationSummaryResponse>>
{
    public async Task<Result<GetRegistrationSummaryResponse>> HandleAsync(
        GetRegistrationSummaryQuery query,
        CancellationToken cancellationToken = default)
    {
        // SQLite cannot SUM decimal columns in SQL, so load only 3 small columns and add up in memory.
        var rows = await dbContext.Registrations
            .AsNoTracking()
            .Select(r => new { r.Status, r.RegistrationFee, r.AmountPaid })
            .ToListAsync(cancellationToken);

        // Cancelled registrations are excluded from the money totals.
        var active = rows.Where(r => r.Status != RegistrationStatus.Cancelled).ToList();
        var totalFees = active.Sum(r => r.RegistrationFee);
        var totalPayments = active.Sum(r => r.AmountPaid);

        return Result<GetRegistrationSummaryResponse>.Success(new GetRegistrationSummaryResponse(
            TotalRegistrations: rows.Count,
            Confirmed: rows.Count(r => r.Status == RegistrationStatus.Confirmed),
            CheckedIn: rows.Count(r => r.Status == RegistrationStatus.CheckedIn),
            Pending: rows.Count(r => r.Status == RegistrationStatus.Pending),
            Cancelled: rows.Count(r => r.Status == RegistrationStatus.Cancelled),
            TotalFees: totalFees,
            TotalPayments: totalPayments,
            UncollectedPayments: totalFees - totalPayments));
    }
}
