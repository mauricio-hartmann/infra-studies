using FluentValidation;

namespace IS.Customers.API.Features.GetCustomersPaged
{
    public class GetPagedCustomersQueryValidator : AbstractValidator<GetPagedCustomersQuery>
    {
        public GetPagedCustomersQueryValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0)
                .WithName("Page number")
                .WithMessage("{PropertyName} must be greater than {ComparisonValue}.");

            RuleFor(x => x.PageSize)
                .GreaterThan(0)
                .WithName("Page size")
                .WithMessage("{PropertyName} must be greater than {ComparisonValue}.");

            When(x => x.Filter != null, () =>
            {
                RuleFor(x => x.Filter)
                .NotEmpty()
                .WithMessage("{PropertyName} cannot be empty.");
            });
        }
    }
}
