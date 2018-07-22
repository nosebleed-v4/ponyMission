using System;

namespace PonyRescue
{
    internal enum Direction
    {
        None = 0,
        North,
        East,
        South,
        West
    }

    static class DirectionExtensions
    {
        public static Direction Inverse(this Direction duration)
        {
            switch (duration)
            {
                case Direction.North:
                    return Direction.South;
                case Direction.East:
                    return Direction.West;
                case Direction.South:
                    return Direction.North;
                case Direction.West:
                    return Direction.East;
                default:
                    throw new ArgumentException("Direction must be specified");
            }
        }
    }
}