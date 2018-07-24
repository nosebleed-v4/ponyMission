using System;
using System.Collections.Generic;
using System.Windows.Documents;

namespace PonyRescue
{
    internal class Chamber
    {
        public Coordinates Coordinates { get; }

        public List<Direction> ChamberConnections = null;

        public List<Direction> PathFromPony { get; private set; } = null;

        public List<Direction> PathToExit { get; private set; } = null;

        public Chamber(Coordinates coordinates)
        {
            this.Coordinates = coordinates;
            this.ChamberConnections = new List<Direction>();
        }

        internal void AddConnectedChamber(Direction direction)
        {
            ChamberConnections.Add(direction);
        }

        internal bool SetPathFromPony(List<Direction> pathFromPony, Direction dir = Direction.None)
        {
            this.PathFromPony = new List<Direction>(pathFromPony);
            if(dir != Direction.None)
                this.PathFromPony.Add(dir);

            if (this.PathToExit != null)
                return true;
            return false;
        }

        internal bool SetPathToExit(List<Direction> pathToExit, Direction dir = Direction.None)
        {
            this.PathToExit = new List<Direction>(pathToExit);
            if (dir != Direction.None)
                PathToExit.Add(dir.Inverse());

            if (this.PathFromPony != null)
                return true;
            return false;
        }

        internal void ClearPathData()
        {
            this.PathFromPony = null;
            this.PathToExit = null; // this does not actually need to be cleared sice it is still valid
        }
    }
}