using Newtonsoft.Json;

namespace PonyRescue
{
    internal class MazeId
    {
        [JsonProperty("maze_id")]
        public string Id { get; private set; }
    }
}