namespace SchedulerApi.Controllers;

public class CheckAvailableBookingsResponse
{
	// AK TODO make it snake case

	// It is a best pratice to return a Data  body, but I need to check if this is an expectedd response
	public Slot[] Data { get; set; }

}

public class Slot
{
	public int AvailableCount { get; set; }

	// AK TODO check formatting
	public DateTime StartDate { get; set; }
}
