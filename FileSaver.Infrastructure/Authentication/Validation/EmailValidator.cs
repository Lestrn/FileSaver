using FluentValidation;

namespace FileSaver.Infrastructure.Authentication.Validation
{
    public class EmailValidator : AbstractValidator<string>
    {
        public EmailValidator()
        {
            RuleFor(email => email)
                .NotEmpty().WithMessage("Email address is required.")
                .EmailAddress().WithMessage("Invalid email address format.");
        }
    }
}
