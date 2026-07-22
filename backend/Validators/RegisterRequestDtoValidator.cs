using FluentValidation;
using Backend.Models.Dto;

namespace Backend.Validators;

public class RegisterRequestDtoValidator : AbstractValidator<RegisterRequestDto>
{
    public RegisterRequestDtoValidator(IConfiguration config)
    {
        var registerConfig = config.GetSection("ValidationSettings:Register");

        int minUserLength = registerConfig.GetValue("MinUsernameLength", 3);
        int maxUserLength = registerConfig.GetValue("MaxUsernameLength", 30);
        int minPassLength = registerConfig.GetValue("MinPasswordLength", 8);
        int maxPassLength = registerConfig.GetValue("MaxPasswordLength", 64);

        RuleFor(dto => dto.Username)
            .NotEmpty().WithMessage("The username is required.")
            .MinimumLength(minUserLength).WithMessage($"The username has a minimum length of {minUserLength} characters.")
            .MaximumLength(maxUserLength).WithMessage($"The username has a maximum length of {maxUserLength} characters.");

        RuleFor(dto => dto.Password)
            .NotEmpty().WithMessage("A password is required.")
            .MinimumLength(minPassLength).WithMessage($"The password has a minimum length of {minPassLength} characters.")
            .MaximumLength(maxPassLength).WithMessage($"The password has a maximum length of {maxPassLength} characters.");
    }
}