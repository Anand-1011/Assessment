using CountryGwp.Api.Repository.Storage;
using Microsoft.Extensions.Caching.Memory;

namespace CountryGwp.Api.Services
{
    public class GwpService : IGwpServcie
    {
        private static readonly int[] Years = Enumerable.Range(2008, 8).ToArray();
        private readonly IGwpRepository _repo;
        private readonly IMemoryCache _cache;

        public GwpService(IGwpRepository repo, IMemoryCache cache)
        {
            _repo = repo;
            _cache = cache;
        }

        public async Task<IDictionary<string, decimal>> GetAverageAsync(string country, IEnumerable<string> lobs, CancellationToken ct)
        {
            var lobList = lobs.Select(s => s.Trim()).Where(s => s.Length > 0).ToArray();
            var cacheKey = $"avg:{country.ToLowerInvariant()}:{string.Join('|', lobList.OrderBy(x => x.ToLowerInvariant()))}";

            if (_cache.TryGetValue(cacheKey, out IDictionary<string, decimal> cached))
            {
                return cached;
            }

            var records = await _repo.GetByCountryAsync(country, ct);
            var byLob = records.GroupBy(r=>r.LineOfBusiness.Trim().ToLowerInvariant())
                .ToDictionary(g=>g.Key,g=>g.ToList());

            var result = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
            foreach (var lob in lobList)
            {
                if (!byLob.TryGetValue(lob.Trim().ToLowerInvariant(), out var list))
                {
                    continue; // unkown lob omitted from result
                }

                //merge multiple rows for same lob present
                var allValues = new List<decimal>();
                foreach (var r in list)
                {
                    foreach (var y in Years)
                    {
                        if (r.ValuesByYear.TryGetValue(y, out var v))
                        {
                            allValues.Add(v);
                        }
                    }
                }
                if (allValues.Count > 0)
                {
                    result[lob] = Math.Round(allValues.Average(), 1);
                }
            }

            //cache for 5 minutes
            _cache.Set(cacheKey, result,TimeSpan.FromMinutes(5));
            return result;
        }
    }
}
