namespace App.Models.Instructions
{
    public class OrderInstruction
    {
        private readonly int _satellite;
        private readonly int _beam;
        private readonly int _user;
        private readonly int _color;

        public OrderInstruction(int satellite, int beam, int user, int color)
        {
            _satellite = satellite;
            _beam = beam;
            _user = user;
            _color = color;
        }

        public override string ToString()
        {
            return $"sat {_satellite} beam {_beam} user {_user} color {(char) ('A' + _color)}";
        }
    }
}