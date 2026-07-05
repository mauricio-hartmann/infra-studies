using FluentValidation;

namespace IS.Customers.API.Features.AddCustomerAddress
{
    public class AddCustomerAddressCommandValidator : AbstractValidator<AddCustomerAddressCommand>
    {
        public AddCustomerAddressCommandValidator()
        {
            RuleFor(x => x.CustomerId)
                .NotEmpty()
                    .WithName("Customer id")
                    .WithMessage("{PropertyName} is required!");

            RuleFor(x => x.Street)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                    .WithMessage("{PropertyName} is required!")
                .MaximumLength(255)
                    .WithMessage("{PropertyName} must have a maximum of {MaxLength} characters. You entered {TotalLength} characters.");

            RuleFor(x => x.Number)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                    .WithMessage("{PropertyName} is required!")
                .MaximumLength(10)
                    .WithMessage("{PropertyName} must have a maximum of {MaxLength} characters. You entered {TotalLength} characters.");

            RuleFor(x => x.AddressComplement)
                .MaximumLength(50)
                    .WithName("Address complement")
                    .WithMessage("{PropertyName} must have a maximum of {MaxLength} characters. You entered {TotalLength} characters.");

            RuleFor(x => x.City)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                    .WithMessage("{PropertyName} is required!")
                .MaximumLength(255)
                    .WithMessage("{PropertyName} must have a maximum of {MaxLength} characters. You entered {TotalLength} characters.");

            RuleFor(x => x.State)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                    .WithMessage("{PropertyName} is required!")
                .MaximumLength(5)
                    .WithMessage("{PropertyName} must have a maximum of {MaxLength} characters. You entered {TotalLength} characters.");

            RuleFor(x => x.Country)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                    .WithMessage("{PropertyName} is required!")
                .MaximumLength(255)
                    .WithMessage("{PropertyName} must have a maximum of {MaxLength} characters. You entered {TotalLength} characters.");
        }
    }
}
