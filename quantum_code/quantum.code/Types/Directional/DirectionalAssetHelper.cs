using Photon.Deterministic;

namespace Quantum.Types
{
    public static class DirectionalAssetHelper
    {
        public const int SIZE = 256;
        public const int ALIGNMENT = 8;

        private static FP DIRECTION_SUCCESS = (FP)7 / 10;

        public static AssetRefEmote GetFromDirection(DirectionalEmote asset, Direction direction)
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

        public static AnimationRef GetFromDirection(DirectionalAnimationRef asset, Direction direction)
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

        public static void SetFromDirection(DirectionalEmote asset, AssetRefEmote value, Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    asset.Up = value;
                    break;

                case Direction.Down:
                    asset.Down = value;
                    break;

                case Direction.Left:
                    asset.Left = value;
                    break;

                case Direction.Right:
                    asset.Right = value;
                    break;
            }
        }

        public static void SetFromDirection(DirectionalFPVector2 asset, FPVector2 value, Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    asset.Up = value;
                    break;

                case Direction.Down:
                    asset.Down = value;
                    break;

                case Direction.Left:
                    asset.Left = value;
                    break;

                case Direction.Right:
                    asset.Right = value;
                    break;
            }
        }

        public static void SetFromDirection(DirectionalAnimationRef asset, AnimationRef value, Direction direction)
        {
            switch (direction)
            {
                case Direction.Neutral:
                    asset.Neutral = value;
                    break;

                case Direction.Left:
                case Direction.Right:
                    asset.Horizontal = value;
                    break;

                case Direction.Up:
                    asset.Up = value;
                    break;

                case Direction.Down:
                    asset.Down = value;
                    break;
            }
        }

        public static AssetRefEmote GetValueFromDirection(DirectionalEmote asset, FPVector2 direction) => GetFromDirection(asset, GetEnumFromDirection(direction));
        public static FPVector2 GetValueFromDirection(DirectionalFPVector2 asset, FPVector2 direction) => GetFromDirection(asset, GetEnumFromDirection(direction));
        public static AnimationRef GetValueFromDirection(DirectionalAnimationRef asset, FPVector2 direction) => GetFromDirection(asset, GetEnumFromDirection(direction));

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
                result = Direction.Neutral;
            }

            return result;
        }
    }
}
