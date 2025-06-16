using FluentValidation;
using RestaurantAPI.Entities;

namespace RestaurantAPI.Models.Validators
{
    public class CreateRestaurantDtoValidator : AbstractValidator<CreateRestaurantDto>
    {
        public CreateRestaurantDtoValidator(RestaurantDbContext dBContext)
        {
            RuleFor(x => x.Name)
                .MaximumLength(25)
                .NotEmpty();


            RuleFor(x => x.ContactEmail)
                .EmailAddress()
                .NotEmpty()
                .Custom((value, context) =>
                {
                    var emailInUse = dBContext.Users.Any(u => u.Email == value);
                    if (emailInUse)
                    {
                        context.AddFailure("Email", "That email is taken");
                    }

                });


            RuleFor(x => x.City)
                .MaximumLength(50)
                .NotEmpty();


            RuleFor(x => x.Street)
                .MaximumLength(50)
                .NotEmpty();
        }

    }
}
