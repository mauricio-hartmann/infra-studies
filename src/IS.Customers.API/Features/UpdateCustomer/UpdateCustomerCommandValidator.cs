using FluentValidation;

namespace IS.Customers.API.Features.UpdateCustomer
{
    public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
    {
        public UpdateCustomerCommandValidator()
        {
            RuleFor(x => x.LegalName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                    .WithName("Legal name")
                    .WithMessage("{PropertyName} is required!")
                .MaximumLength(255)
                    .WithMessage("{PropertyName} must have a maximum of {MaxLength} characters. You entered {TotalLength} characters.");

            RuleFor(x => x.TradeName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                    .WithName("Trade name")
                    .WithMessage("{PropertyName} is required!")
                .MaximumLength(255)
                    .WithMessage("{PropertyName} must have a maximum of {MaxLength} characters. You entered {TotalLength} characters.");

            RuleFor(x => x.MainPhone)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                    .WithName("Main phone")
                    .WithMessage("{PropertyName} is required!")
                .MaximumLength(50)
                    .WithMessage("{PropertyName} must have a maximum of {MaxLength} characters. You entered {TotalLength} characters.");

            RuleFor(x => x.MainContactName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                    .WithName("Main contact name")
                    .WithMessage("{PropertyName} is required!")
                .MaximumLength(255)
                    .WithMessage("{PropertyName} must have a maximum of {MaxLength} characters. You entered {TotalLength} characters.");

            When(x => !string.IsNullOrEmpty(x.Email), () =>
            {
                RuleFor(x => x.Email)
                    .MaximumLength(100)
                        .WithMessage("{PropertyName} must have a maximum of {MaxLength} characters. You entered {TotalLength} characters.");
            });

            When(x => !string.IsNullOrEmpty(x.SecondaryPhone), () =>
            {
                RuleFor(x => x.SecondaryPhone)
                    .MaximumLength(50)
                        .WithName("Secondary phone")
                        .WithMessage("{PropertyName} must have a maximum of {MaxLength} characters. You entered {TotalLength} characters.");
            });

            When(x => !string.IsNullOrEmpty(x.SiteUrl), () =>
            {
                RuleFor(x => x.SiteUrl)
                    .MaximumLength(100)
                        .WithName("Site URL")
                        .WithMessage("{PropertyName} must have a maximum of {MaxLength} characters. You entered {TotalLength} characters.");
            });
        }
    }
}
