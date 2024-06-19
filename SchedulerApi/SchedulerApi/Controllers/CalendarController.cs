using Microsoft.AspNetCore.Mvc;

namespace SchedulerApi.Controllers;
[ApiController]
[Route("[controller]")]
public class CalendarController : ControllerBase
{
	private static readonly string[] Summaries = new[]
	{
		"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
	};

	private readonly ILogger<CalendarController> _logger;

	public CalendarController(ILogger<CalendarController> logger)
	{
		_logger = logger;
	}

	[HttpPost("query", Name = "CheckAvailableBookings")]
	[Produces("application/json")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> Get()
	{
		throw new InvalidCastException();
	}
}
