using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PonyRescue
{
    class PonyChallengeFacade : IPonyChallengeFacade
    {
        private static readonly HttpClient client = new HttpClient();

        public async Task<string> StartNewGameAsync(int width, int height, string name, int difficulty)
        {
            string url = @"https://ponychallenge.trustpilot.com/pony-challenge/maze";
            //initialize maze
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"maze-width", width},
                {"maze-height",height},
                {"maze-player-name", name},
                {"difficulty",  difficulty}
            };
            StringContent content = new StringContent(ParseDataForPostrequest(data), Encoding.UTF8, "application/json");

            var response = await ExecutePostRequest(url, content);

            return JsonConvert.DeserializeObject<MazeId>(response).Id;
        }

        public async Task<MazeState> GetMazeStateAsync(string mazeId)
        {
            string url = @"https://ponychallenge.trustpilot.com/pony-challenge/maze/" + mazeId;

            string responseBody = await client.GetStringAsync(url);

            return JsonConvert.DeserializeObject<MazeState>(responseBody);
        }

        public async Task MovePonyAsync(string mazeId, Direction direction)
        {
            string url = @"https://ponychallenge.trustpilot.com/pony-challenge/maze/" + mazeId;

            //move pony
            if (direction != Direction.None)
            {
                Dictionary<string, object> data = new Dictionary<string, object>()
                {
                    {"direction", direction.ToString().ToLower()},
                };
                StringContent content = new StringContent(ParseDataForPostrequest(data), Encoding.UTF8, "application/json");
                await ExecutePostRequest(url, content);
            }
        }

        public async Task<string> PrintMazeStateAsync(string mazeId)
        {
            string url = @"https://ponychallenge.trustpilot.com/pony-challenge/maze/" + mazeId + @"/print";
            string responseBody = await ExecuteGetRequest(url);

            return responseBody;
        }


        private static async Task<string> ExecuteGetRequest(string url)
        {
            try
            {
                return await client.GetStringAsync(url);
            }
            catch (Exception e)
            {
                //implement connectivity error handling
                throw;
            }

        }

        private async Task<string> ExecutePostRequest(string url, StringContent content)
        {
            string responseString = "";
            try
            {
                var response = await client.PostAsync(url, content);
                responseString = await response.Content.ReadAsStringAsync();
            }
            catch (Exception e)
            {
                //implement connectivity error handling
                throw;
            }
            return responseString;
        }

        private string ParseDataForPostrequest(Dictionary<string, object> parameters)
        {
            var sb = new StringBuilder();
            sb.Append("{");
            foreach (var kvp in parameters)
            {
                if (kvp.Value is string)
                    sb.Append("\"" + kvp.Key + "\":\"" + kvp.Value + "\",");
                else if (kvp.Value is int)
                    sb.Append("\"" + kvp.Key + "\":" + kvp.Value + ",");
                else
                    throw new InvalidOperationException("only int and string types supported for now");
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append("}");
            return sb.ToString();

        }
    }
}
