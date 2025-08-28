using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CountryGwp.Api.Domain;
using CountryGwp.Api.Repository.Storage;
using Microsoft.AspNetCore.Hosting;
using Moq;
using NUnit.Framework;

namespace CountryGwp.Api.Tests.Repository.Storage
{
    [TestFixture]
    public class InMemoryGwpRepositoryTests
    {
        private string _testDataDir;

        [SetUp]
        public void Setup()
        {
            _testDataDir = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData");
            var dataDir = Path.Combine(_testDataDir, "Data");
            Directory.CreateDirectory(dataDir);

            var csv = "country,lineOfBusiness,Y2008,Y2009\nfrance,motor,10,20\nfrance,fire,30,50";
            File.WriteAllText(Path.Combine(dataDir, "gwpByCountry.csv"), csv);
        }


        [TearDown]
        public void TearDown()
        {
            Directory.Delete(_testDataDir, true);
        }

        [Test]
        public async Task GetByCountryAsync_ReturnsCorrectRecords()
        {
            // Arrange
            var envMock = new Mock<IWebHostEnvironment>();
            envMock.Setup(e => e.ContentRootPath).Returns(_testDataDir);

            var repo = new InMemoryGwpRepository(envMock.Object);

            // Act
            var records = await repo.GetByCountryAsync("france", CancellationToken.None);

            // Assert
            Assert.That(records, Has.Count.EqualTo(2));
            Assert.That(records[0].LineOfBusiness, Is.EqualTo("motor"));
            Assert.That(records[1].LineOfBusiness, Is.EqualTo("fire"));
        }
    }
}
