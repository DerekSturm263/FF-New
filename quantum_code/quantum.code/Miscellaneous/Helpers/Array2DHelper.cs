
namespace Quantum.Types
{
    public static class Array2DHelper
    {
        public static AssetRefTile[,] All(Array2DTiles array)
        {
            return new AssetRefTile[5, 5]
            {
                { array.A0, array.A1, array.A2, array.A3, array.A4 },
                { array.B0, array.B1, array.B2, array.B3, array.B4 },
                { array.C0, array.C1, array.C2, array.C3, array.C4 },
                { array.D0, array.D1, array.D2, array.D3, array.D4 },
                { array.E0, array.E1, array.E2, array.E3, array.E4 }
            };
        }
    }
}
