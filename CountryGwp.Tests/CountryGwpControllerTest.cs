using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CountryGwp.Api.Controllers;
using CountryGwp.Api.DTO;
using CountryGwp.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace Galytix_Assesment.Tests.Controllers
{
    [TestFixture]
    public class CountryGwpControllerTests
    {
        private Mock<IGwpServcie> _serviceMock;
        private CountryGwpController _controller;

        [SetUp]
        public void SetUp()
        {
            _serviceMock = new Mock<IGwpServcie>();
            _controller = new CountryGwpController(_serviceMock.Object);
        }

        [Test]
        public async Task GetAvg_ReturnsOk_WhenDataExists()
        {
            // Arrange
            var request = new GwpAvgRequest("france", new List<string> { "motor" });
            var expectedData = new Dictionary<string, decimal> { { "motor", 123.4m } };
            _serviceMock.Setup(s => s.GetAverageAsync("france", request.Lob, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(expectedData);

            // Act
            var result = await _controller.GetAvg(request, CancellationToken.None);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(expectedData, okResult.Value);
        }

        [Test]
        public async Task GetAvg_ReturnsBadRequest_WhenCountryOrLobMissing()
        {
            // Arrange
            var request = new GwpAvgRequest("", new List<string>());

            // Act
            var result = await _controller.GetAvg(request, CancellationToken.None);

            // Assert
            var badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
            Assert.That(badRequest.Value.ToString(), Does.Contain("country and lob are required"));
        }

        [Test]
        public async Task GetAvg_ReturnsNotFound_WhenNoData()
        {
            // Arrange
            var request = new GwpAvgRequest("unknown", new List<string> { "motor" });
            _serviceMock.Setup(s => s.GetAverageAsync("unknown", request.Lob, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new Dictionary<string, decimal>());

            // Act
            var result = await _controller.GetAvg(request, CancellationToken.None);

            // Assert
            var notFound = result as NotFoundObjectResult;
            Assert.IsNotNull(notFound);
            Assert.That(notFound.Value.ToString(), Does.Contain("no data for given country/lob"));
        }
    }
}
