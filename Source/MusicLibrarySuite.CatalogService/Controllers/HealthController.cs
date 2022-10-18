using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MusicLibrarySuite.CatalogService.Controllers;

/// <summary>
/// Represents a Web API controller for health checks.
/// </summary>
[ApiController]
[Route("api/[controller]/[action]")]
public class HealthController : Controller
{
    /// <summary>
    /// Performs a health check.
    /// </summary>
    /// <returns>The task object representing the asynchronous operation.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult CheckAsync()
    {
        return Ok();
    }
}
