// ReSharper disable once RedundantUsingDirective

using System.IO;
using Antlr4.Runtime;

// ReSharper disable PublicConstructorInAbstractClass
// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable once CheckNamespace
// ReSharper disable once CheckNamespace
#pragma warning disable CA1050


public abstract class BeamParserBase : Parser
#pragma warning restore CA1050
{
    public BeamParserBase(ITokenStream input)
        : base(input)
    {
    }

    public BeamParserBase(ITokenStream input, TextWriter output, TextWriter errorOutput) : this(input)
    {
    }
}