using App.Models.Interface;

namespace App.Models.Instructions
{
    internal class UserInstruction : IInstruction
    {
        public int Id { get; }
        public Coordinate Coordinate { get; }

        public UserInstruction(int id, Coordinate coordinate)
        {
            Id = id;
            Coordinate = coordinate;
        }
        
        public override string ToString()
        {
            return $"user {Id} {Coordinate}";
        }
    }
}