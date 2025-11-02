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
using System.Diagnostics.CodeAnalysis;
using TestRNG.RNG;
using TestRNG.Statistics;

namespace TestRNG.Tests;

public static class RandomExcursionsVariant
{
   public const int DEFAULT_CALL_COUNT = 1_000_000;
   public const int MIN_STATE = -9;
   public const int MAX_STATE = 9;

   public static bool Test(IRandom random, int callCount, double sigLevel,
            [NotNullWhen(true)] out double[]? testStatistics, [NotNullWhen(true)] out double[]? pValues,
            out double pValue)
   {
      // default output values
      pValue = 0.0;
      testStatistics = null;
      pValues = null;

      // Count the number of cycles and the number of times each state occurs.
      int bigJ = 0;
      int sum = 0;
      int totalStates = MAX_STATE - MIN_STATE;
      int[] stateCounts = new int[totalStates];
      for (int j = 0; j < callCount; j++)
      {
         sum += random.NextBit() ? 1 : -1;
         if (sum >= MIN_STATE && sum <= MAX_STATE)
         {
            if (sum == 0)
               bigJ++;
            else
               stateCounts[StateToIndex(sum)]++;
         }
      }
      if (sum != 0)
         bigJ++;

      testStatistics = new double[totalStates];
      pValues = new double[totalStates];
      for (int j = 0; j < totalStates; j++)
         testStatistics[j] = stateCounts[j];

      // Compute the p-values
      for (int x = MIN_STATE; x <= MAX_STATE; x++)
      {
         // skip zero
         if (x == 0)
            continue;

         double num = Math.Abs(stateCounts[StateToIndex(x)] - bigJ);
         double den = Math.Sqrt(2.0 * bigJ * (4.0 * Math.Abs(x) - 2.0));
         pValues[StateToIndex(x)] = Normal.ComplementaryErrorFunction(num / den);
      }

      // Combine p-Values into one overall p-Value
      // TODO: Can the Test Statistics be meaningfully combined?
      CombinedPValues combinedPValues = new(pValues, sigLevel);
      pValue = combinedPValues.FischerPValue;

      return pValue >= sigLevel;
   }

   public static int StateToIndex(int state)
   {
      return state < 0 ? state - MIN_STATE : state - MIN_STATE - 1;
   }
}