namespace CountryGwp.Api.Services
{
    public interface IGwpServcie
    {
        Task<IDictionary<string, decimal>> GetAverageAsync(string country, IEnumerable<string> lobs, CancellationToken ct);
    }
}
