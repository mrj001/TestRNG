# Errata
This file documents errors I've found in NIST Special Publication 800-22, Revision 1a, 
A Statistical Test Suite for Random and Pseudorandom Number Generators for Cryptographic Applications

I've not found a list of errata published on the NIST website, nor any means to report errors.  If you
know of such, please file an issue to let me know.

It's also possible that some of these are, in fact, my errors.  If so, please file an issue with an 
explanation.

## Section 2.10.8
See the file RecalculateSecttion2.10.8.ods for the detailed calculations.

| Value | NIST Value | Corrected Value |
|:------|:-----------|:----------------|
| &Chi;<sup>2</sup>(obs) | 2.700348 | 2.706147 |
| P-value | 0.845406 | 0.844721 |

## Section 2.11.4
The values given in (2) for the v<sub>###</sub> values for 3-bit blocks do not add up to 10.
By inspection of the given &epsilon;', we can see that the value for v<sub>111</sub> should be 1 rather than 0.
Note that the squared values which are summed in (3) correct this, as the last value is 1 rather than 0.

In Step (5), the values given for the Upper Incomplete Gamma Function (Q(a, x), per Section 5.5.3) are 
incorrect.  In the following table, Q(a, x) was calculated with the online calculator at:
https://www.danielsoper.com/statcalc/calculator.aspx?id=23

| a | x | NIST Value | Q(a,x) |
|:--|:--|:-----------|:-------|
| 2 | 1.6 / 2 | 0.9057 | 0.808792 |
| 1 | 0.8 / 2 | 0.8805 | 0.670320 |

Note that these values match the p-Values given in Section 2.11.6 for the small example.

## Setcion 2.12
### Section 2.12.4(5) Step 4
The arguments to the log functions are replaced by the correct values divided by 10.  If you do the addition 
with the correct log arguments, you get the answer given.

### Section 2.12.4(6)
This shows
```
&Chi;<sup>2</sup> = 2 * 10 * (0.693147 - 0.190954) = 0.502193
```
However, the calculation is incorrect as 
```
0.693147 - 0.190954 = 0.502193
```

The full expression is equal to:
```
&Chi;<sup>2</sup> = 2 * 10 * (0.693147 - 0.190954) = 10.04386
```

In code, I've used the value 10.043859, which was arrived at by evaluating the above expression without
intermediate rounding to 6 decimal places.  This allowed retaining the tolerance of 10<sup>-6</sup>.

### Section 2.12.4(7)
The incorrect value for the test statistic is shown as an argument to the igamc funcdtion.  However,
the resulting p-Value is actually the one for the corrected test statistic.

