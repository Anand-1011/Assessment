namespace CountryGwp.Api.DTO;
public sealed record GwpAvgRequest
    (
    string Country,
    IReadOnlyList<string> Lob
    )
{
    public string NormalizedCountry => Country.Trim().ToLowerInvariant();
}
