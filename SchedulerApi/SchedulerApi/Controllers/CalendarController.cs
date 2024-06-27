using Microsoft.AspNetCore.Mvc;
using SchedulerApi.Domain;

namespace SchedulerApi.Controllers;
[ApiController]
[Route("[controller]")]
public class CalendarController(ILogger<CalendarController> logger, IAvalableBookingsService bookingsService) : ControllerBase
{
	[HttpPost("query", Name = "CheckAvailableBookings")]
	[Produces("application/json")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> Get([FromBody] GetAvailableManagersRequest req)
	{
		// AK TODO add validation
		// AK TODO add no results handling
		var result = await bookingsService.GetAvailableSlotsWithManagerCountAsync(req.Language, req.Rating, req.Products, DateOnly.FromDateTime(req.Date));
		// AK TODO convert to request
		return Ok(result);
	}
}
