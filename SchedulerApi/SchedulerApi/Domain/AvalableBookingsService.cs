using SchedulerApi.Infrastructure;

namespace SchedulerApi.Domain;

public class AvalableBookingsService(IReadScheduleRepository repo) : IAvalableBookingsService
{
	public async Task<IEnumerable<AvailableSlotWithManagerCount>> GetAvailableSlotsWithManagerCountAsync(Language language, Rating rating, Product[] products, DateOnly date)
	{
		var accessableRatings = GetSearchableRatings(rating);
		var result = await repo.GetAvailableSlotsWithManagerCount(language, accessableRatings, products, date);

		return result;
	}

	// Might as well be a separate domain object but weare being consistent with anemic model here
	private static Rating[] GetSearchableRatings(Rating rating)
	{
		var ratingsStack = new Stack<Rating>();

		ratingsStack.Push(Rating.Bronze);
		ratingsStack.Push(Rating.Silver);
		ratingsStack.Push(Rating.Gold);

		while (ratingsStack.Count > 0 && ratingsStack.Peek() != rating)
		{
			ratingsStack.Pop();
		}

		return [.. ratingsStack];
	}
}

public interface IAvalableBookingsService
{
	public Task<IEnumerable<AvailableSlotWithManagerCount>> GetAvailableSlotsWithManagerCountAsync(Language language, Rating rating, Product[] products, DateOnly date);
}