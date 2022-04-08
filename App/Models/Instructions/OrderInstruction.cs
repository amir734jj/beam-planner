namespace App.Models.Instructions
{
    public record OrderInstruction(int Satellite, int Beam, int User, int Color)
    {
        public override string ToString()
        {
            return $"sat {Satellite} beam {Beam} user {User} color {(char) ('A' + Color)}";
        }
    }
}