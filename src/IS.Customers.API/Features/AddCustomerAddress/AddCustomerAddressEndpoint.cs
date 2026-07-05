using IS.Core.API.Controllers;
using IS.Core.API.Results;
using IS.Core.Communication;
using IS.Core.Mediator.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IS.Customers.API.Features.AddCustomerAddress
{
    [Route("api/customer/{customerId:guid}/address")]
    [Tags("Customers")]
    public class AddCustomerAddressEndpoint : BaseController
    {
        private readonly IMediator _mediator;

        public AddCustomerAddressEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        ///  Add a new customer address
        /// </summary>
        /// <param name="customerId">Customer id</param>
        /// <param name="command">Address data</param>
        /// <returns>Address created</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(BadRequestProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddCustomerAddressAsync([FromRoute] Guid customerId,
                                                                 [FromBody] AddCustomerAddressCommand command,
                                                                 CancellationToken cancellationToken)
        {
            command.CustomerId = customerId;

            BaseResult<Guid> result = await _mediator.SendAsync(command, cancellationToken);

            return result.IsValid ? Created($"api/customer/{customerId}/address/{result.Response}", new { id = result.Response })
                                  : BadRequestProblem(result.Errors);
        }
    }
}
