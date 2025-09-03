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
using System.Text;

namespace TestRNG.Statistics;

public static class ChiSquare
{
   // Reference:
   //   ACM Algorithm 299, Chi-Squared Integral, I.D. Hill, and M.C. Pike
   //   URL: https://dl.acm.org/doi/pdf/10.1145/363242.363274
   //   Accessed: 2025-09-03

   /// <summary>
   /// Finds the probability that Chi Squared on df degrees of freedom exceeds x.
   /// </summary>
   /// <param name="x">The statistic value.</param>
   /// <param name="df">The number of degrees of freedom</param>
   /// <returns></returns>
   public static double ChiProb(double x, int df)
   {
      if (x < 0)
         throw new ArgumentException($"{nameof(x)} cannot be less than zero.");
      if (df < 1)
         throw new ArgumentException($"Degrees of Freedom ({nameof(df)}) cannot be less than one.");

      bool even = (df & 1) == 0;
      double a = 0.5 * x;
      double y = 0.0;
      double s;

      if (even || df > 2)
         y = Math.Exp(-a);
      s = even ? y : 2.0 * Normal.Gauss(-Math.Sqrt(x));

      if (df > 2)
      {
         double c = 0.0;
         x = 0.5 * (df - 1);
         double z = even ? 1.0 : 0.5;

         // If bigx not implemented
         double e = even ? 1.0 : 1.0 / Math.Sqrt(Math.PI * a);
         while (z < x)
         {
            e *= a / z;
            c += e;

            z += 1.0;
         }
         return c * y + s;
      }
      else
      {
         return s;
      }
   }
}