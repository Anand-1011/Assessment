using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CountryGwp.Api.Domain;
using CountryGwp.Api.Repository.Storage;
using CountryGwp.Api.Services;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using NUnit.Framework;

namespace CountryGwp.Api.Tests.Services
{
    [TestFixture]
    public class GwpServiceTests
    {
        private Mock<IGwpRepository> _repoMock;
        private IMemoryCache _cache;
        private GwpService _service;

        [SetUp]
        public void Setup()
        {
            _repoMock = new Mock<IGwpRepository>();
            _cache = new MemoryCache(new MemoryCacheOptions());
            _service = new GwpService(_repoMock.Object, _cache);
        }


        [TearDown]
        public void TearDown()
        {
            _cache.Dispose();
        }

        [Test]
        public async Task GetAverageAsync_ReturnsCorrectAverages()
        {
            // Arrange
            var country = "france";
            var lobs = new[] { "motor", "fire" };

            var motorRecord = new GwpRecord
            {
                Country = "france",
                LineOfBusiness = "motor"
            };
            motorRecord.ValuesByYear.Add(2008, 10);
            motorRecord.ValuesByYear.Add(2009, 20);

            var fireRecord = new GwpRecord
            {
                Country = "france",
                LineOfBusiness = "fire"
            };
            fireRecord.ValuesByYear.Add(2008, 30);
            fireRecord.ValuesByYear.Add(2009, 50);

            var records = new List<GwpRecord> { motorRecord, fireRecord };

            _repoMock.Setup(r => r.GetByCountryAsync(country, It.IsAny<CancellationToken>()))
                .ReturnsAsync(records);

            // Act
            var result = await _service.GetAverageAsync(country, lobs, CancellationToken.None);

            // Assert
            Assert.That(result.ContainsKey("motor"));
            Assert.That(result.ContainsKey("fire"));
            Assert.That(result["motor"], Is.EqualTo(15.0m));
            Assert.That(result["fire"], Is.EqualTo(40.0m));
        }


        [Test]
        public async Task GetAverageAsync_ReturnsEmpty_WhenNoRecords()
        {
            // Arrange
            var country = "unknown";
            var lobs = new[] { "motor" };
            _repoMock.Setup(r => r.GetByCountryAsync(country, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<GwpRecord>());

            // Act
            var result = await _service.GetAverageAsync(country, lobs, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Empty);
        }
    }
}
