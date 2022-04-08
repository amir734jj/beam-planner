# Beam Planner

This application is written in C# .NET Core and it uses
- [ANTLR4](https://www.antlr.org/) for lexer and parser of instructions (`LL(*)` parser)
  - see the `BeamLexer.g4` and `BeamParser.g4` for the lexer and parser 
- [Docker](https://www.docker.com/) to build the console app (Ubuntu+.NET Core SDK+Java for ANTLR)
  - Using Docker was needed because ANTLR uses Java under the hood to analyze the context-free grammar
  and then it dumps C# code during the build time
  - Also the build in Docker sets necessary environment variable (`BUILD_CONFIGURATION`) that turns on 
  the compiler optimization
- [Makefile](https://www.gnu.org/software/make/manual/make.html) for the runtime and testing
  - Simple Makefile to run/test the sample codes

## Solution
This problem is NP-Complete (because verification of correctness of the solution does not 
run in polynomial time) and I used greedy programming to solve this problem. Basically I tried to
start with all possible combinations and tried to filter down. Also I prioritized serving the most
concentration of users (notice the `OrderBy` in the code) to avoid holding up the satellite to small
subset of users.

## Testing

_Make sure you have Docker installed._

```shell
# To see the generated result
make test_cases/00_example.run

# To test the results against python evaluator
make test_cases/00_example.test

# To test all examples
make regression
```

