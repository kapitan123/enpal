using System.Text.Json.Serialization;

namespace SchedulerApi.Controllers;

public class GetAvailableManagersRequest
{
	public DateTime Date { get; set; }

	[JsonPropertyName("products")]
	public string[] Products { get; set; }

	public string Language { get; set; }

	public string Rating { get; set; }
}
