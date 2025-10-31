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
using TestRNG.RNG;
using TestRNG.Statistics;

namespace TestRNG.Tests;

public static class CumulativeSums
{
   public enum Mode
   {
      Forward = 0,
      Backward = 1
   }

   public const int DEFAULT_CALL_COUNT = 1_000_000;
   public const int MINIMUM_CALL_COUNT = 100;

   public static bool Test(IRandom random, int callCount, Mode mode, double sigLevel, out double testStatistic, out double pValue)
   {
      // Convert random sequence
      int[] s = new int[callCount];
      for (int j = 0; j < callCount; j++)
         s[j] = random.NextBit() ? 1 : -1;

      // Do cumulative sum
      int cusum = 0;
      int z = 0;
      if (mode == Mode.Forward)
      {
         for (int j = 0; j < callCount; j++)
         {
            cusum += s[j];
            z = Math.Max(z, Math.Abs(cusum));
         }
      }
      else
      {
         for (int j = callCount - 1; j >= 0; j--)
         {
            cusum += s[j];
            z = Math.Max(z, Math.Abs(cusum));
         }
      }

      testStatistic = z / Math.Sqrt(callCount);

      // Standard Normal Cumulative Probability Distribution Function
      // Compute p-Value
      int k, kll, kul;
      pValue = 1.0;
      kll = (int)Math.Floor((((double)-callCount) / z + 1.0) / 4.0);
      kul = (int)Math.Floor((((double)callCount) / z - 1.0) / 4.0);
      double rootN = Math.Sqrt(callCount);
      for (k = kll; k <= kul; k++)
      {
         double term1 = Normal.Gauss((4 * k + 1) * z / rootN);
         double term2 = Normal.Gauss((4 * k - 1) * z / rootN);
         pValue -= term1 - term2;
      }
      kll = (int)Math.Floor((((double)-callCount) / z - 3.0) / 4.0);
      kul = (int)Math.Floor((((double)callCount) / z - 1.0) / 4.0);
      for (k = kll; k <= kul; k++)
      {
         double term1 = Normal.Gauss((4 * k + 3) * z / rootN);
         double term2 = Normal.Gauss((4 * k + 1) * z / rootN);
         pValue += term1 - term2;
      }

      return pValue >= sigLevel;
   }
}