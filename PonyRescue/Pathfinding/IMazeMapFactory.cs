using System.Collections.Generic;

namespace PonyRescue
{
    internal interface IMazeMapFactory
    {
        IMazeMap Create(int boardWidth, int boardHeight, int ponyLocation, int exitLocation, List<List<string>> data);
    }
}