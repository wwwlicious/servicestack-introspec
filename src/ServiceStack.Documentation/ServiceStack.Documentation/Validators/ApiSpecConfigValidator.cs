
namespace ServiceStack.Documentation.Validators
{
    using FluentValidation;
    using Settings;

    public class ApiSpecConfigValidator : AbstractValidator<ApiSpecConfig>
    {
        public ApiSpecConfigValidator()
        {
            RuleFor(c => c.Contact).NotNull().SetValidator(new ApiContactValidator());
            RuleFor(c => c.Description).NotEmpty();
        }
    }
}
