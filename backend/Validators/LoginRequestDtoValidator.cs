using FluentValidation;
using Backend.Models.Dto;

namespace Backend.Validators;

public class LoginRequestDtoValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestDtoValidator()
    {
        RuleFor(dto => dto.Username)
            .NotEmpty().WithMessage("The username is required.");

        RuleFor(dto => dto.Password)
            .NotEmpty().WithMessage("A password is required.");
    }
}