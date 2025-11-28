//NetCoreAPI-main/Services/Validadores/UsuarioValidador.cs

using Core.Entidades;
using FluentValidation;

namespace Services.Validadores
{
    public class UsuarioValidador : AbstractValidator<Usuario>
    {
        public UsuarioValidador()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
            RuleFor(x => x.Lastname).NotEmpty().MaximumLength(255);
            RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(255);
            RuleFor(x => x.Password).NotEmpty().MinimumLength(6).MaximumLength(100);
            RuleFor(x => x.Status).NotEmpty().MaximumLength(50);
        }
    }
}
