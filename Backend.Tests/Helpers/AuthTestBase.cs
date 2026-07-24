using Backend.Models.Dto;

namespace Backend.Tests.Helper;

public abstract class AuthTestBase : TestSettingsBase
{
    protected RegisterRequestDto GetValidRegisterRequestDto()
    {
        return new RegisterRequestDto
        {
            Username = GenerateStringOfLength(MinUsernameLength),
            Password = GenerateStringOfLength(MinPasswordLength),
        };
    }
    protected LoginRequestDto GetValidLoginRequestDto()
    {
        return new LoginRequestDto
        {
            Username = GenerateStringOfLength(MinUsernameLength),
            Password = GenerateStringOfLength(MinPasswordLength),
        };
    }
}