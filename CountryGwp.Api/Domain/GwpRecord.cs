namespace CountryGwp.Api.Domain;
public sealed class GwpRecord
{
    public required string Country { get; init; }
    public required string LineOfBusiness { get; init; }
    public Dictionary<int, decimal> ValuesByYear { get; } = new();
}
