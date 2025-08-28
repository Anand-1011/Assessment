using CountryGwp.Api.Domain;

namespace CountryGwp.Api.Repository.Storage
{
    public interface IGwpRepository
    {
        Task<IReadOnlyList<GwpRecord>> GetByCountryAsync(string country, CancellationToken ct);
    }
}
