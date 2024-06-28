using Dapper;
using Npgsql;
using Polly;
using SchedulerApi.Domain;

namespace SchedulerApi.Infrastructure
{
	public class ScheduleRepository(IConfiguration configuration) : IReadScheduleRepository
	{
		// The query parts are not intended to be reusable.
		// If needed, we can easily extract manager filtering for caching.
		public async Task<IEnumerable<AvailableBookings>> GetAvailableSlotsWithManagerCount(Language language, Rating[] searchForRatings, Product[] products, DateOnly date)
		{
			using var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection"));
			var query = @"
WITH filtered_managers AS (
    SELECT id
    FROM sales_managers
    WHERE @Language = ANY(languages)
    AND @Ratings::varchar[] && customer_ratings -- any rating from allowed ones is fine
    AND products @> @Products::varchar[] -- manager shout have all products listed
),
overlapping_slots AS (
SELECT s1.id, s1.sales_manager_id, s1.start_date, s1.end_date, s1.booked
FROM slots s1
INNER JOIN slots s2 ON s1.sales_manager_id = s2.sales_manager_id
                    AND s1.id != s2.id
WHERE tstzrange(s1.start_date, s1.end_date, '()') && tstzrange(s2.start_date, s2.end_date, '()')
  AND s1.sales_manager_id IN (SELECT id FROM filtered_managers)
  AND s2.booked = true
    AND tstzrange(s1.start_date, s1.end_date, '[]') && tstzrange(@DayStart, @DayFinish, '[]') -- Overlap with @DayStart to @DayFinish range
),
non_overlapping_slots AS (
    SELECT id, sales_manager_id, start_date, end_date
    FROM slots
    WHERE sales_manager_id IN (SELECT id FROM filtered_managers)
    AND booked = false
    AND tstzrange(start_date, end_date, '[]') && tstzrange(@DayStart, @DayFinish, '[]') -- Filter by @DayStart to @DayFinish range
    AND id NOT IN (SELECT id FROM overlapping_slots)
)
SELECT
    start_date AS StartDate,
    end_date AS EndDate,
    COUNT(sales_manager_id) AS AvailableManagerCount
FROM non_overlapping_slots
GROUP BY(start_date, end_date);
";

			var parameters = new
			{
				Language = language.ToString(),
				Ratings = searchForRatings.Select(s => s.ToString()).ToArray(),
				Products = products.Select(s => s.ToString()).ToArray(),
				DayStart = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0),
				DayFinish = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59)
			};

			var retryPolicy = Policy
				.Handle<NpgsqlException>(ex => ex.IsTransient) // Handle transient PostgreSQL exceptions. Depending on the DB setup, this might be detrimental.
				.WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

			var result = await retryPolicy.ExecuteAsync(() => connection.QueryAsync<AvailableSlotWithManagerCount>(query, parameters));

			return result.Select(r => new AvailableBookings(
				r.StartDate, r.EndDate, r.AvailableManagerCount));
		}
	}

	public interface IReadScheduleRepository
	{
		Task<IEnumerable<AvailableBookings>> GetAvailableSlotsWithManagerCount(Language language, Rating[] searchForRatings, Product[] products, DateOnly date);
	}
}