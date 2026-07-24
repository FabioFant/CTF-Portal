using FluentValidation.TestHelper;
using Backend.Validators;
using Backend.Tests.Helper;

namespace Backend.Tests.Validators;

public class LoginRequestDtoValidatorTests : AuthTestBase
{
    private readonly LoginRequestDtoValidator _validator;
    public LoginRequestDtoValidatorTests()
    {
        _validator = new LoginRequestDtoValidator();
    }

    [Fact]
    public void Validate_ValidDto_NoError()
    {
        // Arrange
        var dto = GetValidLoginRequestDto();

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    } 

    [Fact]
    public void Validate_EmptyUsername_Error()
    {
        // Arrange
        var dto = GetValidLoginRequestDto();
        dto.Username = "";

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Username);
    }

    [Fact]
    public void Validate_EmptyPassword_Error()
    {
        // Arrange
        var dto = GetValidLoginRequestDto();
        dto.Password = "";

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }
}