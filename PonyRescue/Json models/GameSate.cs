using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PonyRescue
{
    public class GameState
    {
        private GameState() { }

        [JsonProperty("state")]
        public string gameState { get; set; }

        [JsonProperty("state-result")]
        public string gameResult { get; set; }
    }
}
