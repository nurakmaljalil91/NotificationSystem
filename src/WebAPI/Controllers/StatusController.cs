using Ardalis.GuardClauses;
using Domain.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

/// <summary>
/// Controller for checking the API status and build version.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class StatusController : ControllerBase
{
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="StatusController"/> class.
    /// </summary>
    /// <param name="configuration">The configuration instance.</param>
    public StatusController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the current API status and build version.
    /// </summary>
    /// <returns>An <see cref="ApiStatus"/> object containing the status and build version.</returns>
    [HttpGet]
    public Task<ActionResult> GetStatus()
    {
        var buildVersion = _configuration.GetValue<string>("buildVersion");

        Guard.Against.NullOrEmpty(buildVersion, message: "Build version is not found");

        return Task.FromResult<ActionResult>(Ok(new ApiStatus
        {
            Status = "Online",
            BuildVersion = buildVersion,
        }));
    }
}
