using FluentValidation.Results;
using IS.Core.Communication;
using IS.Core.Mediator.Interfaces;
using IS.Core.Messaging.Events.IntegrationEvents;
using IS.Core.Messaging.Outbox;
using IS.Customers.API.Data;
using IS.Customers.API.Entities;

namespace IS.Customers.API.Features.CreateCustomer
{
    public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, BaseResult<Guid>>
    {
        private readonly CreateCustomerCommandValidator _validator;
        private readonly CustomerDbContext _customerDbContext;

        public CreateCustomerCommandHandler(CreateCustomerCommandValidator validator,
                                            CustomerDbContext customerDbContext)
        {
            _validator = validator;
            _customerDbContext = customerDbContext;
        }

        public async Task<BaseResult<Guid>> HandleAsync(CreateCustomerCommand request, CancellationToken cancellationToken = default)
        {
            ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                return BaseResult<Guid>.Failure(validationResult.ToDictionary());

            Customer customer = CreateCustomer(request);
            OutboxMessage customerCreatedOutboxMessage = CustomerCreatedOutboxMessage(customer);           
            await _customerDbContext.Customers.AddAsync(customer, cancellationToken);
            await _customerDbContext.OutboxMessages.AddAsync(customerCreatedOutboxMessage, cancellationToken);
            await _customerDbContext.SaveChangesAsync(cancellationToken);

            return BaseResult<Guid>.Success(customer.Id);
        }

        private Customer CreateCustomer(CreateCustomerCommand command)
        {
            return new Customer(command.LegalName, command.TradeName, command.RegistrationNumber)
            {
                Email = command.Email,
                MainPhone = command.MainPhone,
                SecondaryPhone = command.SecondaryPhone,
                SiteUrl = command.SiteUrl,
                MainContactName = command.MainContactName
            };
        }

        private OutboxMessage CustomerCreatedOutboxMessage(Customer customer)
        {
            var payload = new
            {
                CustomerId = customer.Id,
                customer.LegalName,
                customer.TradeName,
                customer.RegistrationNumber,
                customer.DateCreated
            };

            return new OutboxMessage(customer.Id, Events.CustomerCreatedIntegrationEvent, payload);
        }
    }
}
