using System.Collections.Generic;
using Newtonsoft.Json;

namespace PonyRescue
{
    public class MazeState
    {
        private MazeState() { }

        [JsonProperty("pony")]
        private List<int> ponyLocationRaw;

        public int PonyLocation => ponyLocationRaw[0];

        [JsonProperty("domokun")]
        private List<int> domokunLocationRaw { get; set; }
        public int DomokunLocation => domokunLocationRaw[0];

        [JsonProperty("end-point")]
        private List<int> exitLocationRaw { get; set; }
        public int ExitLocation => exitLocationRaw[0];

        [JsonProperty("size")]
        private List<int> sizeRaw { get; set; }
        public int width => sizeRaw[0];
        public int height => sizeRaw[1];

        [JsonProperty("difficulty")]
        public int difficulty { get; set; }

        [JsonProperty("data")]
        public List<List<string>> data { get; set; }

        [JsonProperty("maze_id")]
        public string maze_id { get; set; }

        [JsonProperty("game-state")]
        public GameState gameState { get; set; }
    }
}
