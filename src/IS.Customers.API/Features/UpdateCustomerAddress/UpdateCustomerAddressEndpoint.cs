using IS.Core.API.Controllers;
using IS.Core.API.Results;
using IS.Core.Communication;
using IS.Core.Mediator.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IS.Customers.API.Features.UpdateCustomerAddress
{
    [Route("api/customer/{customerId:guid}/address")]
    [Tags("Customers")]
    public class UpdateCustomerAddressEndpoint : BaseController
    {
        private readonly IMediator _mediator;

        public UpdateCustomerAddressEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Update a customer address.
        /// </summary>
        /// <param name="customerId">Customer id</param>
        /// <param name="addressId">Address id</param>
        /// <param name="command">Address data to update</param>
        /// <returns>Returns the operation status.</returns>
        /// <remarks>
        /// Sample request body:
        ///
        ///     {
        ///         "street": "Av. Paulista",
        ///         "number": "1000",
        ///         "addressComplement": "10th floor",
        ///         "city": "Sao Paulo",
        ///         "state": "SP",
        ///         "country": "Brazil",
        ///         "isMainAddress": true
        ///     }
        ///
        /// </remarks>
        [HttpPut("{addressId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateCustomerAddressAsync([FromRoute] Guid customerId,
                                                                    [FromRoute] Guid addressId,
                                                                    [FromBody] UpdateCustomerAddressCommand command,
                                                                    CancellationToken cancellationToken)
        {
            command.CustomerId = customerId;
            command.AddressId = addressId;

            BaseResult<bool> result = await _mediator.SendAsync(command, cancellationToken);

            return result.IsValid ? Ok() : BadRequestProblem(result.Errors);
        }
    }
}
