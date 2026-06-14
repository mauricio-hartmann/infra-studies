using IS.Core.API.Controllers;
using IS.Core.Mediator.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IS.Customers.API.Features.GetCustomerById
{
    [Route("api/customer")]
    public class GetCustomerByIdEndpoint : BaseController
    {
        private readonly IMediator _mediator;

        public GetCustomerByIdEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get a customer by Id
        /// </summary>
        /// <param name="id">Customer Id</param>
        /// <returns>Customer</returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(CustomerDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> CreateCustomerAsync([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            CustomerDTO result = await _mediator.SendAsync(new GetCustomerByIdQuery(id), cancellationToken);

            return result == null ? NoContent() : Ok(result);
        }
    }
}
