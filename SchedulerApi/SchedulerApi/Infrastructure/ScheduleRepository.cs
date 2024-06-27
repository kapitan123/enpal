using Dapper;
using Npgsql;
using SchedulerApi.Domain;

namespace SchedulerApi.Infrastructure
{
	public class ScheduleRepository : IReadScheduleRepository
	{
		private readonly IConfiguration _configuration;

		public async Task<IEnumerable<AvailableSlotWithManagerCount>> GetAvailableSlotsWithManagerCount(Language language, Rating[] searchForRatings, Product[] products, DateOnly date)
		{
			using var connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

			var query = @"
WITH filtered_managers AS (
    SELECT id
    FROM sales_managers
    WHERE @Language = ANY(languages)
    AND @Ratings && customer_ratings
    AND @Products && products
),
overlapping_slots AS (
    SELECT s1.id, s1.sales_manager_id
    FROM slots s1
    INNER JOIN slots s2
    ON s1.sales_manager_id = s2.sales_manager_id
    AND s1.id != s2.id
    AND tsrange(s1.start_date, s1.end_date, '[]') && tsrange(s2.start_date, s2.end_date, '[]') -- Use overlap
    WHERE s1.sales_manager_id IN (SELECT id FROM filtered_managers) -- We assume the number of managers isn't that big so we don't create a temp table
    AND s1.booked = false
    AND tsrange(s1.start_date, s1.end_date, '[]') && tsrange(@Date, @Date + INTERVAL '1 day') -- Overlap with @Date range
),
non_overlapping_slots AS (
    SELECT id, sales_manager_id, start_date, end_date
    FROM slots
    WHERE sales_manager_id IN (SELECT id FROM filtered_managers)
    AND booked = false
    AND start_date >= @Date
    AND start_date < @Date + INTERVAL '1 day' -- Filter by date range
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
				Ratings = searchForRatings.Select(s => s.ToString()),
				Products = products.Select(s => s.ToString()),
				Date = date.ToString("yyyy-MM-dd")
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