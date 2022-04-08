using App.Models.Interface;

namespace App.Models
{
    internal record Coordinate(double X, double Y, double Z) : IToken
    {
        public static readonly Coordinate Origin = new(0, 0, 0);

        public override string ToString()
        {
            return $"{X} {Y} {Z}";
        }
    }
}