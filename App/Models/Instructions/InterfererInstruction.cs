using App.Models.Interface;

namespace App.Models.Instructions
{
    internal class InterfererInstruction : IInstruction
    {
        public int Id { get; }
        public Coordinate Coordinate { get; }

        public InterfererInstruction(int id, Coordinate coordinate)
        {
            Id = id;
            Coordinate = coordinate;
        }

        public override string ToString()
        {
            return $"interferer {Id} {Coordinate}";
        }
    }
}