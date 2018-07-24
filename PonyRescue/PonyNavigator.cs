using System;
using System.Linq;
using System.Windows.Navigation;

namespace PonyRescue
{
    internal class PonyNavigator : IPonyNavigator
    {
        public Direction GetNextMove(IMazeMap mazeMap, MazeState mazeState)
        {
            Direction returnValue = Direction.None;
            Coordinates domokunLocation = new Coordinates(Coordinates.GetXCoordinate(mazeState.DomokunLocation, mazeState.width), Coordinates.GetYCoordinate(mazeState.DomokunLocation, mazeState.width));
            Coordinates ponyLocation = new Coordinates(Coordinates.GetXCoordinate(mazeState.PonyLocation, mazeState.width), Coordinates.GetYCoordinate(mazeState.PonyLocation, mazeState.width));

            if (mazeMap.CurrentPathToExit == null)
                mazeMap.FindShortestPathToExit(ponyLocation);

            Direction nextPotentialMove = mazeMap.CurrentPathToExit.Peek();
            var nextPotentialLocation = ponyLocation.Move(nextPotentialMove);
            if(mazeMap.IsDomokunClose(domokunLocation, nextPotentialLocation))
            {
                //see if we can escape
                var possibleDodge = Enum.GetValues(typeof(Direction)).Cast<Direction>().AsEnumerable().FirstOrDefault(direction => direction != Direction.None && direction != nextPotentialMove && mazeMap.IsMoveLegal(ponyLocation, direction));
                if (possibleDodge != default(Direction))
                {
                    //escape
                    mazeMap.CurrentPathToExit.Push(possibleDodge.Inverse());
                    returnValue = possibleDodge;
                }
                else
                {
                    //accept our fate
                    returnValue = mazeMap.CurrentPathToExit.Pop();
                }
            }
            else
            {
                returnValue = mazeMap.CurrentPathToExit.Pop();
            }
            return returnValue;
        }
    }
}