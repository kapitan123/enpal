namespace SchedulerApi.Infrastructure;

public class Slot
{
	public int Id { get; set; }
	public DateTime StartDate { get; set; }
	public DateTime EndDate { get; set; }
	public bool Booked { get; set; }
	public int SalesManagerId { get; set; }
}
