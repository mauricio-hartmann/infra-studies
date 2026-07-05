using FluentValidation;

namespace IS.Customers.API.Features.DeleteCustomerAddress
{
    public class DeleteCustomerAddressCommandValidator : AbstractValidator<DeleteCustomerAddressCommand>
    {
        public DeleteCustomerAddressCommandValidator()
        {
            RuleFor(x => x.CustomerId)
                .NotEmpty()
                    .WithName("Customer id")
                    .WithMessage("{PropertyName} is required!");

            RuleFor(x => x.AddressId)
                .NotEmpty()
                    .WithName("Address id")
                    .WithMessage("{PropertyName} is required!");
        }
    }
}
