using IS.Core.API.Controllers;
using IS.Core.API.Results;
using IS.Core.Communication;
using IS.Core.Mediator.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IS.Customers.API.Features.UpdateCustomer
{
    [Route("api/customer")]
    [Tags("Customers")]
    public class UpdateCustomerEndpoint : BaseController
    {
        private readonly IMediator _mediator;

        public UpdateCustomerEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Update a customer.
        /// </summary>
        /// <param name="id">The customer ID.</param>
        /// <param name="command">The customer data to update.</param>
        /// <returns>Returns the operation status.</returns>
        /// <remarks>
        /// Sample request body:
        ///
        ///     {
        ///         "legalName": "Acme Corporation LTDA",
        ///         "tradeName": "Acme",
        ///         "email": "contact@acme.com",
        ///         "mainPhone": "+55 11 99999-0000",
        ///         "secondaryPhone": "+55 11 88888-0000",
        ///         "siteUrl": "https://www.acme.com",
        ///         "mainContactName": "John Doe"
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Returns the operation status</response>
        /// <response code="400">If validation fails or customer is not found</response>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateCustomerAsync([FromRoute] Guid id, [FromBody] UpdateCustomerCommand command, CancellationToken cancellationToken)
        {
            command.Id = id;
            BaseResult<bool> result = await _mediator.SendAsync(command, cancellationToken);

            return result.IsValid ? Ok() : BadRequestProblem(result.Errors);
        }
    }
}
