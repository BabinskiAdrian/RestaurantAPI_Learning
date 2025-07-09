using FluentValidation;
using RestaurantAPI.Entities;
namespace RestaurantAPI.Models.Validators
{
    public class RestaurantQueryValidatior : AbstractValidator<RestaurantQuery>
    {
        private readonly int[] allowedPageSizes = new[] { 5, 10, 15};
        private readonly string[] allowedSortByColumnNames = { nameof(Restaurant.Name), nameof(Restaurant.Category), nameof(Restaurant.Description)};
        public RestaurantQueryValidatior()
        {
            RuleFor(r => r.SearchPhrase).MaximumLength(100);
            RuleFor(r => r.PageNumber).GreaterThanOrEqualTo(1);

            //własna walidacja (custom)
            RuleFor(r => r.PageSize).Custom((value, context) =>
            {
                if(!allowedPageSizes.Contains(value))
                {
                    context.AddFailure(
                        "PageSize", $"Page size must be one of the following values: {string.Join(", ", allowedPageSizes)}");
                }

            });

            RuleFor(r => r.SortBy)
                .Must(
                value => string.IsNullOrEmpty(value) || allowedSortByColumnNames.Contains(value))
                .WithMessage($"Sort by is optional or must be in [{string.Join(",", allowedSortByColumnNames)}]");
                
        }
    }
}
