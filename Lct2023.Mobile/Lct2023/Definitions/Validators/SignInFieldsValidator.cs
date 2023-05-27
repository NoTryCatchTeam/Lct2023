using FluentValidation;
using Lct2023.ViewModels.Auth;

namespace Lct2023.Definitions.Validators;

public class SignInFieldsValidator : AbstractValidator<SignInFields>
{
    public SignInFieldsValidator()
    {
        RuleFor(x => x.Email)
            .Must(x => !string.IsNullOrEmpty(x)).WithMessage("Введите email")
            .EmailAddress().WithMessage("Неверный email");

        RuleFor(x => x.Password)
            .Must(x => !string.IsNullOrEmpty(x)).WithMessage("Введите пароль");
    }
}
