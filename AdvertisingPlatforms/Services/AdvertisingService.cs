using AdvertisingPlatforms.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AdvertisingPlatforms.Services
{
    public class AdvertisingService : IAdvertisingService
    {
        public async Task LoadPlatformsFromFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Файл не предоставлен или пуст.");

            var newRoot = new Node();

            using var reader = new StreamReader(file.OpenReadStream());
            string? line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                ParseLine(line, newRoot);
            }

            LocationTree.Root = newRoot;
        }        

        public IEnumerable<string> GetPlatformsForLocation(string location)
        {
            if (string.IsNullOrWhiteSpace(location) || !location.StartsWith('/'))
                throw new ArgumentException("Некорректная локация.");

            var segments = location.Split('/', StringSplitOptions.RemoveEmptyEntries);
            var current = LocationTree.Root;
            var result = new HashSet<string>();

            foreach (var segment in segments)
            {
                if (!current.Children.TryGetValue(segment, out var child))
                    break;

                current = child;
                foreach (var platform in current.Platforms)
                    result.Add(platform);
            }

            return result.OrderBy(p => p);
        }

        private void ParseLine(string line, Node root)
        {
            var parts = line.Split(':', 2, StringSplitOptions.TrimEntries);
            if (parts.Length < 2)
                return;

            var platform = parts[0];
            var locations = parts[1].Split(',', StringSplitOptions.TrimEntries);

            foreach (var location in locations)
                AddLocationPath(root, location, platform);
        }

        private void AddLocationPath(Node root, string location, string platform)
        {
            if (!location.StartsWith('/'))
                return;

            var segments = location.Split('/', StringSplitOptions.RemoveEmptyEntries);
            var current = root;

            foreach (var segment in segments)
            {
                if (!current.Children.TryGetValue(segment, out var child))
                {
                    child = new Node();
                    current.Children[segment] = child;
                }
                current = child;
            }

            current.AddPlatform(platform);
        }
    }
}
