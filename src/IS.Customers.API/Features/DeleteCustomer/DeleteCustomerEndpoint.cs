using IS.Core.API.Controllers;
using IS.Core.API.Results;
using IS.Core.Communication;
using IS.Core.Mediator.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IS.Customers.API.Features.DeleteCustomer
{
    [Route("api/customer")]
    [Tags("Customers")]
    public class DeleteCustomerEndpoint : BaseController
    {
        private readonly IMediator _mediator;

        public DeleteCustomerEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Deleta um cliente
        /// </summary>
        /// <param name="id">Id do cliente</param>
        /// <returns>Resultado da deleção</returns>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteCustomerAsync([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            BaseResult<bool> result = await _mediator.SendAsync(new DeleteCustomerCommand(id), cancellationToken);

            return result.IsValid ? Ok() : BadRequestProblem(result.Errors);
        }
    }
}
