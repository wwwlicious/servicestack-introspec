namespace ServiceStack.Documentation.Tests.Validators
{
    using Documentation.Models;
    using Documentation.Validators;
    using FluentValidation.TestHelper;
    using Xunit;

    public class ApiContactValidatorTests
    {
        private readonly ApiContactValidator validator = new ApiContactValidator();

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Name_IsMandatory(string name)
        {
            validator.ShouldHaveValidationErrorFor(v => v.Name, new ApiContact { Name = name });
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Email_IsMandatory(string email)
        {
            validator.ShouldHaveValidationErrorFor(v => v.Email, new ApiContact { Email = email });
        }

        [Theory]
        [InlineData("test")]
        [InlineData("test@example")]
        [InlineData("test.example.com")]
        public void Email_MustBeValidEmail(string email)
        {
            validator.ShouldHaveValidationErrorFor(v => v.Email, new ApiContact { Email = email });
        }
    }
}
