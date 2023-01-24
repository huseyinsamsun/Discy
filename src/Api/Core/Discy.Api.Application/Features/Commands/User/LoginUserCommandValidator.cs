using Discy.Common.ViewModels.RequestModels;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discy.Api.Application.Features.Commands.User
{
    public class LoginUserCommandValidator:AbstractValidator<LoginUserCommand>
    {
        public LoginUserCommandValidator()
        {
            RuleFor(i => i.EmailAddress).NotNull().EmailAddress(FluentValidation.Validators.EmailValidationMode.AspNetCoreCompatible).WithMessage("{PromertyName} not a valid email address");
            RuleFor(i => i.Password).NotNull().MinimumLength(6).WithMessage("{ProperyName} should at least be {MinLength} characters");

        }
    }
}
