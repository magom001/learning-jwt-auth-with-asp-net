using System.ComponentModel.DataAnnotations;

public record AuthorizationRoles(string[]? Roles);

public record AppMetadata(AuthorizationRoles? Authorization);

public record JwtTokenPayload([Required] string sub, [Required] int exp, AppMetadata? AppMetadata);
