using AdvertisingPlatforms.Controllers;
using AdvertisingPlatforms.Services;
using Moq;
using System;

namespace AdvertisingPlatforms.Tests;

public class TestFixture : IDisposable
{
    public Mock<IAdvertisingService> AdvertisingServiceMock { get; }
    public AdvertisingController Controller { get; }

    public TestFixture()
    {
        AdvertisingServiceMock = new Mock<IAdvertisingService>();
        Controller = new AdvertisingController(AdvertisingServiceMock.Object);
    }

    public void Dispose()
    {
        
    }
}