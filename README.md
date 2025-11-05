# Background
Recently, I had cause to suspect that the System.Random Pseudo-Random Number Generator was not being sufficiently random.  In a sense, the answer to that is "of course not" - it's a *Pseudo*-Random Number Generator.   But what about the quality of it as a PRNG?  Does it have any kind of predictable behaviour that could be causing what I was seeing?

In searching online, I found claims that the System.Random implemention in .NET Framework 4.8 was flawed and could not pass a coin flip test.  I had already implemented a simple uniformity test with a chi-square fitness test.  This passed for all the different ranges I tried.  I went back to these data and checked even vs. odd - no problem.  I tried the test with a range of 2 - still no problem.  So clearly, this problem was fixed prior to .Net 8.  So it's a better PRNG, but how good?

There are suites for testing like [Die Hard](https://en.wikipedia.org/wiki/Diehard_tests) and [Die Harder](https://rurban.github.io/dieharder/manual/dieharder.pdf).  These would require either hooking them up to call the .NET System.Random or porting to a .NET language.  Neither option seemed appealing.

At some point, I came across [NIST Special Publication 800-22 r1a](https://csrc.nist.gov/pubs/sp/800/22/r1/upd1/final), and an [article](https://learn.microsoft.com/en-us/archive/msdn-magazine/2013/government-special-issue/test-run-implementing-the-national-institute-of-standards-and-technology-tests-of-randomness-using-csharp) about a .NET implementation of the first three tests.  This article encouraged me to build this testing application, and implement all 15 tests (in addition to the simple uniformity test I'd started with).

# References
There's an odd notion of global vs. local references in this project.  Global References are those which have general applicabililty to the project as a whole (eg. the aforementioned NIST Special Publication).  Local References are specific to a particular source file.

Global References are called out by letters, and listed in the file References/Sources.ods.  Local References will be listed somewhere in the file.  They may be called out by a number, or, if singular, just referred to as something like "the reference".

# Results
I've informally run each test a bunch of times, without really tracking results or how many times.  However, my impression is that the System.Random implementation in .NET 8 is a really good PRNG.  I did notice one test (without tracking which one) where the p-Values were all suspiciously very high.

Next, I'm going to go back and run each of the tests numerous times, and evaluate the results per Sections 4.2.1 and 4.2.2 of Reference A (that's the aforementioned NIST Special Publication :-)).

| Test | Repeat Count | Proportion of Passing | Uniformity of pValues |
|:-----|:-------------|:----------------------|:----------------------|
| Monobit | 1,000 | PASS | PASS |
| FrequencyBlock | 1,000 | PASS | PASS |
| Runs | 1,000 | PASS | PASS |
| Longest Run Of Ones | 1,000 | PASS | PASS |

## Monobit test
```
$ ./TestRNG -r 1000 monobit -c 1000000 -s 0.01
```
>Running Monobit test
>Call Count: 1,000,000
>Significance: 0.01
>Repeat Count: 1,000
>RESULTS:
>Acceptable proportion of passing sequences is from 0.980561 to 0.999439
>Observed proportion: 0.990000
>Result: Pass>

>Checking histogram for uniformity:
>Chi-Squared: 7.100000
>Uniformity p-Value: 0.626709
>p-Values are uniformly distributed.

## Frequency Block Test
```
$ ./TestRNG -r 1000 frequencyblock -bs 31 -bc 1000 -s 0.01
```
>Running Frequency Block Test
>Block Size: 31
>Block Count: 1000
>RESULTS:
>Acceptable proportion of passing sequences is from 0.980561 to 0.999439
>Observed proportion: 0.994000
>Result: Pass
>
>Checking histogram for uniformity:
>Chi-Squared: 15.360000
>Uniformity p-Value: 0.081510
>p-Values are uniformly distributed.

## Runs Test
```
$ ./TestRNG -r 1000 runs -c 1000000 -s 0.01
```
>Running Runs Test
>Call Count: 1,000,000
>Significance: 0.01
>RESULTS:
>Acceptable proportion of passing sequences is from 0.980561 to 0.999439
>Observed proportion: 0.986000
>Result: Pass
>
>Checking histogram for uniformity:
>Chi-Squared: 7.660000
>Uniformity p-Value: 0.568739
>p-Values are uniformly distributed.

## Longest Run of Ones Test
```
$ ./TestRNG -r 1000 longestrun -bs Small -c 1000000 -s 0.01
```
>Running Longest Run of Ones Test
>Block Size: Small
>Call Count: 1,000,000
>Significance: 0.01
>RESULTS:
>Acceptable proportion of passing sequences is from 0.980561 to 0.999439
>Observed proportion: 0.992000
>Result: Pass
>
>Checking histogram for uniformity:
>Chi-Squared: 2.300000
>Uniformity p-Value: 0.985788
>p-Values are uniformly distributed.

```
$ ./TestRNG -r 1000 longestrun -bs Medium -c 1000000 -s 0.01
```
>Running Longest Run of Ones Test
>Block Size: Medium
>Call Count: 1,000,000
>Significance: 0.01
>RESULTS:
>Acceptable proportion of passing sequences is from 0.980561 to 0.999439
>Observed proportion: 0.991000
>Result: Pass
>
>Checking histogram for uniformity:
>Chi-Squared: 7.680000
>Uniformity p-Value: 0.566688
>p-Values are uniformly distributed.

```
$ ./TestRNG -r 1000 longestrun -bs Large -c 1000000 -s 0.01
```
>Running Longest Run of Ones Test
>Block Size: Large
>Call Count: 1,000,000
>Significance: 0.01
>RESULTS:
>Acceptable proportion of passing sequences is from 0.980561 to 0.999439
>Observed proportion: 0.989000
>Result: Pass
>
>Checking histogram for uniformity:
>Chi-Squared: 5.280000
>Uniformity p-Value: 0.809249
>p-Values are uniformly distributed.
