using IS.Core.API.Controllers;
using IS.Core.API.Results;
using IS.Core.Communication;
using IS.Core.Mediator.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IS.Customers.API.Features.CreateCustomer
{
    [Route("api/customer")]
    [Tags("Customers")]
    public class CreateCustomerEndpoint : BaseController
    {
        private readonly IMediator _mediator;

        public CreateCustomerEndpoint(IMediator merdiator)
        {
            _mediator = merdiator;
        }

        /// <summary>
        ///  Create a new customer
        /// </summary>
        /// <returns>Customer created</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(BadRequestProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateCustomerAsync([FromBody] CreateCustomerCommand command, CancellationToken cancellationToken)
        {
            BaseResult<Guid> result = await _mediator.SendAsync(command, cancellationToken);

            return result.IsValid ? Created($"api/customer/{result.Response}", new { id = result.Response })
                                  : BadRequestProblem(result.Errors);
        }
    }
}
