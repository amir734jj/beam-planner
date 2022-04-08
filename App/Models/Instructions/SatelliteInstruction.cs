using App.Models.Interface;

namespace App.Models.Instructions
{
    internal class SatelliteInstruction : IInstruction
    {
        public int Id { get; }
        public Coordinate Coordinate { get; }

        public SatelliteInstruction(int id, Coordinate coordinate)
        {
            Id = id;
            Coordinate = coordinate;
        }
        
        public override string ToString()
        {
            return $"sat {Id} {Coordinate}";
        }
    }
}