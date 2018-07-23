using System.Linq;

namespace PonyRescue
{
    internal class PonyNavigator : IPonyNavigator
    {
        public Direction GetNextMove(IMazeMap mazeMap, MazeState mazeState)
        {
            if (mazeMap.PathToExit == null)
                mazeMap.FindShortestPath();

            Coordinates monsterLocation = new Coordinates(Coordinates.GetXCoordinate(mazeState.DomokunLocation, mazeState.width), Coordinates.GetYCoordinate(mazeState.DomokunLocation, mazeState.width));

            //if monster is less than two steps away on the possible track and other routs are possible
            //dodge
            //else
            //go to exit

            return Direction.None;
        }
    }
}