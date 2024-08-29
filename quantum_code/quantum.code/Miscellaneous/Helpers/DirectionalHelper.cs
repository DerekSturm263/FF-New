using Photon.Deterministic;

namespace Quantum.Types
{
    public static class DirectionalHelper
    {
        public const int SIZE = 256;
        public const int ALIGNMENT = 8;

        private static FP DIRECTION_SUCCESS = (FP)7 / 10;

        public static EmoteMessageBinding GetFromDirection(DirectionalEmote asset, Direction direction)
        {
            return direction switch
            {
                Direction.Up => asset.Up,
                Direction.Down => asset.Down,
                Direction.Left => asset.Left,
                Direction.Right => asset.Right,
                _ => default
            };
        }

        public static FPVector2 GetFromDirection(DirectionalFPVector2 asset, Direction direction)
        {
            return direction switch
            {
                Direction.Up => asset.Up,
                Direction.Down => asset.Down,
                Direction.Left => asset.Left,
                Direction.Right => asset.Right,
                _ => default
            };
        }

        public static MoveRef GetFromDirection(DirectionalMoveRef asset, Direction direction)
        {
            return direction switch
            {
                Direction.Neutral => asset.Neutral,
                Direction.Left => asset.Horizontal,
                Direction.Right => asset.Horizontal,
                Direction.Up => asset.Up,
                Direction.Down => asset.Down,
                _ => default
            };
        }

        public static MoveRef GetFromDirection(DirectionalAerialMoveRef asset, Direction direction, bool isFacingRight)
        {
            return direction switch
            {
                Direction.Neutral => asset.Neutral,
                Direction.Left => isFacingRight ? asset.Backward : asset.Forward,
                Direction.Right => isFacingRight ? asset.Forward : asset.Backward,
                Direction.Up => asset.Up,
                Direction.Down => asset.Down,
                _ => default
            };
        }

        public static Direction GetEnumFromDirection(FPVector2 direction)
        {
            Direction result = Direction.Neutral;

            if (FPVector2.Dot(direction, FPVector2.Up) > DIRECTION_SUCCESS)
            {
                result = Direction.Up;
            }
            else if (FPVector2.Dot(direction, FPVector2.Down) > DIRECTION_SUCCESS)
            {
                result = Direction.Down;
            }
            else if (FPVector2.Dot(direction, FPVector2.Left) > DIRECTION_SUCCESS)
            {
                result = Direction.Left;
            }
            else if (FPVector2.Dot(direction, FPVector2.Right) > DIRECTION_SUCCESS)
            {
                result = Direction.Right;
            }

            return result;
        }
    }
}
