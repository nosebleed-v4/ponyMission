using System.Collections.Generic;
using System.Threading.Tasks;

namespace PonyRescue
{
    internal interface IMazeMap
    {
        List<string> PathToExit { get; }
        List<string> FindShortestPath();
        Task<List<string>> FindShortestPathAsync();
        void Initialize(int mazeStateWidth, int mazeStateHeight, int mazeStatePonyLocation, int mazeStateExitLocation, List<List<string>> data);
    }
}