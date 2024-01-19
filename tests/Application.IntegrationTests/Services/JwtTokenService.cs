using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ClaimTypes = MRA.Configurations.Common.Constants.ClaimTypes;

namespace Application.IntegrationTests.Services;

public class JwtTokenService : IDisposable
{
    private List<Claim> _commonClaims =
    [
        new Claim(ClaimTypes.Id, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.Email, "email@example.com"),
        new Claim(ClaimTypes.Username, "username@example.com")
    ];

    public string CreateTokenByClaims(string secret, List<Claim>? claims = default)
    {
        SymmetricSecurityKey key = new(Encoding.UTF8
            .GetBytes(secret));

        if (claims != null)
            _commonClaims.AddRange(claims);


        SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha512Signature);

        var expireDate = DateTime.Now.AddDays(2);

        JwtSecurityToken token = new(
            claims: _commonClaims,
            expires: expireDate,
            signingCredentials: credentials);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }

    public void Dispose()
    {
        _commonClaims =
        [
            new Claim(ClaimTypes.Id, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Email, "email@example.com"),
            new Claim(ClaimTypes.Username, "username@example.com")
        ];
    }
}