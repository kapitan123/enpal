namespace SchedulerApi.Domain;

public class AvailableBookings(DateTime startDate, DateTime endDate, int availableCount)
{
	public DateTime StartDate { get; set; } = startDate;
	public DateTime EndDate { get; set; } = endDate;
	public int ManagersCount { get; set; } = availableCount;
}