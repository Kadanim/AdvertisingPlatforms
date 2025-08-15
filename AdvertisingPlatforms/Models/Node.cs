namespace AdvertisingPlatforms.Models
{
    public class Node
    {
        public Dictionary<string, Node> Children { get; } = new Dictionary<string, Node>();
        private readonly HashSet<string> _platforms = new HashSet<string>();
        public IReadOnlyCollection<string> Platforms => _platforms; 
        public void AddPlatform(string platform) => _platforms.Add(platform);
    }
}
