using App.Models.Interface;

namespace App.Models.Instructions
{
    internal record SatelliteInstruction(int Id, Coordinate Coordinate) : IInstruction
    {
        public override string ToString()
        {
            return $"sat {Id} {Coordinate}";
        }
    }
}