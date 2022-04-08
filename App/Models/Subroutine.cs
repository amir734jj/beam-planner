using System;
using System.Collections.Generic;
using App.Models.Interface;

namespace App.Models
{
    internal class Subroutine: IToken
    {
        public List<IInstruction> Instructions { get; }

        public Subroutine(List<IInstruction> instructions)
        {
            Instructions = instructions;
        }

        public override string ToString()
        {
            return string.Join(Environment.NewLine, Instructions);
        }
    }
}