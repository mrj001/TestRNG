// Copyright 2025 Mark Johnson
// 
// This file is part of TestRNGSln.
// 
// TestRNGSln is free software: you can redistribute it and/or modify it under the
// terms of the GNU General Public License as published by the Free Software
// Foundation, either version 3 of the License, or (at your option) any later
// version.
// 
// TestRNGSln is distributed in the hope that it will be useful, but WITHOUT ANY
// WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR
// A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with
// TestRNGSln. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Runtime;

namespace TestRNG.Statistics;

/// <summary>
/// Gamma functions
/// </summary>
/// <remarks>
/// <para>
/// The methods in the class are based upon chapter 6 of Ref. C.
/// </para>
/// </remarks>
public static class Gamma
{
   /// <summary>
   /// Returns the incomplete gamma function Q(a, x) ≡ 1 − P(a, x).
   /// </summary>
   /// <param name="a"></param>
   /// <param name="x"></param>
   /// <returns></returns>
   public static double IncompleteGammaQ(double a, double x)
   {
      if (x < 0.0 || a <= 0.0)
         throw new ArgumentException("Invalid arguments in routine gammq");

      if (x < (a + 1.0))
         return 1.0 - IncompleteGammaPBySeries(a, x);
      else
         return IncompleteGammaQByContinuedFraction(a, x);
   }

   /// <summary>
   /// Specifies the maximum number of iterations to use in the series representation.
   /// </summary>
   private const int ITMAX = 1_000;

   private const double EPS = 3E-7;

   /// <summary>
   /// Returns the incomplete gamma function P(a, x) evaluated by its series representation
   /// </summary>
   /// <param name="a"></param>
   /// <param name="x"></param>
   private static double IncompleteGammaPBySeries(double a, double x)
   {
      if (x < 0.0)
         throw new ArgumentException($"{nameof(x)} is less than 0 in function {nameof(IncompleteGammaPBySeries)}");

      if (x == 0.0)
      {
         return 0.0;
      }
      else  // x > 0.0
      {
         double gln = GammaLn(a);
         double ap = a;
         double sum = 1.0 / a;
         double del = sum;

         for (int n = 1; n <= ITMAX; n++)
         {
            ++ap;
            del *= x / ap;
            sum += del;
            if (Math.Abs(del) < Math.Abs(sum) * EPS)
               return sum * Math.Exp(-x + a * Math.Log(x) - gln);
         }
         throw new ApplicationException($"{nameof(a)} = {a} is too large or {nameof(ITMAX)} = {ITMAX} is too small in routine {nameof(IncompleteGammaPBySeries)}");
      }
   }

   /// <summary>
   /// Ref. C says this is "A number near the smallest representable floating point number."
   /// </summary>
   /// <remarks>
   /// <para>
   /// The value in Ref. C of 1E-30 clearly indicates that they are using single precision floating
   /// point numbers.  No guidance is given as to how close to the minimum representable
   /// value it should be.
   /// </para>
   /// <para>
   /// Single.Epsilon is approximately 1.4E-45, which is 15 orders of magnitude smaller than the
   /// value used by Ref. C.  It would seem that it does not have to be especially close to the
   /// minimum value.  So we've chosen 1E-300 when Double.Epsilon is about 5E-324.
   /// </para>
   /// </remarks>
   private const double FPMIN = 1E-300;

   /// <summary>
   /// Returns the incomplete gamma function Q(a, x) evaluated by its continued fraction representation
   /// </summary>
   /// <param name="a"></param>
   /// <param name="x"></param>
   /// <returns></returns>
   private static double IncompleteGammaQByContinuedFraction(double a, double x)
   {
      double an, b, c, d, del, h;
      double gln = GammaLn(a);

      b = x + 1.0 - a;
      c = 1.0 / FPMIN;
      d = 1.0 / b;
      h = d;
      for (int i = 1; i <= ITMAX; i++)
      {
         an = -i * (i - a);
         b += 2.0;
         d = an * d + b;
         if (Math.Abs(d) < FPMIN)
            d = FPMIN;

         c = b + an / c;
         if (Math.Abs(c) < FPMIN)
            c = FPMIN;

         d = 1.0 / d;
         del = d * c;
         h *= del;
         if (Math.Abs(del - 1.0) < EPS)
            return Math.Exp(-x + a * Math.Log(x) - gln) * h;
      }

      throw new ApplicationException($"{nameof(a)} too large, or {nameof(ITMAX)} too small in {nameof(IncompleteGammaQByContinuedFraction)}");
   }

   /// <summary>
   /// The coefficients used by the GammaLn function.
   /// </summary>
   private static double[] cof = { 76.18009172947146, -86.50532032941677,
            24.01409824083091, -1.231739572450155,
            0.1208650973866179e-2, -0.5395239384953e-5};

   /// <summary>
   /// The square root of 2 * pi.
   /// </summary>
   private static double RootTwoPi = Math.Sqrt(2.0 * Math.PI);

   /// <summary>
   /// Implements the gammln function from Ref. C.
   /// </summary>
   /// <param name="xx"></param>
   /// <returns></returns>
   private static double GammaLn(double xx)
   {
      double x, y, tmp, ser;
      int j;

      y = xx;
      x = xx;
      tmp = x + 5.5;
      tmp -= (x + 0.5) * Math.Log(tmp);
      ser = 1.000000000190015;
      for (j = 0; j <= 5; j++)
         ser += cof[j] / ++y;

      return -tmp + Math.Log(RootTwoPi * ser / x);
   }

}