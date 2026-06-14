using IS.Core.API.Results;
using Microsoft.AspNetCore.Mvc;

namespace IS.Core.API.Controllers
{
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected IActionResult BadRequestProblem(IDictionary<string, ICollection<string>> errors)
        {
            return BadRequest(new BadRequestProblemDetails(errors, HttpContext));
        }
    }
}
