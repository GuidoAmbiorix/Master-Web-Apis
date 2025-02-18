using System.Data;
using FluentValidation;

namespace MasterNet.Application.Accounts.Register;

public class RegisterValidator : AbstractValidator<RegisterRequest>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
        RuleFor(x => x.NombreCompleto).NotEmpty();
        RuleFor(x => x.Carrera).NotEmpty();
        RuleFor(x => x.UserName).NotEmpty();
    }
}