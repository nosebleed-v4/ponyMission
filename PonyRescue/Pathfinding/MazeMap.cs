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
        private Coordinates ponyLocation;
        private Coordinates exitLocation;
        private int mazeWidth;
        private int mazeHeight;
        private Dictionary<Coordinates, Chamber> Chambers = null;
        public List<string> PathToExit { get; private set; }

        public void Initialize(int mazeStateWidth, int mazeStateHeight, int mazeStatePonyLocation, int mazeStateExitLocation, List<List<string>> data)
        {
            this.mazeWidth = mazeStateWidth;
            this.mazeHeight = mazeStateHeight;
            this.ponyLocation = new Coordinates(Coordinates.GetXCoordinate(mazeStatePonyLocation, mazeWidth), Coordinates.GetYCoordinate(mazeStatePonyLocation, mazeWidth));
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

        public List<string> FindShortestPath()
        {
            List<Coordinates> chambersToExploreStartingFromPony = new List<Coordinates>(){this.ponyLocation};
            Chambers[this.ponyLocation].SetPathFromPony(new List<string>());
            List<Coordinates> chambersToExploreStartingFromExit = new List<Coordinates>(){this.exitLocation};
            Chambers[this.exitLocation].SetPathToExit(new List<string>());
            while(true)
            {
                List<Coordinates> chambersToExploreStartingFromPonyNext = new List<Coordinates>();
                //find chambers in range of pony
                foreach (Coordinates location in chambersToExploreStartingFromPony)
                {
                    var currentChamber = Chambers[location];
                    foreach (Direction dir in currentChamber.ConnectedChambers)
                    {
                        var neighrouringChamber = Chambers[location.Move(dir)];
                        if (neighrouringChamber.PathFromPony == null)
                        {
                            if (neighrouringChamber.SetPathFromPony(currentChamber.PathFromPony, dir))
                            {
                                neighrouringChamber.PathToExit.Reverse();
                                var shortestPathFound = neighrouringChamber.PathFromPony.Concat(neighrouringChamber.PathToExit).ToList();
                                PathToExit = shortestPathFound;
                                return shortestPathFound;
                            }
                            chambersToExploreStartingFromPonyNext.Add(neighrouringChamber.Coordinates);
                        }   
                    }
                }
                chambersToExploreStartingFromPony = chambersToExploreStartingFromPonyNext;


                List<Coordinates> chambersToExploreStartingFromExitNext = new List<Coordinates>();
                foreach (Coordinates location in chambersToExploreStartingFromExit)
                {
                    var currentChamber = Chambers[location];
                    foreach (Direction dir in currentChamber.ConnectedChambers)
                    {
                        var neighrouringChamber = Chambers[location.Move(dir)];
                        if (neighrouringChamber.PathToExit == null)
                        {
                            if(neighrouringChamber.SetPathToExit(currentChamber.PathToExit, dir))
                            {
                                neighrouringChamber.PathToExit.Reverse();
                                var shortestPathFound = neighrouringChamber.PathFromPony.Concat(neighrouringChamber.PathToExit).ToList();
                                PathToExit = shortestPathFound;
                                return shortestPathFound;
                            }
                            chambersToExploreStartingFromExitNext.Add(neighrouringChamber.Coordinates);
                        }
                    }
                }
                chambersToExploreStartingFromExit = chambersToExploreStartingFromExitNext;
            };
        }

        //this was an interesting experiment, but the gain does not seem that great...
        public async Task<List<string>> FindShortestPathAsync()
        {
            var cancelationTokenSource = new CancellationTokenSource(10000); //10 seconds timeout

            Task<List<string>> SearchFromPonyLocation = Task.Run(() =>
            {
                var startingLocation = this.ponyLocation;
                Func<Chamber, List<string>, Direction, bool> fillPathInfoInChamber = (Chamber chamber, List<string> path, Direction direction) =>
                {
                    return chamber.SetPathFromPony(path, direction);
                };
                Func<Chamber, List<string>> propertySelector = (Chamber c) => c.PathFromPony;
                return PopulatePathFindingData(startingLocation, fillPathInfoInChamber, propertySelector, cancelationTokenSource.Token);
            }, cancelationTokenSource.Token);

            Task<List<string>> SearchFromExitLocation = Task.Run(() =>
            {
                var startingLocation = this.exitLocation;
                Func<Chamber, List<string>, Direction, bool> fillPathInfoInChamber = (Chamber chamber, List<string> path, Direction direction) =>
                {
                    return chamber.SetPathToExit(path, direction);
                };
                Func<Chamber, List<string>> propertySelector = (Chamber c) => c.PathToExit;
                return PopulatePathFindingData(startingLocation, fillPathInfoInChamber, propertySelector, cancelationTokenSource.Token);
            }, cancelationTokenSource.Token);

            var firstTaskToComplete = await Task.WhenAny(new[] { SearchFromPonyLocation, SearchFromExitLocation });
            cancelationTokenSource.Cancel();
            var outcome = firstTaskToComplete.Status == TaskStatus.RanToCompletion ? firstTaskToComplete.Result : null;
            this.PathToExit = outcome;
            return outcome;
        }

        private List<string> PopulatePathFindingData(Coordinates startingLocation, Func<Chamber, List<string>, Direction, bool> fillPathInfoInChamber, Func<Chamber, List<string>> propertySelector, CancellationToken cancelationToken)
        {
            List<Coordinates> chambersToExplore = new List<Coordinates>() { startingLocation };
            fillPathInfoInChamber(Chambers[startingLocation], new List<string>(), Direction.None);
            while (true)
            {
                if (cancelationToken.IsCancellationRequested)
                    return new List<string>();
                List<Coordinates> chambersToExploreNext = new List<Coordinates>();
                foreach (Coordinates location in chambersToExplore)
                {
                    var currentChamber = Chambers[location];
                    foreach (Direction dir in currentChamber.ConnectedChambers) //the neighbours could be inferred dynamically during the path search
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
    }
}
