using IS.Core.Communication;
using IS.Core.Mediator.Interfaces;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace IS.Customers.API.Features.CreateCustomer
{
    [ApiController]
    public class CreateCustomerEndpoint : Controller
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
        [HttpPost("api/customer")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string[]), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateCustomerAsync([FromBody] CreateCustomerCommand command, CancellationToken cancellationToken)
        {
            BaseResult<Guid> result = await _mediator.SendAsync(command, cancellationToken);

            return result.IsValid ? Created() : BadRequest(new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                { "Mensagens", result.Errors.ToArray() }
            }));
        }
    }
}
