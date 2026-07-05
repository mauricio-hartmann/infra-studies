using IS.Core.API.Controllers;
using IS.Core.API.Results;
using IS.Core.Communication;
using IS.Core.Mediator.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IS.Customers.API.Features.DeleteCustomerAddress
{
    [Route("api/customer/{customerId:guid}/address")]
    [Tags("Customers")]
    public class DeleteCustomerAddressEndpoint : BaseController
    {
        private readonly IMediator _mediator;

        public DeleteCustomerAddressEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Delete a customer address
        /// </summary>
        /// <param name="customerId">Customer id</param>
        /// <param name="addressId">Address id to delete</param>
        /// <param name="command">Optional new main address id</param>
        /// <returns>Deletion result</returns>
        [HttpDelete("{addressId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteCustomerAddressAsync([FromRoute] Guid customerId,
                                                                    [FromRoute] Guid addressId,
                                                                    [FromBody] DeleteCustomerAddressCommand command,
                                                                    CancellationToken cancellationToken)
        {
            command.CustomerId = customerId;
            command.AddressId = addressId;

            BaseResult<bool> result = await _mediator.SendAsync(command, cancellationToken);

            return result.IsValid ? Ok() : BadRequestProblem(result.Errors);
        }
    }
}
