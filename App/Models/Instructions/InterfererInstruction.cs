using App.Models.Interface;

namespace App.Models.Instructions
{
    internal record InterfererInstruction(int Id, Coordinate Coordinate) : IInstruction
    {
        public override string ToString()
        {
            return $"interferer {Id} {Coordinate}";
        }
    }
}