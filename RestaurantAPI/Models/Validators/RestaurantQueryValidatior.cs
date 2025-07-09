using FluentValidation;

namespace RestaurantAPI.Models.Validators
{
    public class RestaurantQueryValidatior : AbstractValidator<RestaurantQuery>
    {
        private readonly int[] allowedPageSizes = new[] { 5, 10, 15};

        public RestaurantQueryValidatior()
        {
            RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);

            //własna walidacja (custom)
            RuleFor(x => x.PageSize).Custom((value, context) =>
            {
                if(!allowedPageSizes.Contains(value))
                {
                    context.AddFailure(
                        "PageSize", $"Page size must be one of the following values: {string.Join(", ", allowedPageSizes)}");
                }

            });

            RuleFor(x => x.SearchPhrase).MaximumLength(100);
        }
    }
}
