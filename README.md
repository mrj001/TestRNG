# Background
Recently, I had cause to suspect that the System.Random Pseudo-Random Number Generator was not being sufficiently random.  In a sense, the answer to that is "of course not" - it's a *Pseudo*-Random Number Generator.   But what about the quality of it as a PRNG?  Does it have any kind of predictable behaviour that could be causing what I was seeing?

In searching online, I found claims that the System.Random implemention in .NET Framework 4.8 was flawed and could not pass a [coin flip test](https://fuglede.dk/en/blog/bias-in-net-rng/).  I had already implemented a simple uniformity test with a chi-square fitness test.  This passed for all the different ranges I tried.  I went back to these data and checked even vs. odd - no problem.  I tried the test with a range of 2 - still no problem.  So clearly, this problem was fixed prior to .Net 8.  So it's a better PRNG, but how good?

There are suites for testing like [Die Hard](https://en.wikipedia.org/wiki/Diehard_tests) and [Die Harder](https://rurban.github.io/dieharder/manual/dieharder.pdf).  These would require either hooking them up to call the .NET System.Random or porting to a .NET language.  Neither option seemed appealing.

At some point, I came across [NIST Special Publication 800-22 r1a](https://csrc.nist.gov/pubs/sp/800/22/r1/upd1/final), and an [article](https://learn.microsoft.com/en-us/archive/msdn-magazine/2013/government-special-issue/test-run-implementing-the-national-institute-of-standards-and-technology-tests-of-randomness-using-csharp) about a .NET implementation of the first three tests.  This article encouraged me to build this testing application, and implement all 15 tests (in addition to the simple uniformity test I'd started with).

# References
There's an odd notion of global vs. local references in this project.  Global References are those which have general applicabililty to the project as a whole (eg. the aforementioned NIST Special Publication).  Local References are specific to a particular source file.

Global References are called out by letters, and listed in the file References/Sources.ods.  Local References will be listed somewhere in the file.  They may be called out by a number, or, if singular, just referred to as something like "the reference".

# Results
I've informally run each test a bunch of times, without really tracking results or how many times.  However, my impression is that the System.Random implementation in .NET 8 is a really good PRNG.  I did notice one test (without tracking which one) where the p-Values were all suspiciously very high.

The following table is the result of many runs of each test, and evaluating the results per Sections 4.2.1 and 4.2.2 of Reference A (that's the aforementioned NIST Special Publication :-)).

| NIST | Test | Repeat Count | Proportion of Passing | Uniformity of pValues |
|:-----|:-----|:-------------|:----------------------|:----------------------|
| 2.1 | Monobit | 1,000 | PASS | PASS |
| 2.2 | FrequencyBlock | 1,000 | PASS | PASS |
| 2.3 | Runs | 1,000 | PASS | PASS |
| 2.4 | Longest Run Of Ones | 1,000 | PASS | PASS |
| 2.5 | Binary Matrix Rank | 1,000 | PASS | PASS |
| 2.6 | Spectral | 1,000 | PASS | FAIL |
| 2.7 | Non-overlapping Template | 1,000 | FAIL | FAIL |
| 2.8 | Overlapping Template | 1,000 | PASS | PASS |
| 2.9 | Maurer's "Universal Statistical" | 1,000 | FAIL | FAIL |
| 2.10 | Linear Complexity | 1,000 | FAIL | FAIL |
| 2.11 | Serial | 1,000 | PASS\* | PASS |
| 2.12 | Approximate Entropy | 1,000 | FAIL | FAIL |
| 2.13 | Cumulative Sums | 1,000 | PASS | PASS |
| 2.14 | Random Excursions | 1,000 | FAIL\*\* | FAIL\*\* |
| 2.15 | Random Excursions Variant | 1,000 | PASS | PASS |

\* A few failures were observed, but not enough to fail overall.\
\*\* Overall too few sequences achieved the minimum required number of cycles.  Accordingly, 
the randomness hypothesis had to be rejected.  It is only when excluding these sequences
with insufficient cycles do we find all the states passing the Proportion of passing
and uniformity of p-Value tests.

## 2.1 Monobit Test
```
$ ./TestRNG -r 1000 monobit -c 1000000 -s 0.01
```
>Running Monobit test\
>Call Count: 1,000,000\
>Significance: 0.01\
>Repeat Count: 1,000\
>RESULTS:\
>Acceptable proportion of passing sequences is from 0.980561 to 0.999439\
>Observed proportion: 0.990000\
>Result: Pass\
>\
>Checking histogram for uniformity:\
>Chi-Squared: 7.100000\
>Uniformity p-Value: 0.626709\
>p-Values are uniformly distributed.

## 2.2 Frequency Block Test
```
$ ./TestRNG -r 1000 frequencyblock -bs 31 -bc 1000 -s 0.01
```
>Running Frequency Block Test\
>Block Size: 31\
>Block Count: 1000\
>RESULTS:\
>Acceptable proportion of passing sequences is from 0.980561 to 0.999439\
>Observed proportion: 0.994000\
>Result: Pass\
>\
>Checking histogram for uniformity:\
>Chi-Squared: 15.360000\
>Uniformity p-Value: 0.081510\
>p-Values are uniformly distributed.

## 2.3 Runs Test
```
$ ./TestRNG -r 1000 runs -c 1000000 -s 0.01
```
>Running Runs Test\
>Call Count: 1,000,000\
>Significance: 0.01\
>RESULTS:\
>Acceptable proportion of passing sequences is from 0.980561 to 0.999439\
>Observed proportion: 0.986000\
>Result: Pass\
>\
>Checking histogram for uniformity:\
>Chi-Squared: 7.660000\
>Uniformity p-Value: 0.568739\
>p-Values are uniformly distributed.

## 2.4 Longest Run of Ones Test
```
$ ./TestRNG -r 1000 longestrun -bs Small -c 1000000 -s 0.01
```
>Running Longest Run of Ones Test\
>Block Size: Small\
>Call Count: 1,000,000\
>Significance: 0.01\
>RESULTS:\
>Acceptable proportion of passing sequences is from 0.980561 to 0.999439\
>Observed proportion: 0.992000\
>Result: Pass\
>\
>Checking histogram for uniformity:\
>Chi-Squared: 2.300000\
>Uniformity p-Value: 0.985788\
>p-Values are uniformly distributed.

```
$ ./TestRNG -r 1000 longestrun -bs Medium -c 1000000 -s 0.01
```
>Running Longest Run of Ones Test\
>Block Size: Medium\
>Call Count: 1,000,000\
>Significance: 0.01\
>RESULTS:\
>Acceptable proportion of passing sequences is from 0.980561 to 0.999439\
>Observed proportion: 0.991000\
>Result: Pass\
>\
>Checking histogram for uniformity:\
>Chi-Squared: 7.680000\
>Uniformity p-Value: 0.566688\
>p-Values are uniformly distributed.

```
$ ./TestRNG -r 1000 longestrun -bs Large -c 1000000 -s 0.01
```
>Running Longest Run of Ones Test\
>Block Size: Large\
>Call Count: 1,000,000\
>Significance: 0.01\
>RESULTS:\
>Acceptable proportion of passing sequences is from 0.980561 to 0.999439\
>Observed proportion: 0.989000\
>Result: Pass\
>\
>Checking histogram for uniformity:\
>Chi-Squared: 5.280000\
>Uniformity p-Value: 0.809249\
>p-Values are uniformly distributed.\

## 2.5 Binary Matrix Rank Test
```
$ ./TestRNG -r 1000 matrixrank -ms 32 -c 1000000 -s 0.01
```
>Binary Matrix Rank Test\
>Matrix Size: 32\
>Call Count: 1,000,000\
>Significance: 0.01\
>Matrix Count: 976\
>Unused Bit Count: 576\
>RESULTS:\
>Acceptable proportion of passing sequences is from 0.980561 to 0.999439\
>Observed proportion: 0.983000\
>Result: Pass\
>\
>Checking histogram for uniformity:\
>Chi-Squared: 18.600000\
>Uniformity p-Value: 0.028817\
>p-Values are uniformly distributed.

## 2.6 Spectral Test
```
$ ./TestRNG -r 1000 spectral -c 1000 -s 0.01
```
>Spectral Test\
>Call Count: 1,000\
>Significance: 0.01\
>Call Count was adjusted to 1,024\
>RESULTS:\
>Acceptable proportion of passing sequences is from 0.980561 to 0.999439\
>Observed proportion: 0.988000\
>Result: Pass\
>\
>Checking histogram for uniformity:\
>Chi-Squared: 407.380000\
>Uniformity p-Value: 0.000000\
>p-Values are NOT uniformly distributed

```
$ ./TestRNG -r 1000 spectral -c 2048 -s 0.01
```
>Spectral Test\
>Call Count: 2,048\
>Significance: 0.01\
>RESULTS:\
>Acceptable proportion of passing sequences is from 0.980561 to 0.999439\
>Observed proportion: 0.989000\
>Result: Pass\
>\
>Checking histogram for uniformity:\
>Chi-Squared: 104.480000\
>Uniformity p-Value: 0.000000\
>p-Values are NOT uniformly distributed

```
$ ./TestRNG -r 1000 spectral -c 4096 -s 0.01
```
>Spectral Test\
>Call Count: 4,096\
>Significance: 0.01\
>RESULTS:\
>Acceptable proportion of passing sequences is from 0.980561 to 0.999439\
>Observed proportion: 0.990000\
>Result: Pass\
>\
>Checking histogram for uniformity:\
>Chi-Squared: 23.520000\
>Uniformity p-Value: 0.005128\
>p-Values are uniformly distributed.

## 2.7 Non-overlapping Template Matching Test
```
$ ./TestRNG -r 1000 nonoverlapping -s 0.01
```
>Non-overlapping Template Matching Test\
>Call Count: 8,000\
>Significance: 0.01
<table>
<tr><th>Template Length</th><th>Pass proportion</th><th>Result</th><th>Uniformity Chi-Squared</th><th>P-Value</th><th>Result</th></tr>
<tr><td>9</td><td>0.943</td><td>FAIL</td><td>199.260</td><td>0.000000</td><td>FAIL</td></tr>
<tr><td>10</td><td>0.891</td><td>FAIL</td><td>467.100</td><td>0.000000</td><td>FAIL</td></tr>
</table>

It should be noted here that the code runs all the non-periodic templates of length 9 and 10,
combining the results into an overall p-Value using Fischer's method.  This test was then run
1,000 times to produce the above.

## 2.8 Overlapping Template Matching Test
```
$ ./TestRNG -r 1000 overlapping -s 0.01
```
>Overlapping Template Matching Test\
>Significance: 0.01\
>RESULTS:\
>Acceptable proportion of passing sequences is from 0.980561 to 0.999439\
>Observed proportion: 0.992000\
>Result: Pass\
>\
>Checking histogram for uniformity:\
>Chi-Squared: 11.220000\
>Uniformity p-Value: 0.260930\
>p-Values are uniformly distributed.

## 2.9 Maurer's "Universal Statistical" Test
```
$ ./TestRNG -r 1000 maurer -bs 6 -s 0.01
```
>Maurer's "Universal Statistical" Test\
>Significance: 0.01\
>Block Size: 2**6\
>RESULTS:\
>Acceptable proportion of passing sequences is from 0.980561 to 0.999439\
>Observed proportion: 0.989000\
>Result: Pass\
>\
>Checking histogram for uniformity:\
>Chi-Squared: 15.180000\
>Uniformity p-Value: 0.086109\
>p-Values are uniformly distributed.

The results of each block size from 6 to 16 are summarized in this table:

<table>
<tr><th>&nbsp;</th><th colspan="2">Proportion of Sequences</th><th colspan="3">Uniform Distribution of P-Values</th></tr>
<tr><th>Block Size</th><th>Passing Proportion</th><th>Result</th><th>Chi-Squared</th><th>P-Value</th><th>Result</th></tr>
<tr><td>6</td><td>0.989</td><td>PASS</td><td>15.18</td><td>0.086109</td><td>PASS</td></tr>
<tr><td>7</td><td>0.995</td><td>PASS</td><td>17.56</td><td>0.040635</td><td>PASS</td></tr>
<tr><td>8</td><td>0.992</td><td>PASS</td><td>33.78</td><td>0.000098</td><td>FAIL</td></tr>
<tr><td>9</td><td>0.998</td><td>PASS</td><td>46.58</td><td>0.000000</td><td>FAIL</td></tr>
<tr><td>10</td><td>0.998</td><td>PASS</td><td>39.28</td><td>0.000010</td><td>FAIL</td></tr>
<tr><td>11</td><td>0.998</td><td>PASS</td><td>58.70</td><td>0.000000</td><td>FAIL</td></tr>
<tr><td>12</td><td>1.000</td><td>FAIL</td><td>85.36</td><td>0.000000</td><td>FAIL</td></tr>
<tr><td>13</td><td>0.996</td><td>PASS</td><td>83.22</td><td>0.000000</td><td>FAIL</td></tr>
<tr><td>14</td><td>1.000</td><td>FAIL</td><td>117.94</td><td>0.000000</td><td>FAIL</td></tr>
<tr><td>15</td><td>1.000</td><td>FAIL</td><td>164.46</td><td>0.000000</td><td>FAIL</td></tr>
<tr><td>16</td><td>1.000</td><td>FAIL</td><td>120.06</td><td>0.000000</td><td>FAIL</td></tr>
</table>

## 2.10 Linear Complexity Test
```
$ ./TestRNG -r 1000 linear -bs 500 -bc 1000 -s 0.01
```
>Linear Complexity Test\
>Significance: 0.01\
>Block Size: 500\
>Block Count: 1000\
>RESULTS:\
>Acceptable proportion of passing sequences is from 0.980561 to 0.999439\
>Observed proportion: 0.993000\
>Result: Pass\
>\
>Checking histogram for uniformity:\
>Chi-Squared: 13.600000\
>Uniformity p-Value: 0.137282\
>p-Values are uniformly distributed.

The results for a range of block sizes from 500 to 5000 are summarized in the following table.  The block count
and repeat counts were both held at 1,000.

<table>
<tr><th>&nbsp;</th><th colspan="2">Proportion of Sequences</th><th colspan="3">Uniform Distribution of P-Values</th></tr>
<tr><th>Block Size</th><th>Passing Proportion</th><th>Result</th><th>Chi-Squared</th><th>P-Value</th><th>Result</th></tr>
<tr><td>500</td><td>0.993</td><td>PASS</td><td>13.60</td><td>0.137282</td><td>PASS</td></tr>
<tr><td>1,000</td><td>0.993</td><td>PASS</td><td>10.34</td><td>0.323668</td><td>PASS</td></tr>
<tr><td>2,000</td><td>0.987</td><td>PASS</td><td>4.04</td><td>0.908760</td><td>PASS</td></tr>
<tr><td>3,000</td><td>0.988</td><td>PASS</td><td>15.20</td><td>0.085587</td><td>PASS</td></tr>
<tr><td>4,000</td><td>0.991</td><td>PASS</td><td>5.06</td><td>0.829047</td><td>PASS</td></tr>
<tr><td>5,000</td><td>0.993</td><td>PASS</td><td>14.12</td><td>0.118120</td><td>PASS</td></tr>
</table>

## 2.11 Serial Test
```
$ ./TestRNG -r 1000 serial -bs 3 -c 1000000 -s 0.01
```
>Serial Test\
>Significance: 0.01\
>Block Size: 3\
>Call Count: 1,000,000\
>RESULTS:\
>Acceptable proportion of passing sequences is from 0.980561 to 0.999439\
>Observed proportion: 0.983000\
>Result: Pass\
>\
>First set of p-Values\
>Checking histogram for uniformity:\
>Chi-Squared: 7.560000\
>Uniformity p-Value: 0.579021\
>p-Values are uniformly distributed.\
>Second set of p-Values\
>Checking histogram for uniformity:\
>Chi-Squared: 21.160000\
>Uniformity p-Value: 0.011959\
>p-Values are uniformly distributed.

<table>
<tr><th>&nbsp;</th><th>Proportion of Sequences</th><th colspan="2">Uniform Distribution of P-Values</th></tr>
<tr><th>Block Size</th><th>Result</th><th>P-Value #1</th><th>P-Value #2</th></tr>
<tr><td>2</td><td>PASS</td><td>PASS</td><td>PASS</td></tr>
<tr><td>3</td><td>2/3</td><td>PASS</td><td>PASS</td></tr>*
<tr><td>4</td><td>2/3</td><td>PASS</td><td>PASS</td></tr>
<tr><td>5</td><td>PASS</td><td>PASS</td><td>PASS</td></tr>
<tr><td>6</td><td>PASS</td><td>PASS</td><td>PASS</td></tr>
<tr><td>7</td><td>PASS</td><td>PASS</td><td>PASS</td></tr>
<tr><td>8</td><td>2/3</td><td>PASS</td><td>PASS</td></tr>
<tr><td>9</td><td>PASS</td><td>PASS</td><td>PASS</td></tr>
<tr><td>10</td><td>PASS</td><td>PASS</td><td>PASS</td></tr>
<tr><td>11</td><td>1/3</td><td>PASS</td><td>PASS</td></tr>
<tr><td>12</td><td>PASS</td><td>PASS</td><td>PASS</td></tr>
<tr><td>13</td><td>PASS</td><td>PASS</td><td>PASS</td></tr>
<tr><td>14</td><td>1/3</td><td>PASS</td><td>PASS</td></tr>
<tr><td>15</td><td>2/3</td><td>PASS</td><td>PASS</td></tr>
<tr><td>16</td><td>PASS</td><td>PASS</td><td>PASS</td></tr>
</table>

## 2.12 Approximate Entropy Test
```
$ ./TestRNG -r 1000 entropy -bs 2 -c 1000000 -s 0.01
```
>Approximate Entropy Test\
>Significance: 0.01\
>Block Size: 2\
>Call Count: 1,000,000\
>RESULTS:\
>Acceptable proportion of passing sequences is from 0.980561 to 0.999439\
>Observed proportion: 0.994000\
>Result: Pass\
>\
>Checking histogram for uniformity:\
>Chi-Squared: 15.640000\
>Uniformity p-Value: 0.074791\
>p-Values are uniformly distributed.

For the following, the repeat count and call count were held at the values above.  For the block sizes
of 15 and 16, the call counts were automatically adjusted to 1,048,576 and 2,097,152 respectively.

<table>
<tr><th>&nbsp;</th><th colspan="2">Proportion of Sequences</th><th colspan="3">Uniform Distribution of P-Values</th></tr>
<tr><th>Block Size</th><th>Passing Proportion</th><th>Result</th><th>Chi-Squared</th><th>P-Value</th><th>Result</th></tr>
<tr><td>2</td><td>0.994</td><td>PASS</td><td>15.64</td><td>0.074791</td><td>PASS</td></tr>
<tr><td>3</td><td>0.992</td><td>PASS</td><td>7.72</td><td>0.562591</td><td>PASS</td></tr>
<tr><td>4</td><td>0.987</td><td>PASS</td><td>5.34</td><td>0.803720</td><td>PASS</td></tr>
<tr><td>5</td><td>0.989</td><td>PASS</td><td>10.24</td><td>0.331408</td><td>PASS</td></tr>
<tr><td>6</td><td>0.991</td><td>PASS</td><td>6.60</td><td>0.678686</td><td>PASS</td></tr>
<tr><td>7</td><td>0.991</td><td>PASS</td><td>4.02</td><td>0.910091</td><td>PASS</td></tr>
<tr><td>8</td><td>0.993</td><td>PASS</td><td>12.04</td><td>0.211064</td><td>PASS</td></tr>
<tr><td>9</td><td>0.986</td><td>PASS</td><td>7.44</td><td>0.591409</td><td>PASS</td></tr>
<tr><td>10</td><td>0.986</td><td>PASS</td><td>3.48</td><td>0.942198</td><td>PASS</td></tr>
<tr><td>11</td><td>0.990</td><td>PASS</td><td>12.04</td><td>0.211064</td><td>PASS</td></tr>
<tr><td>12</td><td>0.982</td><td>PASS</td><td>31.72</td><td>0.000223</td><td>PASS</td></tr>
<tr><td>13</td><td>0.980</td><td>FAIL</td><td>67.68</td><td>0.000000</td><td>FAIL</td></tr>
<tr><td>14</td><td>0.945</td><td>FAIL</td><td>625.86</td><td>0.000000</td><td>FAIL</td></tr>
<tr><td>15</td><td>0.583</td><td>FAIL</td><td>5438.00</td><td>0.000000</td><td>FAIL</td></tr>
<tr><td>16</td><td>0.239</td><td>FAIL</td><td>8131.68</td><td>0.000000</td><td>FAIL</td></tr>
</table>

## 2.13 Cumulative Sums Test
```
$ ./TestRNG -r 1000 cusum -c 1000000 -m forward -s 0.01
```
>Cumulative Sums (Cusum) Test\
>Significance: 0.01\
>Call Count: 1,000,000\
>Mode: Forward\
>RESULTS:\
>Acceptable proportion of passing sequences is from 0.980561 to 0.999439\
>Observed proportion: 0.982000\
>Result: Pass\
>\
>Checking histogram for uniformity:\
>Chi-Squared: 6.260000\
>Uniformity p-Value: 0.713641\
>p-Values are uniformly distributed.

```
$ ./TestRNG -r 1000 cusum -c 1000000 -m backward -s 0.01
```
>Cumulative Sums (Cusum) Test\
>Significance: 0.01\
>Call Count: 1,000,000\
>Mode: Backward\
>RESULTS:\
>Acceptable proportion of passing sequences is from 0.980561 to 0.999439\
>Observed proportion: 0.990000\
>Result: Pass\
>\
>Checking histogram for uniformity:\
>Chi-Squared: 7.300000\
>Uniformity p-Value: 0.605916\
>p-Values are uniformly distributed.

##  2.14 Random Excursions Test
```
./TestRNG -r 1000 excursions -c 1000000 -s 0.01
```
>Random Excursions Test\
>Significance: 0.01\
>Call Count: 1,000,000\
>RESULTS:\
>Acceptable proportion of passing sequences is from 0.980561 to 0.999439\
>Observed proportion: 0.602000\
>Result: FailTooLow\
>\
>There were 618 test runs with enough cycles.\
>For each state:\
>Minimum acceptable success proportion: 0.978\
>Maximum acceptable success proportion: 1.002
>|State | Proportion | Pass/Fail | Chi-Square |  P-Value | Pass/Fail|
>|------|------------|-----------|------------|----------|----------|
>|   -4 |      0.992 |      PASS |       3.75 | 0.927228 |      PASS|
>|   -3 |      0.992 |      PASS |       4.56 | 0.871121 |      PASS|
>|   -2 |      0.994 |      PASS |      18.25 | 0.032425 |      PASS|
>|   -1 |      0.994 |      PASS |       7.60 | 0.575037 |      PASS|
>|    1 |      0.995 |      PASS |       9.41 | 0.400232 |      PASS|
>|    2 |      0.994 |      PASS |       4.65 | 0.863390 |      PASS|
>|    3 |      0.995 |      PASS |       9.02 | 0.435186 |      PASS|
>|    4 |      0.998 |      PASS |      11.97 | 0.215140 |      PASS|

Section 3.14 contains this:
>If $J < \max(0.005\sqrt{n},500)$, the randomness hypothesis is rejected.

Therefore overall, the above test is a failure because it is only when excluding the
sequences with too few cycles that we find the remaining sequences passing.

Note that $0.005\sqrt{n} = 500$ for $n=100,000,000,000$.  To try to get a higher
proportion of runs with sufficient cycles, I tried call counts up to 1,000,000,000.
This test run is shown below.

The number of test runs with sufficient cycles was 992.  This is within the acceptable
range of 981 to 999.  The observed proportion below shows 0.955 because some runs were 
deemed as failures when the p-Values of all states were combined using Fischer's Method.

The table showing results by states is only considering the 992 test runs that survived the
test of having enough cycles.

Overall, I've deemed this test as a failure due to the insufficient number of cycles.
In the example which used the first million bits of the binary expansion of e,
we see that this has 1,490 cycles or nearly three times the minimum.  This test reveals
evidence of non-randomness.

```
$ ./TestRNG -r 1000 excursions -c 1000000000 -s 0.01
```
>Random Excursions Test\
>Significance: 0.01\
>Call Count: 1,000,000,000\
>RESULTS:\
>Acceptable proportion of passing sequences is from 0.980561 to 0.999439\
>Observed proportion: 0.955000\
>Result: FailTooLow\
>\
>There were 992 test runs with enough cycles.\
>For each state:\
>Minimum acceptable success proportion: 0.981\
>Maximum acceptable success proportion: 0.999
>|State | Proportion | Pass/Fail | Chi-Square |  P-Value | Pass/Fail|
>|------|------------|-----------|------------|----------|----------|
>|   -4 |      0.991 |      PASS |       7.88 | 0.546381 |      PASS|
>|   -3 |      0.988 |      PASS |      12.58 | 0.182718 |      PASS|
>|   -2 |      0.986 |      PASS |       4.41 | 0.882321 |      PASS|
>|   -1 |      0.992 |      PASS |      18.10 | 0.034022 |      PASS|
>|    1 |      0.991 |      PASS |       9.05 | 0.432820 |      PASS|
>|    2 |      0.997 |      PASS |       7.15 | 0.621170 |      PASS|
>|    3 |      0.989 |      PASS |       7.23 | 0.612784 |      PASS|
>|    4 |      0.988 |      PASS |       3.97 | 0.913523 |      PASS|

## 2.15 Random Excursions Variant Test
```
$ time ./TestRNG -r 1000 excursionsvariant -c 1000000 -s 0.01
```
>Random Excursions Test\
>Significance: 0.01\
>Call Count: 1,000,000\
>For each state:\
>Minimum acceptable success proportion: 0.981\
>Maximum acceptable success proportion: 0.999
>|State | Proportion | Pass/Fail | Chi-Square |  P-Value | Pass/Fail|
>|------|------------|-----------|------------|----------|----------|
>|   -9 |      0.979 |      FAIL |       8.98 | 0.439122 |      PASS|
>|   -8 |      0.981 |      PASS |      12.14 | 0.205531 |      PASS|
>|   -7 |      0.983 |      PASS |       9.94 | 0.355364 |      PASS|
>|   -6 |      0.985 |      PASS |      14.94 | 0.092597 |      PASS|
>|   -5 |      0.981 |      PASS |       7.20 | 0.616305 |      PASS|
>|   -4 |      0.991 |      PASS |       9.20 | 0.419021 |      PASS|
>|   -3 |      0.994 |      PASS |       9.04 | 0.433590 |      PASS|
>|   -2 |      0.991 |      PASS |       6.12 | 0.727851 |      PASS|
>|   -1 |      0.991 |      PASS |       9.02 | 0.435430 |      PASS|
>|    1 |      0.988 |      PASS |      15.14 | 0.087162 |      PASS|
>|    2 |      0.989 |      PASS |      18.32 | 0.031637 |      PASS|
>|    3 |      0.988 |      PASS |      11.82 | 0.223648 |      PASS|
>|    4 |      0.987 |      PASS |       5.82 | 0.757790 |      PASS|
>|    5 |      0.991 |      PASS |       3.50 | 0.941144 |      PASS|
>|    6 |      0.986 |      PASS |       6.50 | 0.689019 |      PASS|
>|    7 |      0.983 |      PASS |       7.26 | 0.610070 |      PASS|
>|    8 |      0.983 |      PASS |       7.86 | 0.548314 |      PASS|
>|    9 |      0.983 |      PASS |       6.26 | 0.713641 |      PASS|

With only one state failing the Proportion test, and by only 2 sequences,
this is judged to be a pass.

For this test, Section 3.15 states:
> The test suite code assumes $J \geq 500$.

There is no statement that a failure to meet this assumption is taken as a failure of this test.
With this proviso, this test can be seen as passing.

