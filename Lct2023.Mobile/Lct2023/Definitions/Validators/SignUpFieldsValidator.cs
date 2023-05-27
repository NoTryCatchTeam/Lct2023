using System.Text.RegularExpressions;
using FluentValidation;
using Lct2023.ViewModels.Auth;

namespace Lct2023.Definitions.Validators;

public class SignUpFieldsValidator : AbstractValidator<SignUpFields>
{
    public SignUpFieldsValidator()
    {
        RuleFor(x => x.Email)
            .Must(x => !string.IsNullOrEmpty(x)).WithMessage("Введите email")
            .EmailAddress().WithMessage("Неверный email");

        RuleFor(x => x.Password)
            .Must(x => !string.IsNullOrEmpty(x)).WithMessage("Введите пароль")
            .MinimumLength(6).WithMessage("Длина пароля должна быть от 6 символов")
            .Matches(new Regex("[a-zA-Z0-9]*")).WithMessage("Допустимые символы a-zA-Z0-9");

        RuleFor(x => x.RepeatPassword)
            .Equal(x => x.Password).WithMessage("Пароли не совпадают");

        RuleFor(x => x.Name)
            .Must(x => !string.IsNullOrEmpty(x)).WithMessage("Введите имя");

        RuleFor(x => x.Surname)
            .Must(x => !string.IsNullOrEmpty(x)).WithMessage("Введите фамилию");
    }
}
