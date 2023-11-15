using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public record AuthorizationRoles([property: JsonPropertyName("roles")] string[]? Roles);

public record AppMetadata(
    [property: JsonPropertyName("authorization")] AuthorizationRoles? Authorization
);

public record JwtTokenPayload(
    [Required] [property: JsonPropertyName("sub")] string Sub,
    [Required] [property: JsonPropertyName("exp")] int Exp,
    [property: JsonPropertyName("app_metadata")] AppMetadata AppMetadata
);
