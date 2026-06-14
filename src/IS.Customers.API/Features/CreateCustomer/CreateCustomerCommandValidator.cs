using FluentValidation;
using IS.Customers.API.Data.Repositories.Interfaces;

namespace IS.Customers.API.Features.CreateCustomer
{
    public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
    {
        private readonly ICustomerRepository _customerRepository;

        public CreateCustomerCommandValidator(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;

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

            RuleFor(x => x.RegistrationNumber)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                    .WithName("Registration code")
                    .WithMessage("{PropertyName} is required!")
                .MaximumLength(50)
                    .WithMessage("{PropertyName} must have a maximum of {MaxLength} characters. You entered {TotalLength} characters.")
                .MustAsync(async (registrationNumber, cancellationToken) =>
                {
                    bool exists = await _customerRepository.ExistsByRegistrationNumberAsync(registrationNumber, cancellationToken);
                    return !exists;
                })
                    .WithMessage("A customer with same registration number already exists!");

            RuleFor(x => x.Email)
                .MaximumLength(100)
                .WithMessage("{PropertyName} must have a maximum of {MaxLength} characters. You entered {TotalLength} characters.");

            RuleFor(x => x.MainPhone)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                    .WithName("Main phone")
                    .WithMessage("{PropertyName} is required!")
                .MaximumLength(50)
                    .WithMessage("{PropertyName} must have a maximum of {MaxLength} characters. You entered {TotalLength} characters.");

            RuleFor(x => x.SecondaryPhone)
                .MaximumLength(50)
                .WithName("Secondary phone")
                .WithMessage("{PropertyName} must have a maximum of {MaxLength} characters. You entered {TotalLength} characters.");

            RuleFor(x => x.SiteUrl)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(100)
                .WithName("Site URL")
                .WithMessage("{PropertyName} must have a maximum of {MaxLength} characters. You entered {TotalLength} characters.");

            RuleFor(x => x.MainContactName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                    .WithName("Main contact name")
                    .WithMessage("{PropertyName} is required!")
                .MaximumLength(255)
                    .WithMessage("{PropertyName} must have a maximum of {MaxLength} characters. You entered {TotalLength} characters.");
        }
    }
}
