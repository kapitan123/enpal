using System.Text.Json.Serialization;
using SchedulerApi.Domain;

namespace SchedulerApi.Controllers;

// AK TODO add validation
public class GetAvailableManagersRequest
{
	[JsonPropertyName("date")]
	public DateTime Date { get; set; }

	[JsonPropertyName("products")]
	public Product[] Products { get; set; }

	[JsonPropertyName("language")]
	public Language Language { get; set; }

	[JsonPropertyName("rating")]
	public Rating Rating { get; set; }
}
