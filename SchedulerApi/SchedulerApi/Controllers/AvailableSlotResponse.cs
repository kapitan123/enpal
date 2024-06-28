using System.Text.Json.Serialization;

namespace SchedulerApi.Controllers;


public class AvailableSlotResponse
{
	[JsonPropertyName("start_date")]
	public string StartDate { get; set; }

	[JsonPropertyName("available_count")]
	public int AvailableCount { get; set; }
}

