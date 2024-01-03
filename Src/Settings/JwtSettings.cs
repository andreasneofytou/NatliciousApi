public class JwtSettings
{
    public required string Secret { get; set; }
    public required string ValidIssuer { get; set; }
    public required string[] ValidAudiences { get; set; }
    public int AccessTokenExpiration { get; set; }
    public int RefreshTokenExpiration { get; set; }
}