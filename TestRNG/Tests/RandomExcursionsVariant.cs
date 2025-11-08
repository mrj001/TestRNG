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

public static class RandomExcursionsVariant
{
   public const int DEFAULT_CALL_COUNT = 1_000_000;
   public const int MINIMUM_STATE = -9;
   public const int MAXIMUM_STATE = 9;

   // The maximum and minimum are both inclusive, but zero is not a 
   // valid state.
   public const int STATE_COUNT = MAXIMUM_STATE - MINIMUM_STATE;


   public static bool Test(IRandom random, int callCount, double sigLevel,
            out double[] testStatistics, out double[] pValues,
            out double pValue)
   {
      // default output values
      pValue = 0.0;

      // Count the number of cycles and the number of times each state occurs.
      int bigJ = 0;
      int sum = 0;
      int totalStates = MAXIMUM_STATE - MINIMUM_STATE;
      int[] stateCounts = new int[totalStates];
      for (int j = 0; j < callCount; j++)
      {
         sum += random.NextBit() ? 1 : -1;
         if (sum >= MINIMUM_STATE && sum <= MAXIMUM_STATE)
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
      for (int x = MINIMUM_STATE; x <= MAXIMUM_STATE; x++)
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
      return state < 0 ? state - MINIMUM_STATE : state - MINIMUM_STATE - 1;
   }

   /// <summary>
   /// Given an index into one of the arrays of P-Values or Test Statistics, outputs the State number.
   /// </summary>
   /// <param name="stateIndex"></param>
   /// <returns></returns>
   public static int IndexToState(int stateIndex)
   {
      int rv = stateIndex + MINIMUM_STATE;
      if (rv >= 0)
         rv += 1;
      return rv;
   }
}