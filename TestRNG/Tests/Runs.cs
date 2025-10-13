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

public static class Runs
{
   public static bool Test(IRandom random, int callCount, double sigLevel, out double testStatistic, out double pValue)
   {
      // Calculate the test statistic and proportion of ones within the bit sequence.
      testStatistic = 1.0;
      bool last = random.NextBit();
      double proportionOfOnes = last ? 1.0 : 0.0;
      bool current;
      for (int j = 1; j < callCount; j++)
      {
         current = random.NextBit();
         if (current)
            proportionOfOnes += 1.0;
         if (current != last)
            testStatistic += 1.0;
         last = current;
      }
      proportionOfOnes /= callCount;

      // Should the test proceed?
      double tau = 2 / Math.Sqrt(callCount);
      if (proportionOfOnes - 0.5 >= tau)
      {
         testStatistic = double.MaxValue;
         pValue = 0.0;
         return false;
      }

      // Calculate the p-Value
      double erfcArg = Math.Abs(testStatistic - 2 * callCount * proportionOfOnes * (1.0 - proportionOfOnes)) /
              (2.0 * Math.Sqrt(2.0 * callCount) * proportionOfOnes * (1.0 - proportionOfOnes));
      pValue = Normal.ComplementaryErrorFunction(erfcArg);

      return pValue >= sigLevel;
   }
}