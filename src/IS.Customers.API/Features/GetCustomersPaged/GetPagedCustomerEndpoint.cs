using IS.Core.API.Controllers;
using IS.Core.API.Results;
using IS.Core.Communication;
using IS.Core.Data.Pagination;
using IS.Core.Mediator.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IS.Customers.API.Features.GetCustomersPaged
{
    [Route("api/customer")]
    [Tags("Customers")]
    public class GetPagedCustomerEndpoint : BaseController
    {
        private readonly IMediator _mediator;

        public GetPagedCustomerEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("paged")]
        [ProducesResponseType(typeof(PagedResult<PagedCustomerDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(BadRequestProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPagedCustomersAsync([FromQuery] GetPagedCustomersQuery query, CancellationToken cancellationToken)
        {
            BaseResult<PagedResult<PagedCustomerDTO>> result = await _mediator.SendAsync(query, cancellationToken);

            if (!result.IsValid)
                return BadRequestProblem(result.Errors);

            return result.Response!.TotalCount > 0 ? Ok(result.Response) : NoContent();
        }
    }
}
