using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Documents;

namespace PonyRescue
{
    internal class Pathfinder
    {
        private Coordinates ponyLocation;
        private Coordinates exitLocation;
        private int mazeWidth;
        private int mazeHeight;
        private Dictionary<Coordinates, Chamber> Chambers = null;

        public Pathfinder(int mazeStateWidth, int mazeStateHeight, int mazeStatePonyLocation, int mazeStateExitLocation)
        {
            this.mazeWidth = mazeStateWidth;
            this.mazeHeight = mazeStateHeight;
            this.ponyLocation = new Coordinates(Coordinates.GetXCoordinate(mazeStatePonyLocation, mazeWidth), Coordinates.GetYCoordinate(mazeStatePonyLocation, mazeWidth));
            this.exitLocation = new Coordinates(Coordinates.GetXCoordinate(mazeStateExitLocation, mazeWidth), Coordinates.GetYCoordinate(mazeStateExitLocation, mazeWidth));
        }

        internal void InitializeChambers(List<List<string>> data)
        {
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

                if (xIndex < mazeWidth-1 && !data[masterIndex+1].Contains(Direction.West.ToString().ToLower()))
                    Chambers[coordinates].AddConnectedChamber(Direction.East);

                if (yIndex < mazeHeight -1 && !data[masterIndex + mazeWidth].Contains(Direction.North.ToString().ToLower()))
                    Chambers[coordinates].AddConnectedChamber(Direction.South);

                if (!reportedWalls.Contains(Direction.West.ToString().ToLower()))
                    Chambers[coordinates].AddConnectedChamber(Direction.West);
            }
        }

        internal List<string> FindShortestPath()
        {
            List<Coordinates> chambersToExploreStartingFromPony = new List<Coordinates>(){this.ponyLocation};
            Chambers[this.ponyLocation].SetPathFromPony(new List<string>());
            List<Coordinates> chambersToExploreStartingFromExit = new List<Coordinates>(){this.exitLocation};
            Chambers[this.exitLocation].SetPathToExit(new List<string>());
            while(true) //TODO- make sure we never get stuck
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
                                return neighrouringChamber.PathFromPony.Concat(neighrouringChamber.PathToExit).ToList();
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
                                return neighrouringChamber.PathFromPony.Concat(neighrouringChamber.PathToExit).ToList();
                            }
                            chambersToExploreStartingFromExitNext.Add(neighrouringChamber.Coordinates);
                        }
                    }
                }
                chambersToExploreStartingFromExit = chambersToExploreStartingFromExitNext;
            };
        }
    }
}
