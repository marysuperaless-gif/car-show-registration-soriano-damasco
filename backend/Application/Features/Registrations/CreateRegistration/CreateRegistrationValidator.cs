namespace Application.Features.Registrations.CreateRegistration;

using Domain.Entities;
using FluentValidation;

public sealed class CreateRegistrationValidator : AbstractValidator<CreateRegistrationCommand>
{
    public CreateRegistrationValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.ParticipantName).NotEmpty().MaximumLength(150);

        RuleFor(x => x.ContactNumber)
            .NotEmpty()
            .Matches(@"^09\d{9}$")
            .WithMessage("'Contact Number' must be a PH mobile number in the format 09XXXXXXXXX (11 digits).");

        RuleFor(x => x.Email).NotEmpty().MaximumLength(200).EmailAddress();

        RuleFor(x => x.Address).NotEmpty().MaximumLength(300);

        RuleFor(x => x.CarMakeModel).NotEmpty().MaximumLength(100);

        RuleFor(x => x.PlateNumber).NotEmpty().MaximumLength(15);

        RuleFor(x => x.RegistrationType)
            .IsInEnum()
            .WithMessage("'Registration Type' must be Exhibitor, Competitor or VIP.");

        RuleFor(x => x.AmountPaid)
            .GreaterThanOrEqualTo(0m)
            .WithMessage("'Amount Paid' must be 0 or more.");

        // AmountPaid cannot be more than the fee of the chosen type.
        RuleFor(x => x).Custom((command, context) =>
        {
            if (!Enum.IsDefined(command.RegistrationType) || command.AmountPaid < 0) return;

            var fee = RegistrationFees.For(command.RegistrationType);
            if (command.AmountPaid > fee)
            {
                context.AddFailure(
                    nameof(CreateRegistrationCommand.AmountPaid),
                    $"'Amount Paid' cannot be more than the fee ({fee:N0}) for a {command.RegistrationType} registration.");
            }
        });
    }
}
