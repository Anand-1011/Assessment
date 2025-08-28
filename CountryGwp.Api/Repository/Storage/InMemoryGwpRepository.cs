using CsvHelper;
using CsvHelper.Configuration;
using CountryGwp.Api.Domain;
using System.Globalization;

namespace CountryGwp.Api.Repository.Storage
{
    public class InMemoryGwpRepository :IGwpRepository
    {
        private readonly Dictionary<string, List<GwpRecord>> _byCountry = new();

        public InMemoryGwpRepository(IWebHostEnvironment env)
        {
            var path = Path.Combine(env.ContentRootPath, "Data", "gwpByCountry.csv");
            using var reader = new StreamReader(path);
            var cfg = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                PrepareHeaderForMatch  = args => args.Header?.Trim(),
                MissingFieldFound = null,
                BadDataFound = null
            };
            using var csv = new CsvReader(reader, cfg);
            var rows = csv.GetRecords<dynamic>();
            foreach (var row in rows)
            {
                var dict = (IDictionary<string, object>)row;
                var country = (dict["country"]?.ToString() ?? string.Empty).Trim().ToLowerInvariant();
                var lob = (dict.ContainsKey("lineOfBusiness") ? dict["lineOfBusiness"]?.ToString(): null) ?? string.Empty;
                var record = new GwpRecord { Country = country,LineOfBusiness= lob};

                foreach (var kv in dict)
                {
                    if(kv.Key.StartsWith("Y",StringComparison.OrdinalIgnoreCase))
                    {
                        if (int.TryParse(kv.Key[1..], out var year) && kv.Value is not null)
                        {
                            if(decimal.TryParse(kv.Value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture,out var val))
                            {
                                record.ValuesByYear[year]  = val;
                            }
                        }
                    }
                }

                if(!_byCountry.TryGetValue(country , out var list))
                {
                    list = new List<GwpRecord>();
                    _byCountry[country] = list;
                }
                list.Add(record);
            }
        }

        public Task<IReadOnlyList<GwpRecord>> GetByCountryAsync(string country,CancellationToken ct)
        {
            _byCountry.TryGetValue(country.Trim().ToLowerInvariant(), out var list);
            return Task.FromResult<IReadOnlyList<GwpRecord>>(list?? new List<GwpRecord>());
        }
    }

}
