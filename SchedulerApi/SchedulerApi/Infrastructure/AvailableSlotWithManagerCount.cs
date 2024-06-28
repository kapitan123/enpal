namespace SchedulerApi.Infrastructure;

public class AvailableSlotWithManagerCount
{
	public DateTime StartDate { get; set; }
	public DateTime EndDate { get; set; }
	public int AvailableManagerCount { get; set; }
}
