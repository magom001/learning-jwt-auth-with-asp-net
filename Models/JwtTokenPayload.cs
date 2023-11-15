using System.ComponentModel.DataAnnotations;

public record JwtTokenPayload([Required] string sub, [Required] int exp);
