using Dapper;
using Npgsql;
using SchedulerApi.Domain;

namespace SchedulerApi.Infrastructure
{
	public class ScheduleRepository(IConfiguration configuration) : IReadScheduleRepository
	{
		public async Task<IEnumerable<AvailableSlotWithManagerCount>> GetAvailableSlotsWithManagerCount(Language language, Rating[] searchForRatings, Product[] products, DateOnly date)
		{
			using var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection"));
			var query = @"
WITH filtered_managers AS (
    SELECT id
    FROM sales_managers
    WHERE @Language = ANY(languages)
    AND @Ratings::varchar[] && customer_ratings
    AND @Products::varchar[] && products
),
overlapping_slots AS (
    SELECT s1.id, s1.sales_manager_id
    FROM slots s1
    INNER JOIN slots s2 ON s1.sales_manager_id = s2.sales_manager_id
                        AND s1.id != s2.id
    WHERE tstzrange(s1.start_date, s1.end_date, '[]') && tstzrange(s2.start_date, s2.end_date, '[]') -- Use overlap
    AND s1.sales_manager_id IN (SELECT id FROM filtered_managers)
    AND s1.booked = false
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
    id,
    sales_manager_id AS SalesManagerId,
    start_date AS StartDate,
    end_date AS EndDate,
    COUNT(sales_manager_id) OVER (PARTITION BY start_date, end_date) AS AvailableManagerCount
FROM non_overlapping_slots;
";

			var parameters = new
			{
				Language = language.ToString(),
				Ratings = searchForRatings.Select(s => s.ToString()).ToArray(),
				Products = products.Select(s => s.ToString()).ToArray(),
				DayStart = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0),
				DayFinish = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59)
			};
			var result = await connection.QueryAsync<AvailableSlotWithManagerCount>(query, parameters);

			return result;
		}
	}

	public interface IReadScheduleRepository
	{
		Task<IEnumerable<AvailableSlotWithManagerCount>> GetAvailableSlotsWithManagerCount(Language language, Rating[] searchForRatings, Product[] products, DateOnly date);
	}

}