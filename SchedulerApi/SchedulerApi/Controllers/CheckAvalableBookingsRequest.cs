namespace SchedulerApi.Controllers;

public class CheckAvalableBookingsRequest
{
	// AK TODO it is expeced to be in UTC
	public DateTime Date { get; set; }

	public string[] Products { get; set; }

	// AK TODO these two are probably enums
	public string Language { get; set; }

	public string Rating { get; set; }

}
