using System.Collections.Generic;

namespace PonyRescue
{
    internal class MazeMapFactory: IMazeMapFactory
    {
        public IMazeMap Create(int boardWidth, int boardHeight, int ponyLocation, int exitLocation, List<List<string>> data)
        {
            var mazeMap = new MazeMap();
            mazeMap.Initialize(boardWidth, boardHeight, ponyLocation, exitLocation, data);
            return mazeMap;
        }
    }
}