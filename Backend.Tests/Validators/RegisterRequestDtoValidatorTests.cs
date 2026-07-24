using FluentValidation.TestHelper;
using Backend.Validators;
using Backend.Tests.Helper;

namespace Backend.Tests.Validators;

public class RegisterRequestDtoValidatorTests : AuthTestBase
{
    private readonly RegisterRequestDtoValidator _validator;
    public RegisterRequestDtoValidatorTests()
    {
        _validator = new RegisterRequestDtoValidator(MockConfig);
    }

    [Fact]
    public void Validate_ValidDto_NoError()
    {
        // Arrange
        var dto = GetValidRegisterRequestDto();

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    #region Username
    [Fact]
    public void Validate_EmptyUsername_Error()
    {
        // Arrange
        var dto = GetValidRegisterRequestDto();
        dto.Username = "";

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Username);
    }

    [Fact]
    public void Validate_TooShortUsername_Error()
    {
        // Arrange
        var dto = GetValidRegisterRequestDto();
        dto.Username = GenerateStringOfLength(MinUsernameLength - 1);

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Username);
    }

    [Fact]
    public void Validate_TooLongUsername_Error()
    {
        // Arrange
        var dto = GetValidRegisterRequestDto();
        dto.Username = GenerateStringOfLength(MaxUsernameLength + 1);

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Username);
    }
    #endregion

    #region Password
    [Fact]
    public void Validate_EmptyPassword_Error()
    {
        // Arrange
        var dto = GetValidRegisterRequestDto();
        dto.Password = "";

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Validate_TooShortPassword_Error()
    {
        // Arrange
        var dto = GetValidRegisterRequestDto();
        dto.Password = GenerateStringOfLength(MinPasswordLength - 1);

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Validate_TooLongPassword_Error()
    {
        // Arrange
        var dto = GetValidRegisterRequestDto();
        dto.Password = GenerateStringOfLength(MaxPasswordLength + 1);

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }
    #endregion
}