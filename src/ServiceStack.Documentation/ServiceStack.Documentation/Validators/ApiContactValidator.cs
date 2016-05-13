namespace ServiceStack.Documentation.Validators
{
    using FluentValidation;
    using Models;

    public class ApiContactValidator : AbstractValidator<ApiContact>
    {
        public ApiContactValidator()
        {
            RuleFor(c => c.Name).NotEmpty();
            RuleFor(c => c.Email).NotEmpty().EmailAddress();
        }
    }
}