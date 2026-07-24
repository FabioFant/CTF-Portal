using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Backend.Controllers;
using Backend.Models.Dto;
using Backend.Data;

namespace Backend.Tests.Helper;

public abstract class TestSettingsBase
{
    protected readonly int MinUsernameLength = 2;
    protected readonly int MaxUsernameLength = 4;
    protected readonly int MinPasswordLength = 2;
    protected readonly int MaxPasswordLength = 4;

    protected readonly string JwtIssuer = "TestIssuer";
    protected readonly string JwtAudience = "TestAudience";
    protected readonly string JwtKey = "eowuxZagltF68p9HdUqF2w4egNZ8AZtmsMdwxWiSZy4";

    protected readonly IConfiguration MockConfig;

    protected TestSettingsBase()
    {
        var inMemorySettings = new Dictionary<string, string> {
            {"ValidationSettings:Register:MinUsernameLength", MinUsernameLength.ToString()},
            {"ValidationSettings:Register:MaxUsernameLength", MaxUsernameLength.ToString()},
            {"ValidationSettings:Register:MinPasswordLength", MinPasswordLength.ToString()},
            {"ValidationSettings:Register:MaxPasswordLength", MaxPasswordLength.ToString()},

            {"Jwt:Issuer", JwtIssuer},
            {"Jwt:Audience", JwtAudience},
            {"Jwt:Key", JwtKey}
        };
        
        MockConfig = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();
    }
    protected BackendContext GetContext()
    {
        var options = new DbContextOptionsBuilder<BackendContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new BackendContext(options); 
    }
    protected string GenerateStringOfLength(int length)
    {
        return new string('a', length);
    }
}