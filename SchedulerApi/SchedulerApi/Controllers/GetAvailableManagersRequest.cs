using System.Text.Json.Serialization;

namespace SchedulerApi.Controllers;

public class GetAvailableManagersRequest
{
	[JsonPropertyName("date")]
	public DateOnly Date { get; set; }

	[JsonPropertyName("products")]
	public string[] Products { get; set; }

	[JsonPropertyName("language")]
	public string Language { get; set; }

	[JsonPropertyName("rating")]
	public string Rating { get; set; }
}
