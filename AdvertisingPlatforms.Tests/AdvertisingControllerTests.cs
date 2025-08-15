using AdvertisingPlatforms.Controllers;
using AdvertisingPlatforms.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AdvertisingPlatforms.Tests
{
    public partial class AdvertisingControllerTests : IClassFixture<TestFixture>
    {
        private readonly AdvertisingController _controller;
        private readonly Mock<IAdvertisingService> _serviceMock;

        public AdvertisingControllerTests(TestFixture fixture)
        {
            _controller = fixture.Controller;
            _serviceMock = fixture.AdvertisingServiceMock;
        }

        [Fact]
        public async Task UploadFile_ValidFile_ReturnsOk()
        {
            // Arrange
            var content = "Яндекс.Директ:/ru";
            var bytes = Encoding.UTF8.GetBytes(content);
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(bytes));
            fileMock.Setup(f => f.Length).Returns(bytes.Length);

            // Act
            var result = await _controller.UploadFile(fileMock.Object) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal("Данные успешно загружены.", result.Value);
            _serviceMock.Verify(s => s.LoadPlatformsFromFileAsync(fileMock.Object), Times.Once);
        }

        [Fact]
        public async Task UploadFile_EmptyFile_ReturnsBadRequest()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(0);
            _serviceMock.Setup(s => s.LoadPlatformsFromFileAsync(fileMock.Object)).ThrowsAsync(new ArgumentException("Файл не предоставлен или пуст."));

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _controller.UploadFile(fileMock.Object));
        }

        [Fact]
        public void GetPlatforms_ValidLocation_ReturnsOkWithPlatforms()
        {
            // Arrange
            _serviceMock.Setup(s => s.GetPlatformsForLocation("/ru")).Returns(new[] { "Яндекс.Директ" });

            // Act
            var result = _controller.GetPlatforms("/ru") as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            var platforms = Assert.IsAssignableFrom<IEnumerable<string>>(result.Value);
            Assert.Single(platforms);
            Assert.Contains("Яндекс.Директ", platforms);
        }

        [Fact]
        public void GetPlatforms_InvalidLocation_ReturnsBadRequest()
        {
            // Arrange
            _serviceMock.Setup(s => s.GetPlatformsForLocation("invalid")).Throws(new ArgumentException("Некорректная локация."));

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _controller.GetPlatforms("invalid"));
        }
    }
}