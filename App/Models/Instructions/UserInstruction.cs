using App.Models.Interface;

namespace App.Models.Instructions
{
    internal record UserInstruction(int Id, Coordinate Coordinate) : IInstruction
    {
        public override string ToString()
        {
            return $"user {Id} {Coordinate}";
        }
    }
}