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

/// <summary>
/// This class implements the Monobit test of Ref. A, section 2.1.
/// </summary>
public static class MonobitTest
{
   /// <summary>
   /// Runs a Monobit test on the provided Random Number Generator.
   /// </summary>
   /// <param name="rnd">The Random Number Generator implementation to test.</param>
   /// <param name="callCount">The number of times to call the Random Number Generator.</param>
   /// <param name="sigLevel">The statistical significance level at which to conduct the test.</param>
   /// <param name="testStatistic">returns the value of the Test Statistic</param>
   /// <param name="pValue">returns the calculated p-Value for the Test Statistic.</param>
   /// <returns>True if the test passes; false if the test fails.</returns>
   public static bool Test(IRandom rnd, int callCount, double sigLevel, out double testStatistic, out double pValue)
   {
      testStatistic = 0.0;

      // Calculate the Test Statistic.
      for (int j = 0; j < callCount; j++)
         testStatistic += rnd.NextBit() ? 1 : -1;
      testStatistic = Math.Abs(testStatistic) / Math.Sqrt(callCount);

      // Compute p-Value
      pValue = Normal.ComplementaryErrorFunction(testStatistic / Math.Sqrt(2.0));

      return pValue >= sigLevel;
   }
}