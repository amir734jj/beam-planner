# Beam Planner

This application is written in C# .NET Core and it uses
- [ANTLR4](https://www.antlr.org/) for lexer and parser of instructions (`LL(*)` parser)
  - see the `BeamLexer.g4` and `BeamParser.g4` for the lexer and parser definition
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
start with all potential combinations and tried to filter down. Also I prioritized serving the most
concentration of users (notice the `OrderBy` in the code) to avoid holding up the satellite to small
subset of users.

## Observation
- As the number of users/satellites/interferences increase, the completeness of my solution decrease. This is expected 
as my solution uses greedy programming (greedy condition: always start with satellite with covered users). The correct
solution should use dynamic programming such that it would provide service to maximum number of users.
- The time complexity of this code is `O(users x satellites x interferences)`
- Although my greedy code does not provide _perfect_ solution, but it make it up by being very fast

## Testing

_Make sure you have Docker installed._

```shell
# To see the generated result
make test_cases/00_example.run

# To test the results against python evaluator
make test_cases/00_example.test

# To test all examples
make regression

# To see how long it takes to run each test case
make benchmark
```

There is a [`regression.txt`](regression.txt) file available.

## Notes 

- There is a `config.json` file available under `/App` which can be used to quickly modify the parameters

## Benchmark

Some optimizations I did/attempted:
- used ANTLR which is an state of art lexer/parser used by many application so reading the instructions is fast
- parallelized the outer loops when there is many level of nested loop by using C# built-in `AsParallel()`
- tried to cache the result of calculation of inner angle of three vectors but code rarely encountered duplicate
calculation so it did not result in any performance improvement
- result of `Analyzer` method is an `IEnumerable` or an iterator which means as the new result comes out of the function, 
it gets printed so no list is used to store the result as they are calculated
- used Rider (C# IDE) memory profiling to analyze which block of code is using too much memory

```shell
Testing 00_example (1 seconds)
Testing 01_simplest_possible (1 seconds)
Testing 02_two_users (1 seconds)
Testing 03_five_users (1 seconds)
Testing 04_one_interferer (1 seconds)
Testing 05_equatorial_plane (1 seconds)
Testing 06_partially_fullfillable (1 seconds)
Testing 07_eighteen_planes (1 seconds)
Testing 08_eighteen_planes_northern (2 seconds)
Testing 09_ten_thousand_users (2 seconds)
Testing 10_ten_thousand_users_geo_belt (2 seconds)
Testing 11_one_hundred_thousand_users (15 seconds)
```

Tested on Ubuntu 21.10 with the following spec:
```shell
(main) ✗ lscpu
Architecture:                    x86_64
CPU op-mode(s):                  32-bit, 64-bit
Byte Order:                      Little Endian
CPU(s):                          8
On-line CPU(s) list:             0-7
Thread(s) per core:              2
Core(s) per socket:              4
Socket(s):                       1
NUMA node(s):                    1
Vendor ID:                       GenuineIntel
CPU family:                      6
Model:                           142
Model name:                      Intel(R) Core(TM) i7-8565U CPU @ 1.80GHz


(main) ✗ cat /proc/meminfo
MemTotal:       16090976 kB
```
