# Errata
This file documents errors I've found in NIST Special Publication 800-22, Revision 1a, 
A Statistical Test Suite for Random and Pseudorandom Number Generators for Cryptographic Applications

I've not found a list of errata published on the NIST website, nor any means to report errors.  If you
know of such, please file an issue to let me know.

It's also possible that some of these are, in fact, my errors.  If so, please file an issue with an 
explanation.

## The Million Bits of e
Several of the tests use the first million bits of the binary expansion of e.  Initially, I was unable to find
a version of this that I could download.

As a result, I decided to generate the data myself.  To be confident of its accuracy, I generated it two ways.
First, I used a spigot algorithm (see `TestTestRNG/Utility/GenerateTestFiles.cs`).  Secondly, I 
did a conversion from a decimal representation found on the [NASA website](https://apod.nasa.gov/htmltest/gifcity/e.1mil).
These two versions were the same.

Additionally, some tests based on the million bits of e agreed perfectly with the NIST document.
This lends some confidence that where there are discrepancies it is either my code or the NIST
document, rather than the million bits of e used.

Later, I found a copy of the NIST reference implementation had been posted to [gitub](https://github.com/terrillmoore/NIST-Statistical-Test-Suite).  This contains a copy of the data for 1,000,000 bits of e.

## Section 2.8.8
The values for v<sub>0</sub> and v<sub>1</sub> should be 330 and 164.  I confirmed these counts
using [an independent implementation](https://github.com/dj-on-github/sp800_22_tests), 
after tweaking the values at the top of the `overlapping_template_matching_test` method in
file `sp800_22_overlapping_template_matching_test.py`.

Also, I was unable to reproduce the &Chi;<sup>2</sup>(obs) and P-Values given.  See the file
`RecalculateSection2.10.8.ods` for the detailed calculations.

| Value | NIST Value | Corrected Value |
|:------|:-----------|:----------------|
| &Chi;<sup>2</sup>(obs) | 8.965859 | 7.999866 |
| P-value | 0.110434 | 0.156243 |

## Section 2.9.4(5)
In this step, a formula is given for &sigma; as a function of the variance listed in the table.
However, in the numerical example for L = 2, the value of the variance is directly inserted
into the formula instead of calculating &sigma;.  An additional discrepancy is that the square root
of this value is taken when the formula does not show &sigma; inside the square root (only the 2).

I used 4 references (listed below) to conclude that this was an error.  Maurer's original paper 
shows a formula for &sigma; which is very similar to that in this section of the NIST reference.
References 2 & 3 both use the same formula as NIST.  It is presumed to be a more accurate approximation
arrived at after the publication of the original Maurer paper.

Python implementions (#2 & #3 below) both use the formula for &sigma; in calculating the test statistic.  
The python implemention (#4 below) uses the variance from the table directly in the formula as in this NIST section.

When I implemented this as per the numerical example, I found all practical sequences were producing p-Values
in excess of 0.999.  When I implemented this per the formulae given, I find more reasonable values for
the p-Values.  Accordingly, I chose to follow the formulae.

1. Maurer, U.M. A universal statistical test for random bit generators. J. Cryptology 5, 89–105 (1992). 
https://doi.org/10.1007/BF00193563
2. https://github.com/alexandru-stancioiu/Maurer-s-Universal-Statistical-Test/blob/master/maurer.py
3. https://github.com/dj-on-github/sp800_22_tests/blob/master/sp800_22_maurers_universal_test.py
4. https://github.com/GINARTeam/NIST-statistical-test/blob/master/09_maurers_universal_test.py
(All accessed on 2025-11-05.)

## Section 2.10.8
See the file RecalculateSection2.10.8.ods for the detailed calculations.

| Value | NIST Value | Corrected Value |
|:------|:-----------|:----------------|
| &Chi;<sup>2</sup>(obs) | 2.700348 | 2.706147 |
| P-value | 0.845406 | 0.844721 |

## Section 2.11.4
The values given in (2) for the v<sub>###</sub> values for 3-bit blocks do not add up to 10.
By inspection of the given &epsilon;', we can see that the value for v<sub>111</sub> should be 1 rather than 0.
Note that the squared values which are summed in (3) correct this, as the last value is 1 rather than 0.

In Step (5), the formulae for the p-Values are missing a denominator for the second argument.  As given, they are:

$$
\begin{align}
P-value1 = igamc(2^{m-2}, \nabla\Psi^2_m)\\
P-value2 = igamc(2^{m-3}, \nabla^2\Psi^2_m)
\end{align}
$$

When they should be:

$$
\begin{align}
P-value1 = igamc\bigg(2^{m-2}, \frac{\nabla\Psi^2_m}{2}\bigg)\\
P-value2 = igamc\bigg(2^{m-3}, \frac{\nabla^2\Psi^2_m}{2}\bigg)
\end{align}
$$

This is confirmed in the numerical example, where they show the previously calculated values divided by two.

Also in Step (5), the values given for the Upper Incomplete Gamma Function (Q(a, x), per Section 5.5.3) are 
incorrect.  In the following table, Q(a, x) was calculated with the online calculator at:
https://www.danielsoper.com/statcalc/calculator.aspx?id=23

| a | x | NIST Value | Q(a,x) |
|:--|:--|:-----------|:-------|
| 2 | 1.6 / 2 | 0.9057 | 0.808792 |
| 1 | 0.8 / 2 | 0.8805 | 0.670320 |

Note that these values match the p-Values given in Section 2.11.6 for the small example.

## Section 2.12
### Section 2.12.4(5) Step 4
The arguments to the log functions are replaced by the correct values divided by 10.  If you do the addition 
with the correct log arguments, you get the answer given.

### Section 2.12.4(6)
This shows:

$$
\begin{align}
X^2 = 2 * 10 * (0.693147 - 0.190954) = 0.502193
\end{align}
$$

However, the calculation is incorrect because:

$$
\begin{align}
0.693147 - 0.190954 = 0.502193
\end{align}
$$

The full expression is equal to:

$$
\begin{align}
X^2 = 2 * 10 * (0.693147 - 0.190954) = 10.04386
\end{align}
$$

In code, I've used the value 10.043859, which was arrived at by evaluating the above expression without
intermediate rounding to 6 decimal places.  This allowed retaining the tolerance of 10<sup>-6</sup>.

### Section 2.12.4(7)
The incorrect value for the test statistic is shown as an argument to the igamc function.  However,
the resulting p-Value is actually the one for the corrected test statistic.

## Section 2.13.4(4)
No guidance is given as to how to round off the limits of the summations.  Math.Floor and Math.Ceiling seem
most likely.  However, Math.Round might be plausible too.  After searching online, and finding this:
https://github.com/GINARTeam/NIST-statistical-test/blob/master/13_cumulative_sums_test.py
Math.Floor is chosen.

## Section 2.13.4(3)
The test statistic is given as the maximum of the absolute values of the sums.  However, this is inconsistent
with Section 3.13, which divides it by the square root of n.

Additionally, it is inconsistent with the example in Section 2.13.8 which does divide the maximum value by the 
square root of n.

## Section 2.14.4 (3) & (4)
There is a discrepancy in the counting of the sequence positions.  In the figure, the x-axis is drawn 
zero-based.  In the text, the indices are listed as unit-based indices.

In the list of zeroes of S' ("positions 3, 5, and 12") - for some reason the final appended zero is listed,
but the initial prepended zero is not  (Position 1).

## Section 2.14.8
The listed test statistics and p-Values appear to be somewhat inaccurate.  I found an independent implementation
in Python, and fed it the 1,000,000 bits of e test data.
https://github.com/dj-on-github/sp800_22_tests

After reviewing the code, and finding it to be a faithful implementation of the described test, and tweaking a 
couple probabilities (I had added a trailing 5 digit to two entries.), it produced exactly the same results as
my implementation.

<table>
<tr><th>&nbsp;</th><th colspan="2">NIST</th><th colspan="2">My Values</th></tr>
<tr><th>State=<i>x</i></th><th>X<sup>2</sup></th><th>P-value</th><th>X<sup>2</sup></th><th>P-value</th></tr>
<tr><td>4</td><td>3.835698</td><td>0.573306</td><td>3.810488</td><td>0.577011</td></tr>
<tr><td>-3</td><td>7.318707</td><td>0.197996</td><td>7.314571</td><td>0.198277</td></tr>
<tr><td>-2</td><td>7.861927</td><td>0.164011</td><td>7.841437</td><td>0.165194</td></tr>
<tr><td>-1</td><td>15.692617</td><td>0.007779</td><td>15.692617</td><td>0.007779</td></tr>
<tr><td>1</td><td>2.485906</td><td>0.778616</td><td>2.430872</td><td>0.786868</td></tr>
<tr><td>2</td><td>5.429381</td><td>0.365752</td><td>4.824346</td><td>0.437691</td></tr>
<tr><td>3</td><td>2.404171</td><td>0.790853</td><td>2.387421</td><td>0.793346</td></tr>
<tr><td>4</td><td>2.393928</td><td>0.792378</td><td>2.529995</td><td>0.771971</td></tr>
</table>
