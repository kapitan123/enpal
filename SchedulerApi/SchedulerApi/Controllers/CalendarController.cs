using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SchedulerApi.Domain;

namespace SchedulerApi.Controllers;
[ApiController]
[Route("calendar")]
public class CalendarController(ILogger<CalendarController> logger, IValidator<GetAvailableManagersRequest> validator, IAvalableBookingsService bookingsService) : ControllerBase
{
	[HttpPost("query", Name = "GetAvailableSlots")]
	[Produces("application/json")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> GetAvailableSlots([FromBody] GetAvailableManagersRequest req)
	{
		var validationResult = validator.Validate(req);

		if (!validationResult.IsValid)
		{
			return BadRequest(validationResult.Errors);
		}

		_ = Enum.TryParse<Language>(req.Language, out var language);
		_ = Enum.TryParse<Rating>(req.Language, out var rating);
		var products = req.Products.Select(str =>
		{
			_ = Enum.TryParse<Product>(str, out var product);
			return product;

		}).ToArray();

		var result = await bookingsService.GetAvailableSlotsWithManagerCountAsync(language, rating, products, DateOnly.FromDateTime(req.Date));
		// AK TODO convert to request
		return Ok(result);
	}
}
