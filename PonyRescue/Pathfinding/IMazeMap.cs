using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PonyRescue
{
    internal interface IMazeMap
    {
        Stack<Direction> CurrentPathToExit { get; }
        Stack<Direction> FindShortestPathToExit(Coordinates ponyLocation);
        void Initialize(int mazeStateWidth, int mazeStateHeight, int mazeStatePonyLocation, int mazeStateExitLocation, List<List<string>> data);
        bool IsDomokunClose(Coordinates domokunLocation, Coordinates nextPotentialLocation);
        bool IsMoveLegal(Coordinates ponyLocation, Direction direction);
        [Obsolete("this was an interesting experiment, but unless the maps get much bigger the gain does not seem that great...")]
        Task<Stack<Direction>> FindShortestPathAsync(Coordinates ponyLocation);
    }
}