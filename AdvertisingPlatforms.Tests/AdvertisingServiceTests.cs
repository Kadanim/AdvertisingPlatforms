using AdvertisingPlatforms.Models;
using AdvertisingPlatforms.Services;
using Microsoft.AspNetCore.Http;
using Moq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AdvertisingPlatforms.Tests
{
    public class AdvertisingServiceTests
    {
        [Fact]
        public async Task LoadPlatformsFromFileAsync_ValidFile_BuildsTree()
        {
            // Arrange
            var service = new AdvertisingService();
            var content = "Яндекс.Директ:/ru\r\nGoogle:/ru/svrd"; 
            var bytes = Encoding.UTF8.GetBytes(content);
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(bytes));
            fileMock.Setup(f => f.Length).Returns(bytes.Length);

            // Act
            await service.LoadPlatformsFromFileAsync(fileMock.Object);

            // Assert
            var root = LocationTree.Root;
            Assert.True(root.Children.ContainsKey("ru"), "Ключ 'ru' отсутствует в Children.");
            Assert.Contains("Яндекс.Директ", root.Children["ru"].Platforms);
            Assert.True(root.Children["ru"].Children.ContainsKey("svrd"), "Ключ 'svrd' отсутствует в Children.");
            Assert.Contains("Google", root.Children["ru"].Children["svrd"].Platforms);
        }

        [Fact]
        public async Task LoadPlatformsFromFileAsync_EmptyFile_ThrowsArgumentException()
        {
            // Arrange
            var service = new AdvertisingService();
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(0);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.LoadPlatformsFromFileAsync(fileMock.Object));
        }

        [Fact]
        public void GetPlatformsForLocation_ValidLocation_ReturnsPlatforms()
        {
            // Arrange
            var service = new AdvertisingService();
            var root = new Node();
            var ruNode = new Node();
            var svrdNode = new Node();
            root.Children.Add("ru", ruNode);
            ruNode.Children.Add("svrd", svrdNode);
            ruNode.AddPlatform("Яндекс.Директ");
            svrdNode.AddPlatform("Google");
            LocationTree.Root = root;

            // Act
            var platforms = service.GetPlatformsForLocation("/ru/svrd");

            // Assert
            Assert.Equal(2, platforms.Count());
            Assert.Contains("Яндекс.Директ", platforms);
            Assert.Contains("Google", platforms);
        }

        [Fact]
        public void GetPlatformsForLocation_InvalidLocation_ThrowsArgumentException()
        {
            // Arrange
            var service = new AdvertisingService();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => service.GetPlatformsForLocation("invalid"));
        }
    }
}