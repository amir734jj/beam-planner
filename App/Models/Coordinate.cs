using App.Models.Interface;

namespace App.Models
{
    internal class Coordinate : IToken
    {
        public static readonly Coordinate Origin = new(0, 0, 0);
        
        public double X { get; }
        public double Y { get; }
        public double Z { get; }

        public Coordinate(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override string ToString()
        {
            return $"{X} {Y} {Z}";
        }
    }
}