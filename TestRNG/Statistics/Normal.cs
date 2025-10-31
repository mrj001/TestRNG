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
using System.Security.Cryptography.X509Certificates;

namespace TestRNG.Statistics;

public static class Normal
{
   // References:
   // ACM Algorithm 209: Gauss, D. Ibbetson.
   //   URL: https://dl.acm.org/doi/pdf/10.1145/367651.367664
   //   Accessed: 2025-09-03

   /// <summary>
   /// Calculates 1/sqrt(2*pi) * integral from -infinity to x of
   /// 'Exp(-0.5*u*u) * du'
   /// by means of polynomial approximations due to A.M. Murrray of Aberdeen University.
   /// </summary>
   /// <param name="x"></param>
   /// <returns></returns>
   /// <remarks>
   /// <para>
   /// In a separate project, I compared the accuracy of this method with
   /// MathNet.Numerics.Distributions.Normal.CDF over a range from -6.0 to 6.0
   /// (inclusive), in steps of 0.1.  The maximum discrepancy between the two
   /// implementations was about 1.01E-9.
   /// </para>
   /// </remarks>
   public static double Gauss(double x)
   {
      double y;
      double z;
      double w;

      if (x == 0.0)
      {
         z = 0.0;
      }
      else
      {
         y = Math.Abs(x) / 2.0;
         if (y >= 3.0)
         {
            z = 1.0;
         }
         else if (y < 1.0)
         {
            w = y * y;
            z = ((((((((0.000124818987 * w
                     - 0.001075204047) * w
                     + 0.005198775019) * w
                     - 0.019198292004) * w
                     + 0.059054035642) * w
                     - 0.151968751364) * w
                     + 0.319152932694) * w
                     - 0.531923007300) * w
                     + 0.797884560593) * y * 2;
         }
         else
         {
            y -= 2.0;
            z = (((((((((((((-0.000045255659 * y
                     + 0.000152529290) * y
                     - 0.000019538132) * y
                     - 0.000676904986) * y
                     + 0.001390604284) * y
                     - 0.000794620820) * y
                     - 0.002034254874) * y
                     + 0.006549791214) * y
                     - 0.010557625006) * y
                     + 0.011630447319) * y
                     - 0.009279453341) * y
                     + 0.005353579108) * y
                     - 0.002141268741) * y
                     + 0.000535310849) * y
                     + 0.999936657524;
         }
      }

      if (x > 0)
         return (1 + z) / 2.0;
      else
         return (1 - z) / 2.0;
   }

   /// <summary>
   /// Calculates the Error Function erf(x).
   /// </summary>
   /// <param name="z"></param>
   /// <returns></returns>
   /// <remarks>
   /// <para>
   /// This is based upon equation 7.1.26 of Ref B.
   /// </para>
   /// </remarks>
   public static double ErrorFunction(double z)
   {
      return 1.0 - ComplementaryErrorFunction(z);
   }

   /// <summary>
   /// Calculates the Complementary Error Function (erfc(z))
   /// </summary>
   /// <param name="z"></param>
   /// <returns></returns>
   /// <remarks>
   /// <para>
   /// This is based upon equation 7.1.26 of Ref B.
   /// </para>
   /// </remarks>
   public static double ComplementaryErrorFunction(double z)
   {
      double p = 0.3275911;
      double a1 = 0.254829592;
      double a2 = -0.284496736;
      double a3 = 1.421413741;
      double a4 = -1.453152027;
      double a5 = 1.061405429;
      double t = 1.0 / (1.0 + p * z);
      double e = Math.Exp(-z * z);

      double rv = a5 * t;
      rv = (rv + a4) * t;
      rv = (rv + a3) * t;
      rv = (rv + a2) * t;
      rv = (rv + a1) * t;
      rv *= e;

      return rv;
   }
}