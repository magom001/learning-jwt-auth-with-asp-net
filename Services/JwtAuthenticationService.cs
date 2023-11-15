using System.Security.Cryptography;
using System.Text.Json;
using JWT.Algorithms;
using JWT.Builder;

class JwtAuthenticationService : IJwtAuthenticationService
{
    private const string RS1024_PRIVATE_KEY =
        @"-----BEGIN RSA PRIVATE KEY-----
MIICXQIBAAKBgQCNUls0UJhSPCd/cGldwAqVirYtSRLtc5Qrno7YsmpA+JnZuTQo
cFyT8GFbyFdQORjBUz9kCVTBsVk/Mjn9a5tRqc+2IPXWyTm+7YgwoxCJrlNqzN5t
bPMgJQf0QemqyQNJEU2jgNPYNLEMJ+1Q747+zVTv1bi7okUScCzYPaf1uwIDAQAB
AoGBAINiDWiVhQbu5cmUuGBwKWbdjoCLbw1SZm4m+qZ7OE0u0dmYVOVDkM34rIqn
toTekCUrP8PA6Qsp1c7q4v63C61+hOvUsxbUR5U5PFXQU2dOw7/mxNJTaoYBIvG6
lW6ZwSPgUd7QOu1GfnbYfL4tigYR8nf7b7/WMLLX+TEctwABAkEAv6atTWG3dnfW
fRWC5mn6C8InpzQY5C1EFpt8xeTs50r8B6iMUdrlTSa23Xj3mD/1zTA7FAV4SMxL
DnUWVojHuwJBALzFoTB9nsEmkuxXL9XBN9Pdkgb9c6AuW5qfE5h+HWof1x2WTeW4
sL9KE0/igX/hGqGE7uqtGRsmdIWQ/x8nqgECQDxa4lLvRhax8MNdpeaoU02mrFQ0
zO32721rNCUiThUdATfsNZyFohbk7UvcD6VL5z3iRYitnE7Yv35jE1DXLIcCQQCs
pce1lL60gvYPJ/J8+ll38QbUU8wDbUKkmOcQKg/29qYEzmnyN0eXvEULY+ryrUtw
/CaTBbuXhEU/v4xFzz4BAkAd58bkM3j1VFQrrYG/9ak3vrXzvzlIxZXBr7oFvj8n
ip0AYR52GrhzKyAKJq+SReBylh9YEu7kyANU9ZlcwE2+
-----END RSA PRIVATE KEY-----";

    private const string RS1024_PUBLIC_KEY =
        @"-----BEGIN PUBLIC KEY-----
MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCNUls0UJhSPCd/cGldwAqVirYt
SRLtc5Qrno7YsmpA+JnZuTQocFyT8GFbyFdQORjBUz9kCVTBsVk/Mjn9a5tRqc+2
IPXWyTm+7YgwoxCJrlNqzN5tbPMgJQf0QemqyQNJEU2jgNPYNLEMJ+1Q747+zVTv
1bi7okUScCzYPaf1uwIDAQAB
-----END PUBLIC KEY-----";

    private const int JWT_TOKEN_VALIDITY_IN_HOURS = 1;
    private readonly ILogger<JwtAuthenticationService> logger;
    private readonly IUserService userService;
    private readonly RS1024Algorithm algorithm;

    public JwtAuthenticationService(
        IUserService userService,
        ILogger<JwtAuthenticationService> logger
    )
    {
        this.logger = logger;
        this.userService = userService;

        var privateKey = RSA.Create();
        privateKey.ImportFromPem(RS1024_PRIVATE_KEY.ToCharArray());
        var publicKey = RSA.Create();
        publicKey.ImportFromPem(RS1024_PUBLIC_KEY.ToCharArray());

        this.algorithm = new RS1024Algorithm(publicKey, privateKey);
    }

    public string? AuthenticateByUsernameAndPassword(string username, string password)
    {
        var user = this.userService.FindUserByUsernameAndPassword(username, password);

        if (user is null)
        {
            return null;
        }

        var token = JwtBuilder
            .Create()
            .WithAlgorithm(this.algorithm)
            .AddClaim(
                "exp",
                DateTimeOffset.UtcNow.AddHours(JWT_TOKEN_VALIDITY_IN_HOURS).ToUnixTimeSeconds()
            )
            .AddClaim("sub", username)
            .Encode();

        return token;
    }

    public JwtTokenPayload? DecryptJwtToken(string token)
    {
        try
        {
            var jsonString = JwtBuilder
                .Create()
                .WithAlgorithm(this.algorithm)
                .MustVerifySignature()
                .Decode(token);

            return JsonSerializer.Deserialize<JwtTokenPayload>(jsonString);
        }
        catch (Exception e)
        {
            this.logger.LogError("Failed to decrypt JWT token. Exception: {e}", e);

            return null;
        }
    }
}
