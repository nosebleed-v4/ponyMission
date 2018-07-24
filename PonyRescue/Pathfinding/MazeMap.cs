using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace PonyRescue
{
    internal class MazeMap : IMazeMap
    {
        private Coordinates exitLocation;
        private int mazeWidth;
        private int mazeHeight;
        private Dictionary<Coordinates, Chamber> Chambers = null;

        public Stack<Direction> CurrentPathToExit { get; private set; }

        public void Initialize(int mazeStateWidth, int mazeStateHeight, int mazeStatePonyLocation, int mazeStateExitLocation, List<List<string>> data)
        {
            this.mazeWidth = mazeStateWidth;
            this.mazeHeight = mazeStateHeight;
            this.exitLocation = new Coordinates(Coordinates.GetXCoordinate(mazeStateExitLocation, mazeWidth), Coordinates.GetYCoordinate(mazeStateExitLocation, mazeWidth));

            Chambers = new Dictionary<Coordinates, Chamber>();

            for (int masterIndex = 0; masterIndex < data.Count; masterIndex++)
            {
                int xIndex = Coordinates.GetXCoordinate(masterIndex, mazeWidth);
                int yIndex = Coordinates.GetYCoordinate(masterIndex, mazeWidth);
                var coordinates = new Coordinates(xIndex, yIndex);
                Chambers.Add(coordinates, new Chamber(coordinates));

                var reportedWalls = data[masterIndex];

                if (!reportedWalls.Contains(Direction.North.ToString().ToLower()))
                    Chambers[coordinates].AddConnectedChamber(Direction.North);

                if (xIndex < mazeWidth - 1 && !data[masterIndex + 1].Contains(Direction.West.ToString().ToLower()))
                    Chambers[coordinates].AddConnectedChamber(Direction.East);

                if (yIndex < mazeHeight - 1 && !data[masterIndex + mazeWidth].Contains(Direction.North.ToString().ToLower()))
                    Chambers[coordinates].AddConnectedChamber(Direction.South);

                if (!reportedWalls.Contains(Direction.West.ToString().ToLower()))
                    Chambers[coordinates].AddConnectedChamber(Direction.West);
            }
        }

        public bool IsDomokunClose(Coordinates domokunLocation, Coordinates nextPotentialLocation)
        {
            var nextPotentialChamber = Chambers[nextPotentialLocation];
            return domokunLocation.Equals(nextPotentialLocation) 
                   || nextPotentialChamber.ChamberConnections.Any(direction => nextPotentialLocation.Move(direction).Equals(domokunLocation));
        }

        public Stack<Direction> FindShortestPathToExit(Coordinates ponyLocation)
        {
            if (CurrentPathToExit != null)
            {
                foreach (Chamber cham in Chambers.Values)
                {
                    cham.ClearPathData();
                }
            }

            List<Coordinates> chambersWithNeighboursToExploreStartingFromPony = new List<Coordinates>(){ponyLocation};
            Chambers[ponyLocation].SetPathFromPony(new List<Direction>());
            List<Coordinates> chambersWithNeighboursToExploreStartingFromExit = new List<Coordinates>(){this.exitLocation};
            Chambers[this.exitLocation].SetPathToExit(new List<Direction>());
            while(true)
            {
                List<Coordinates> chambersToExploreStartingFromPonyNext = new List<Coordinates>();
                //find chambers in range of pony
                foreach (Coordinates location in chambersWithNeighboursToExploreStartingFromPony)
                {
                    var currentChamber = Chambers[location];
                    foreach (Direction dir in currentChamber.ChamberConnections)
                    {
                        var neighrouringChamber = Chambers[location.Move(dir)];
                        if (neighrouringChamber.PathFromPony == null)
                        {
                            if (neighrouringChamber.SetPathFromPony(currentChamber.PathFromPony, dir))
                            {
                                neighrouringChamber.PathToExit.Reverse();
                                var shortestPathFound = neighrouringChamber.PathFromPony.Concat(neighrouringChamber.PathToExit).ToList();
                                shortestPathFound.Reverse();
                                CurrentPathToExit = new Stack<Direction>(shortestPathFound);
                                return CurrentPathToExit;
                            }
                            chambersToExploreStartingFromPonyNext.Add(neighrouringChamber.Coordinates);
                        }   
                    }
                }
                chambersWithNeighboursToExploreStartingFromPony = chambersToExploreStartingFromPonyNext;


                var chambersToExploreStartingFromExitNext = new List<Coordinates>();
                foreach (Coordinates location in chambersWithNeighboursToExploreStartingFromExit)
                {
                    var currentChamber = Chambers[location];
                    foreach (Direction dir in currentChamber.ChamberConnections)
                    {
                        var neighrouringChamber = Chambers[location.Move(dir)];
                        if (neighrouringChamber.PathToExit == null)
                        {
                            if(neighrouringChamber.SetPathToExit(currentChamber.PathToExit, dir))
                            {
                                neighrouringChamber.PathToExit.Reverse();
                                var shortestPathFound = neighrouringChamber.PathFromPony.Concat(neighrouringChamber.PathToExit).ToList();
                                shortestPathFound.Reverse();
                                CurrentPathToExit = new Stack<Direction>(shortestPathFound);
                                return CurrentPathToExit;
                            }
                            chambersToExploreStartingFromExitNext.Add(neighrouringChamber.Coordinates);
                        }
                    }
                }
                chambersWithNeighboursToExploreStartingFromExit = chambersToExploreStartingFromExitNext;
            };
        }

        public async Task<Stack<Direction>> FindShortestPathAsync(Coordinates ponyLocation)
        {
            if (CurrentPathToExit != null)
            {
                foreach (Chamber cham in Chambers.Values)
                {
                    cham.ClearPathData();
                }
            }
            var cancelationTokenSource = new CancellationTokenSource(10000); //10 seconds timeout

            Task<List<Direction>> searchFromPonyLocation = Task.Run(() =>
            {
                var startingLocation = ponyLocation;
                bool FillPathInfoInChamber(Chamber chamber, List<Direction> path, Direction direction)
                {
                    return chamber.SetPathFromPony(path, direction);
                }
                List<Direction> PropertySelector(Chamber c) => c.PathFromPony;
                return PopulatePathFindingData(startingLocation, FillPathInfoInChamber, PropertySelector, cancelationTokenSource.Token);
            }, cancelationTokenSource.Token);

            Task<List<Direction>> searchFromExitLocation = Task.Run(() =>
            {
                var startingLocation = this.exitLocation;
                bool FillPathInfoInChamber(Chamber chamber, List<Direction> path, Direction direction)
                {
                    return chamber.SetPathToExit(path, direction);
                }
                List<Direction> PropertySelector(Chamber c) => c.PathToExit;
                return PopulatePathFindingData(startingLocation, FillPathInfoInChamber, PropertySelector, cancelationTokenSource.Token);
            }, cancelationTokenSource.Token);

            var firstTaskToComplete = await Task.WhenAny(new[] { searchFromPonyLocation, searchFromExitLocation });
            cancelationTokenSource.Cancel();
            var outcome = firstTaskToComplete.Status == TaskStatus.RanToCompletion ? firstTaskToComplete.Result : new List<Direction>();
            outcome.Reverse();
            this.CurrentPathToExit = new Stack<Direction>(outcome);
            return CurrentPathToExit;
        }

        private List<Direction> PopulatePathFindingData(Coordinates startingLocation, Func<Chamber, List<Direction>, Direction, bool> fillPathInfoInChamber, Func<Chamber, List<Direction>> propertySelector, CancellationToken cancelationToken)
        {
            List<Coordinates> chambersToExplore = new List<Coordinates>() { startingLocation };
            fillPathInfoInChamber(Chambers[startingLocation], new List<Direction>(), Direction.None);
            while (true)
            {
                if (cancelationToken.IsCancellationRequested)
                    return new List<Direction>();
                List<Coordinates> chambersToExploreNext = new List<Coordinates>();
                foreach (Coordinates location in chambersToExplore)
                {
                    var currentChamber = Chambers[location];
                    foreach (Direction dir in currentChamber.ChamberConnections) //the neighbours could be inferred dynamically during the path search
                    {
                        var neighrouringChamber = Chambers[location.Move(dir)];
                        if (propertySelector(neighrouringChamber) == null)
                        {
                            if (fillPathInfoInChamber(neighrouringChamber, propertySelector(currentChamber), dir))
                            {
                                neighrouringChamber.PathToExit.Reverse();
                                return neighrouringChamber.PathFromPony.Concat(neighrouringChamber.PathToExit).ToList();
                            }
                            chambersToExploreNext.Add(neighrouringChamber.Coordinates);
                        }
                    }
                }
                chambersToExplore = chambersToExploreNext;
            }
        }

        public bool IsMoveLegal(Coordinates ponyLocation, Direction direction)
        {
            return Chambers[ponyLocation].ChamberConnections.Contains(direction);
        }
    }
}
