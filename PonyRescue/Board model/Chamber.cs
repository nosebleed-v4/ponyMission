using System;
using System.Collections.Generic;
using System.Windows.Documents;

namespace PonyRescue
{
    internal class Chamber
    {
        public Coordinates Coordinates { get; }

        public List<Direction> ConnectedChambers = null;

        public List<string> PathFromPony { get; private set; } = null;

        public List<string> PathToExit { get; private set; } = null;

        public Chamber(Coordinates coordinates)
        {
            this.Coordinates = coordinates;
            this.ConnectedChambers = new List<Direction>();
        }

        internal void AddConnectedChamber(Direction direction)
        {
            ConnectedChambers.Add(direction);
        }

        internal bool SetPathFromPony(List<string> pathFromPony, Direction dir = Direction.None)
        {
            this.PathFromPony = new List<string>(pathFromPony);
            if(dir != Direction.None)
                this.PathFromPony.Add(dir.ToString().ToLower());

            if (this.PathToExit != null)
                return true;
            return false;
        }

        internal bool SetPathToExit(List<string> pathToExit, Direction dir = Direction.None)
        {
            this.PathToExit = new List<string>(pathToExit);
            if (dir != Direction.None)
                PathToExit.Add(dir.Inverse().ToString().ToLower());

            if (this.PathFromPony != null)
                return true;
            return false;
        }
    }
}