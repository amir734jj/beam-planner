using System;
using System.Collections.Generic;
using App.Models.Interface;

namespace App.Models
{
    internal record Subroutine(List<IInstruction> Instructions) : IToken
    {
        public override string ToString()
        {
            return string.Join(Environment.NewLine, Instructions);
        }
    }
}