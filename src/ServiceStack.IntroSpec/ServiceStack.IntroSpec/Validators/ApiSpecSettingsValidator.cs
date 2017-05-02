// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Validators
{
    using FluentValidation;

    public class ApiSpecSettingsValidator : AbstractValidator<IApiSpecSettings>
    {
        public ApiSpecSettingsValidator()
        {
            RuleFor(c => c.ContactName).NotEmpty();
            RuleFor(c => c.ContactEmail).NotEmpty().EmailAddress();
            RuleFor(c => c.Description).NotEmpty();
        }
    }
}