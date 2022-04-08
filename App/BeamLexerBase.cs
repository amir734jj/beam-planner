using System.IO;
using Antlr4.Runtime;

// ReSharper disable PublicConstructorInAbstractClass
// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable once CheckNamespace
#pragma warning disable CA1050

public abstract class BeamLexerBase : Lexer
#pragma warning restore CA1050
{
    public BeamLexerBase(ICharStream input)
        : base(input)
    {
    }

    public BeamLexerBase(ICharStream input, TextWriter output, TextWriter errorOutput) : this(input)
    {
    }
}
