using SchedulerApi.Infrastructure;

namespace SchedulerApi.Domain;

public class AvalableBookingsService(IReadScheduleRepository repo) : IAvalableBookingsService
{
	// AK TODO decouple
	public async Task<IEnumerable<AvailableSlotWithManagerCount>> GetAvailableSlotsWithManagerCountAsync(Language language, Rating rating, Product[] products, DateOnly date)
	{
		var accessableRatings = GetSearchableRatings(rating);
		var result = await repo.GetAvailableSlotsWithManagerCount(language, accessableRatings, products, date);

		return result;
	}

	private static Rating[] GetSearchableRatings(Rating rating)
	{
		var ratingsStack = new Stack<Rating>();

		ratingsStack.Push(Rating.Bronze);
		ratingsStack.Push(Rating.Silver);
		ratingsStack.Push(Rating.Golden);

		// Pop elements until we find the matching rating
		while (ratingsStack.Count > 0 && ratingsStack.Peek() != rating)
		{
			ratingsStack.Pop();
		}

		// Return the rest of the elements in the stack
		return ratingsStack.ToArray();
	}
}

public interface IAvalableBookingsService
{
	public Task<IEnumerable<AvailableSlotWithManagerCount>> GetAvailableSlotsWithManagerCountAsync(Language language, Rating rating, Product[] products, DateOnly date);
}