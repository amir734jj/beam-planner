using System;
using System.Linq;
using App.Models;
using App.Models.Instructions;
using App.Models.Interface;
using static BeamParser;

namespace App.Logic
{
    internal class BeamVisitor : BeamParserBaseVisitor<IToken>
    {
        public override IToken VisitCoordinate(CoordinateContext context)
        {
            var x = double.Parse(context.children[0].GetText());
            var y = double.Parse(context.children[1].GetText());
            var z = double.Parse(context.children[2].GetText());

            return new Coordinate(x, y, z);
        }

        public override IToken VisitInstruction(InstructionContext context)
        {
            var id = int.Parse(context.id().GetText());
            var coordinate = (Coordinate)Visit(context.coordinate());
            
            if (context.User() != null)
            {
                return new UserInstruction(id, coordinate);
            }
            
            if (context.Satellite() != null)
            {
                return new SatelliteInstruction(id, coordinate);
            }
            
            if (context.Interferer() != null)
            {
                return new InterfererInstruction(id, coordinate);
            }

            throw new ArgumentException();
        }

        public override IToken VisitSubroutine(SubroutineContext context)
        {
            return new Subroutine(context.instruction().Select(Visit).Cast<IInstruction>().ToList());
        }
    }
}