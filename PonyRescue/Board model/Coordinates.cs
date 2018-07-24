using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace PonyRescue
{
    internal struct Coordinates : IEquatable<Coordinates>
    {
        public Coordinates(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public int X { get; }
        public int Y { get; }

        public static int GetXCoordinate(int masterIndex, int width) => masterIndex % width;

        public static int GetYCoordinate(int masterIndex, int width) => masterIndex / width;

        internal Coordinates Move(Direction dir)
        {
            switch (dir)
            {
                case Direction.North:
                    return new Coordinates(X, Y - 1);
                    break;
                case Direction.East:
                    return new Coordinates(X +1,  Y);
                    break;
                case Direction.South:
                    return new Coordinates(X, Y + 1);
                    break;
                case Direction.West:
                    return new Coordinates(X - 1, Y);
                default:
                    throw new ArgumentException("direction not specified");
            }
        }

        public bool Equals(Coordinates other)
        {
            if (this.X == other.X && this.Y == other.Y)
                return true;
           
            return false;
        }
    }
}