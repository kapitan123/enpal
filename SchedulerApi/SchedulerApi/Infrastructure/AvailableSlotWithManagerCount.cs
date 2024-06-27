﻿namespace SchedulerApi.Infrastructure;

public class AvailableSlotWithManagerCount
{
	public int Id { get; set; }
	public DateTime StartDate { get; set; }
	public DateTime EndDate { get; set; }
	public int AvailableManagerCount { get; set; }
}
