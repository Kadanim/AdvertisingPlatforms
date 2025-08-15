using AdvertisingPlatforms.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdvertisingPlatforms.Services
{
    public interface IAdvertisingService
    {
        Task LoadPlatformsFromFileAsync(IFormFile file);
        IEnumerable<string> GetPlatformsForLocation(string location);
    }
}