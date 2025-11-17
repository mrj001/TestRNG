# Results

I will be adding results of testing the System.Random, with a seed passed to the constructor here.

The following table is the result of many runs of each test, and evaluating the results per Sections 4.2.1 and 4.2.2 of Reference A.

| NIST | Test | Repeat Count | Proportion of Passing | Uniformity of P-Values |
|:-----|:-----|:-------------|:----------------------|:----------------------|
| 2.1 | Monobit | 1,000 | PASS | PASS |
| 2.2 | Frequency Block | 1,000 | PASS | PASS |
| 2.3 | Runs | 1,000 | PASS | PASS |
| 2.4 | Longest Run of Ones | 1,000 | PASS | PASS |
| 2.5 | Binary Matrix Rank | 1,000 | PASS | PASS |
| 2.6 | Spectral | 1,000 | PASS | FAIL |
| 2.7 | Non-overlapping Template Matching | 1,000 | FAIL | FAIL |
| 2.8 | Overlapping Template Matching | 1,000 | PASS | PASS |
| 2.9 | Maurer's "Universal Statistical" | 1,000 | FAIL | FAIL |
| 2.10 | Linear Complexity | 1,000 | PASS | PASS |
| 2.11 | Serial | 1,000 | FAIL | PASS |
| 2.12 | Approximate Entropy | 1,000 | FAIL | FAIL |
| 2.13 | Cumulative Sums | 1,000 | PASS | PASS |
| 2.14 | Random Excursions | 1,000 | FAIL | FAIL |
| 2.15 | Random Excursions Variant | 1,000 | PASS | PASS |

## Comparison with seedless System.Random

<table>
<tr><th colspan="2">&nbsp;</th><th colspan="2">Seeded System.Random</th><th colspan="2">Seedless System.Random</th></tr>
<tr><th>NIST</th><th>Test</th><th>Proportion of Passing</th><th>Uniformity of P-Values</th><th>Proportion of Passing</th><th>Uniformity of P-Values</th></tr>
<tr><td>2.1 </td><td>Monobit</td><td>PASS</td><td>PASS</td><td>PASS</td><td>PASS</td></tr>
<tr><td>2.2 </td><td>Frequency Block</td><td>PASS</td><td>PASS</td><td>PASS</td><td>PASS</td></tr>
<tr><td>2.3 </td><td>Runs</td><td>PASS</td><td>PASS</td><td>PASS</td><td>PASS</td></tr>
<tr><td>2.4 </td><td>Longest Run of Ones</td><td>PASS</td><td>PASS</td><td>PASS</td><td>PASS</td></tr>
<tr><td>2.5 </td><td>Binary Matrix Rank</td><td>PASS</td><td>PASS</td><td>PASS</td><td>PASS</td></tr>
<tr><td>2.6 </td><td>Spectral</td><td>PASS</td><td>FAIL</td><td>PASS</td><td>FAIL</td></tr>
<tr><td>2.7 </td><td>Non-overlapping Template Matching</td><td>FAIL</td><td>FAIL</td><td>FAIL</td><td>FAIL</td></tr>
<tr><td>2.8 </td><td>Overlapping Template Matching</td><td>PASS</td><td>PASS</td><td>PASS</td><td>PASS</td></tr>
<tr><td>2.9 </td><td>Maurer's "Universal Statistical"</td><td>FAIL</td><td>FAIL</td><td>FAIL</td><td>FAIL</td></tr>
<tr><td>2.10</td><td>Linear Complexity</td><td>PASS</td><td>PASS</td><td>PASS</td><td>PASS</td></tr>
<tr><td>2.11</td><td>Serial</td><td>FAIL</td><td>PASS</td><td>FAIL</td><td>PASS</td></tr>
<tr><td>2.12</td><td>Approximate Entropy</td><td>FAIL</td><td>FAIL</td><td>FAIL</td><td>FAIL</td></tr>
<tr><td>2.13</td><td>Cumulative Sums</td><td>PASS</td><td>PASS</td><td>PASS</td><td>PASS</td></tr>
<tr><td>2.14</td><td>Random Excursions</td><td>FAIL</td><td>FAIL</td><td>FAIL</td><td>FAIL</td></tr>
<tr><td>2.15</td><td>Random Excursions Variant</td><td>PASS</td><td>PASS</td><td>PASS</td><td>PASS</td></tr>
</table>

# Detailed Results
## 2.1 Monobit Test
```
$ ./TestRNG -r 1000 -rng seeded monobit --calls 1000000 -s 0.01
```

>Running Monobit test
>Call Count: 1,000,000
>Significance: 0.01
>Repeat Count: 1,000
>RESULTS:
>Acceptable proportion of passing sequences is from 0.980561 to 0.999439
>Observed proportion: 0.987000
>Result: Pass
>
>Checking histogram for uniformity:
>Chi-Squared: 2.440000
>Uniformity p-Value: 0.982454
>p-Values are uniformly distributed.

## 2.2 Frequency Block Test
```
$ ./TestRNG -r 1000 -rng seeded frequencyblock -bs 31 -bc 1000 -s 0.01
```

>Running Frequency Block Test
>Block Size: 31
>Block Count: 1000
>RESULTS:
>Acceptable proportion of passing sequences is from 0.980561 to 0.999439
>Observed proportion: 0.989000
>Result: Pass
>
>Checking histogram for uniformity:
>Chi-Squared: 8.180000
>Uniformity p-Value: 0.516113
>p-Values are uniformly distributed.

## 2.3 Runs Test
```
$ ./TestRNG -r 1000 -rng seeded  runs -c 1000000 -s 0.01
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
>Chi-Squared: 4.520000
>Uniformity p-Value: 0.873987
>p-Values are uniformly distributed.

## 2.4 Longest Run of Ones Test
```
$ ./TestRNG -r 1000 -rng seeded longestrun -bs Small -c 1000000 -s 0.01
```

>Running Longest Run of Ones Test
>Block Size: Small
>Call Count: 1,000,000
>Significance: 0.01
>RESULTS:
>Acceptable proportion of passing sequences is from 0.980561 to 0.999439
>Observed proportion: 0.991000
>Result: Pass
>
>Checking histogram for uniformity:
>Chi-Squared: 7.340000
>Uniformity p-Value: 0.601766
>p-Values are uniformly distributed.

## 2.5 Binary Matrix Rank Test
```
$ ./TestRNG -r 1000 -rng seeded matrixrank -ms 32 -c 1000000 -s 0.01
```

>Binary Matrix Rank Test
>Matrix Size: 32
>Call Count: 1,000,000
>Significance: 0.01
>Matrix Count: 976
>Unused Bit Count: 576
>RESULTS:
>Acceptable proportion of passing sequences is from 0.980561 to 0.999439
>Observed proportion: 0.982000
>Result: Pass
>
>Checking histogram for uniformity:
>Chi-Squared: 9.320000
>Uniformity p-Value: 0.408275
>p-Values are uniformly distributed.

## 2.6 Spectral Test
```
$ ./TestRNG -r 1000 -rng seeded spectral -c 1000 -s 0.01
```

>Spectral Test
>Call Count: 1,000
>Significance: 0.01
>Call Count was adjusted to 1,024
>RESULTS:
>Acceptable proportion of passing sequences is from 0.980561 to 0.999439
>Observed proportion: 0.990000
>Result: Pass
>
>Checking histogram for uniformity:
>Chi-Squared: 411.080000
>Uniformity p-Value: 0.000000
>p-Values are NOT uniformly distributed

I repeated this test (count of 1,204) and the p-Values always failed.

```
$ ./TestRNG -r 1000 -rng seeded spectral -c 2048 -s 0.01
```

>Spectral Test
>Call Count: 2,048
>Significance: 0.01
>RESULTS:
>Acceptable proportion of passing sequences is from 0.980561 to 0.999439
>Observed proportion: 0.987000
>Result: Pass
>
>Checking histogram for uniformity:
>Chi-Squared: 106.560000
>Uniformity p-Value: 0.000000
>p-Values are NOT uniformly distributed

```
./TestRNG -r 1000 -rng seeded spectral -c 4096 -s 0.01
```

>Spectral Test
>Call Count: 4,096
>Significance: 0.01
>RESULTS:
>Acceptable proportion of passing sequences is from 0.980561 to 0.999439
>Observed proportion: 0.992000
>Result: Pass
>
>Checking histogram for uniformity:
>Chi-Squared: 26.040000
>Uniformity p-Value: 0.002012
>p-Values are uniformly distributed.

Repeating this last run (count 4,096), it passed the p-Values distribution each time.

## 2.7 Non-overlapping Template Matching Test
```
$ ./TestRNG -r 1000 -rng seeded nonoverlapping -s 0.01
```

>Non-overlapping Template Matching Test
>Call Count: 8,000
>Significance: 0.01
>|Template Length | Pass proportion | Result | Uniformity Chi-Squared |  p-Value | Result |
>|----------------|-----------------|--------|------------------------|----------|-------|
>|              9 |           0.938 |   FAIL |                230.580 | 0.000000 |   FAIL |
>|             10 |           0.880 |   FAIL |                389.360 | 0.000000 |   FAIL |

## 2.8 Overlapping Template Matching Test
```
$ ./TestRNG -r 1000 -rng seeded overlapping -s 0.01
```

>Overlapping Template Matching Test
>Significance: 0.01
>RESULTS:
>Acceptable proportion of passing sequences is from 0.980561 to 0.999439
>Observed proportion: 0.990000
>Result: Pass
>
>Checking histogram for uniformity:
>Chi-Squared: 4.160000
>Uniformity p-Value: 0.900569
>p-Values are uniformly distributed.

## 2.9 Maurer's "Universal Statistical" Test
```
$ ./TestRNG -r 1000 -rng seeded maurer -bs 6 -s 0.01
```

>Maurer's "Universal Statistical" Test
>Significance: 0.01
>Block Size: 2**6
>RESULTS:
>Acceptable proportion of passing sequences is from 0.980561 to 0.999439
>Observed proportion: 0.994000
>Result: Pass
>
>Checking histogram for uniformity:
>Chi-Squared: 5.880000
>Uniformity p-Value: 0.751866
>p-Values are uniformly distributed.


The results of each block size from 6 to 16 are summarized in this table:

<table>
<tr><th>&nbsp;</th><th colspan="2">Proportion of Sequences</th><th colspan="3">Uniform Distribution of P-Values</th></tr>
<tr><th>Block Size</th><th>Passing Proportion</th><th>Result</th><th>Chi-Squared</th><th>P-Value</th><th>Result</th></tr>
<tr><td>6</td><td>0.994</td><td>PASS</td><td>5.88</td><td>0.751866</td><td>PASS</td></tr>
<tr><td>7</td><td>0.994</td><td>PASS</td><td>16.02</td><td>0.066465</td><td>PASS</td></tr>
<tr><td>8</td><td>0.996</td><td>PASS</td><td>30.00</td><td>0.000439</td><td>PASS</td></tr>
<tr><td>9</td><td>0.995</td><td>PASS</td><td>32.50</td><td>0.000163</td><td>PASS</td></tr>
<tr><td>10</td><td>0.999</td><td>PASS</td><td>50.70</td><td>0.000000</td><td>FAIL</td></tr>
<tr><td>11</td><td>1.000</td><td>FAIL</td><td>56.18</td><td>0.000000</td><td>FAIL</td></tr>
<tr><td>12</td><td>1.000</td><td>FAIL</td><td>62.34</td><td>0.000000</td><td>FAIL</td></tr>
<tr><td>13</td><td>0.999</td><td>PASS</td><td>113.98</td><td>0.000000</td><td>FAIL</td></tr>
<tr><td>14</td><td>1.000</td><td>FAIL</td><td>118.40</td><td>0.000000</td><td>FAIL</td></tr>
<tr><td>15</td><td>1.000</td><td>FAIL</td><td>131.42/td><td>0.000000</td><td>FAIL</td></tr>
<tr><td>16</td><td>1.000</td><td>FAIL</td><td>162.08</td><td>0.000000</td><td>FAIL</td></tr>
</table>

## 2.10 Linear Complexity Test
```
$ ./TestRNG -r 1000 -rng seeded linear -bs 500 -bc 1000 -s 0.01
```

The results for a range of block sizes from 500 to 5000 are summarized in the following table.  The block count
and repeat counts were both held at 1,000.

<table>
<tr><th>&nbsp;</th><th colspan="2">Proportion of Sequences</th><th colspan="3">Uniform Distribution of P-Values</th></tr>
<tr><th>Block Size</th><th>Passing Proportion</th><th>Result</th><th>Chi-Squared</th><th>P-Value</th><th>Result</th></tr>
<tr><td>500</td><td>0.990</td><td>PASS</td><td>10.48</td><td>0.313041</td><td>PASS</td></tr>
<tr><td>1000</td><td>0.990</td><td>PASS</td><td>9.80</td><td>0.366918</td><td>PASS</td></tr>
<tr><td>2000</td><td>0.987</td><td>PASS</td><td>4.42</td><td>0.881662</td><td>PASS</td></tr>
<tr><td>3000</td><td>0.985</td><td>PASS</td><td>7.10</td><td>0.626709</td><td>PASS</td></tr>
<tr><td>4000</td><td>0.983</td><td>PASS</td><td>2.54</td><td>0.979788</td><td>PASS</td></tr>
<tr><td>5000</td><td>0.994</td><td>PASS</td><td>8.02</td><td>0.532132</td><td>PASS</td></tr>
</table>

## 2.11 Serial Test
```
$ ./TestRNG -r 1000 -rng seeded serial -bs 3 -c 1000000 -s 0.01
```

>Serial Test
>Significance: 0.01
>Block Size: 3
>Call Count: 1,000,000
>RESULTS:
>Acceptable proportion of passing sequences is from 0.980561 to 0.999439
>Observed proportion: 0.986000
>Result: Pass
>
>First set of p-Values
>Checking histogram for uniformity:
>Chi-Squared: 9.400000
>Uniformity p-Value: 0.401199
>p-Values are uniformly distributed.
>Second set of p-Values
>Checking histogram for uniformity:
>Chi-Squared: 10.060000
>Uniformity p-Value: 0.345650
>p-Values are uniformly distributed.

The following table summarizes results for block sizes 2 through 16.  For each block size,
the tests were run 3 times.  The numbers in
parentheses indicate the number of times each test passed.  Two out of 3 is deemed a pass for that
test; less than 2 is deemed a failure.

<table>
<tr><th>&nbsp;</th><th>Proportion of Sequences</th><th colspan="2">Uniform Distribution of P-Values</th></tr>
<tr><th>Block Size</th><th>Result</th><th>P-Value #1</th><th>P-Value #2</th></tr>
<tr><td>2</td><td>PASS (2/3)</td><td>PASS (3/3)</td><td>PASS (3/3)</td></tr>
<tr><td>3</td><td>PASS (3/3)</td><td>PASS (3/3)</td><td>PASS (3/3)</td></tr>
<tr><td>4</td><td>PASS (2/3)</td><td>PASS (3/3)</td><td>PASS (3/3)</td></tr>
<tr><td>5</td><td>PASS (3/3)</td><td>PASS (3/3)</td><td>PASS (3/3)</td></tr>
<tr><td>6</td><td>FAIL (1/3)</td><td>PASS (3/3)</td><td>PASS (3/3)</td></tr>
<tr><td>7</td><td>FAIL (1/3)</td><td>PASS (3/3)</td><td>PASS (3/3)</td></tr>
<tr><td>8</td><td>PASS (2/3)</td><td>PASS (3/3)</td><td>PASS (3/3)</td></tr>
<tr><td>9</td><td>PASS (2/3)</td><td>PASS (3/3)</td><td>PASS (3/3)</td></tr>
<tr><td>10</td><td>PASS (3/3)</td><td>PASS (3/3)</td><td>PASS (3/3)</td></tr>
<tr><td>11</td><td>PASS (3/3)</td><td>PASS (3/3)</td><td>PASS (3/3)</td></tr>
<tr><td>12</td><td>PASS (3/3)</td><td>PASS (3/3)</td><td>PASS (3/3)</td></tr>
<tr><td>13</td><td>PASS (3/3)</td><td>PASS (3/3)</td><td>PASS (3/3)</td></tr>
<tr><td>14</td><td>FAIL (1/3)</td><td>PASS (3/3)</td><td>PASS (3/3)</td></tr>
<tr><td>15</td><td>PASS (3/3)</td><td>PASS (3/3)</td><td>PASS (3/3)</td></tr>
<tr><td>16</td><td>PASS (3/3)</td><td>PASS (3/3)</td><td>PASS (3/3)</td></tr>
</table>

This is deemed to fail the Proportion of Sequences test because out of 15 block sizes, 3 only achieved 1 out of 3 successes.
As 1 out of 3 is deemed a failure, this is a 20% failure rate, which is too high.

## 2.12 Approximate Entropy Test
```
./TestRNG -r 1000 -rng seeded entropy -bs 2 -c 1000000 -s 0.01
```

>Approximate Entropy Test
>Significance: 0.01
>Block Size: 2
>Call Count: 1,000,000
>RESULTS:
>Acceptable proportion of passing sequences is from 0.980561 to 0.999439
>Observed proportion: 0.992000
>Result: Pass
>
>Checking histogram for uniformity:
>Chi-Squared: 7.540000
>Uniformity p-Value: 0.581082
>p-Values are uniformly distributed.


For the following, the repeat count and call count were held at the values above.  For the block sizes
of 15 and 16, the call counts were automatically adjusted to 1,048,576 and 2,097,152 respectively.

<table>
<tr><th>&nbsp;</th><th colspan="2">Proportion of Sequences</th><th colspan="3">Uniform Distribution of P-Values</th></tr>
<tr><th>Block Size</th><th>Passing Proportion</th><th>Result</th><th>Chi-Squared</th><th>P-Value</th><th>Result</th></tr>
<tr><td>2</td><td>0.992</td><td>PASS</td><td>7.54</td><td>0.581082</td><td>PASS</td></tr>
<tr><td>3</td><td>0.985</td><td>PASS</td><td>6.98</td><td>0.639202</td><td>PASS</td></tr>
<tr><td>4</td><td>0.992</td><td>PASS</td><td>14.46</td><td>0.106877</td><td>PASS</td></tr>
<tr><td>5</td><td>0.994</td><td>PASS</td><td>9.80</td><td>0.366918</td><td>PASS</td></tr>
<tr><td>6</td><td>0.991</td><td>PASS</td><td>5.50</td><td>0.788728</td><td>PASS</td></tr>
<tr><td>7</td><td>0.993</td><td>PASS</td><td>8.64</td><td>0.471146</td><td>PASS</td></tr>
<tr><td>8</td><td>0.993</td><td>PASS</td><td>8.16</td><td>0.518106</td><td>PASS</td></tr>
<tr><td>9</td><td>0.986</td><td>PASS</td><td>6.54</td><td>0.684890</td><td>PASS</td></tr>
<tr><td>10</td><td>0.992</td><td>PASS</td><td>10.20</td><td>0.334538</td><td>PASS</td></tr>
<tr><td>11</td><td>0.991</td><td>PASS</td><td>16.28</td><td>0.061260</td><td>PASS</td></tr>
<tr><td>12</td><td>0.988</td><td>PASS</td><td>8.74</td><td>0.461612</td><td>PASS</td></tr>
<tr><td>13</td><td>0.977</td><td>FAIL</td><td>128.6</td><td>0.000000</td><td>FAIL</td></tr>
<tr><td>14</td><td>0.946</td><td>FAIL</td><td>620.26</td><td>0.000000</td><td>FAIL</td></tr>
<tr><td>15</td><td>0.565</td><td>FAIL</td><td>5756.86</td><td>0.000000</td><td>FAIL</td></tr>
<tr><td>16</td><td>0.226</td><td>FAIL</td><td>8471.88</td><td>0.000000</td><td>FAIL</td></tr>
</table>

## 2.13 Cumulative Sums Test
```
$ ./TestRNG -r 1000 -rng seeded cusum -c 1000000 -m forward -s 0.01
```

>Cumulative Sums (Cusum) Test
>Significance: 0.01
>Call Count: 1,000,000
>Mode: Forward
>RESULTS:
>Acceptable proportion of passing sequences is from 0.980561 to 0.999439
>Observed proportion: 0.990000
>Result: Pass
>
>Checking histogram for uniformity:
>Chi-Squared: 12.300000
>Uniformity p-Value: 0.196920
>p-Values are uniformly distributed.

```
$ ./TestRNG -r 1000 -rng seeded cusum -c 1000000 -m backward -s 0.01
```

>Cumulative Sums (Cusum) Test
>Significance: 0.01
>Call Count: 1,000,000
>Mode: Backward
>RESULTS:
>Acceptable proportion of passing sequences is from 0.980561 to 0.999439
>Observed proportion: 0.993000
>Result: Pass
>
>Checking histogram for uniformity:
>Chi-Squared: 7.400000
>Uniformity p-Value: 0.595549
>p-Values are uniformly distributed.

##  2.14 Random Excursions Test
```
$ ./TestRNG -r 1000 -rng seeded excursions -c 1000000 -s 0.01
```
>Random Excursions Test
>Significance: 0.01
>Call Count: 1,000,000
>RESULTS:
>Acceptable proportion of passing sequences is from 0.980561 to 0.999439
>Observed proportion: 0.576000
>Result: FailTooLow
>
>There were 610 test runs with enough cycles.
>For each state:
>Minimum acceptable success proportion: 0.978
>Maximum acceptable success proportion: 1.002
>|State | Proportion | Pass/Fail | Chi-Square |  P-Value | Pass/Fail |
>|------|------------|-----------|------------|----------|-----------|
>|   -4 |      0.990 |      PASS |      12.95 | 0.164844 |      PASS |
>|   -3 |      0.992 |      PASS |      17.11 | 0.046949 |      PASS |
>|   -2 |      0.992 |      PASS |      12.59 | 0.182044 |      PASS |
>|   -1 |      0.985 |      PASS |       4.36 | 0.886114 |      PASS |
>|    1 |      0.979 |      PASS |       9.15 | 0.423768 |      PASS |
>|    2 |      0.979 |      PASS |       8.23 | 0.511193 |      PASS |
>|    3 |      0.979 |      PASS |       6.98 | 0.638826 |      PASS |
>|    4 |      0.979 |      PASS |       8.33 | 0.501472 |      PASS |

Section 3.14 contains this:
>If $J < \max(0.005\sqrt{n},500)$, the randomness hypothesis is rejected.

Therefore overall, the above test is a failure because it is only when excluding the
sequences with too few cycles that we find the remaining sequences passing.

For this PRNG, I did not try much larger numbers of call counts due to the
time required for such runs.

This test is again deemed a failure due to the low number of cycles.

## 2.15 Random Excursions Variant Test
```
$ ./TestRNG -r 1000 -rng seeded excursionsvariant -c 1000000 -s 0.01
```

>Random Excursions Test
>Significance: 0.01
>Call Count: 1,000,000
>For each state:
>Minimum acceptable success proportion: 0.981
>Maximum acceptable success proportion: 0.999
>|State | Proportion | Pass/Fail | Chi-Square |  P-Value | Pass/Fail |
>|------|------------|-----------|------------|----------|-----------|
>|   -9 |      0.983 |      PASS |       5.60 | 0.779188 |      PASS |
>|   -8 |      0.984 |      PASS |       3.52 | 0.940080 |      PASS |
>|   -7 |      0.984 |      PASS |       6.96 | 0.641284 |      PASS |
>|   -6 |      0.987 |      PASS |       3.96 | 0.914025 |      PASS |
>|   -5 |      0.988 |      PASS |       8.90 | 0.446556 |      PASS |
>|   -4 |      0.991 |      PASS |       4.98 | 0.836048 |      PASS |
>|   -3 |      0.991 |      PASS |      10.78 | 0.291091 |      PASS |
>|   -2 |      0.993 |      PASS |      15.48 | 0.078567 |      PASS |
>|   -1 |      0.990 |      PASS |       5.62 | 0.777265 |      PASS |
>|    1 |      0.993 |      PASS |       6.12 | 0.727851 |      PASS |
>|    2 |      0.988 |      PASS |       9.00 | 0.437274 |      PASS |
>|    3 |      0.988 |      PASS |       2.08 | 0.990138 |      PASS |
>|    4 |      0.985 |      PASS |       5.12 | 0.823725 |      PASS |
>|    5 |      0.986 |      PASS |      17.88 | 0.036592 |      PASS |
>|    6 |      0.987 |      PASS |       6.56 | 0.682823 |      PASS |
>|    7 |      0.987 |      PASS |       5.74 | 0.765632 |      PASS |
>|    8 |      0.986 |      PASS |      12.44 | 0.189625 |      PASS |
>|    9 |      0.987 |      PASS |      11.72 | 0.229559 |      PASS |


For this test, Section 3.15 states:
> The test suite code assumes $J \geq 500$.

There is no statement that a failure to meet this assumption is taken as a failure of this test.
Since the Random Excursions Test showed many sequences failing to meet this criterion, it is safe
to assume that they do for this test, as well.  With these provisos, this test can be seen as passing.
