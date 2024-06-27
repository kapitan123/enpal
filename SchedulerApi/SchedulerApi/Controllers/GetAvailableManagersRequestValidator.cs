using FluentValidation;
using SchedulerApi.Controllers;
using SchedulerApi.Domain;

public class GetAvailableManagersRequestValidator : AbstractValidator<GetAvailableManagersRequest>
{
	public GetAvailableManagersRequestValidator()
	{
		RuleFor(x => x.Date)
			.NotEmpty().WithMessage("Date is required");

		RuleFor(x => x.Products)
			.NotEmpty().WithMessage("Products are required")
			.Must(products => products.All(p => Enum.IsDefined(typeof(Product), p)))
				.WithMessage("Products must be either 'SolarPanels' or 'Heatpumps'");

		RuleFor(x => x.Language)
			.NotEmpty().WithMessage("Language is required")
			.Must(l => Enum.IsDefined(typeof(Language), l))
				.WithMessage("Invalid language");

		RuleFor(x => x.Rating)
			.NotEmpty().WithMessage("Rating is required")
			.Must(r => Enum.IsDefined(typeof(Rating), r))
				.WithMessage("Invalid rating");
	}
}