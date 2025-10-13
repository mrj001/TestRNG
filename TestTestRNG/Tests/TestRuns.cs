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

using TestRNG.Tests;
using TestRNG.RNG;
using Xunit;
using System;

namespace TestTestRNG.Tests;

public class TestRuns
{
   /// <summary>
   /// This FakeRandom class returns the "random" bit string used in Example 2.1.8 of Ref. A.
   /// </summary>
   private class FakeRandom : IRandom
   {
      private const string _bitString = "1100100100001111110110101010001000100001011010001100001000110100110001001100011001100010100010111000";
      private int index = 0;

      public int Next(int maxValue)
      {
         throw new System.NotImplementedException();
      }

      public bool NextBit()
      {
         // NOTE: This will throw an exception if called too many times.
         bool rv = _bitString[index] == '1';
         index++;
         return rv;
      }
   }

   /// <summary>
   /// This test uses the example in section 2.3.8 of Ref. A.
   /// </summary>
   [Fact]
   public void Runs()
   {
      IRandom random = new FakeRandom();
      double expectedTestStatistic = 52.0;
      double expectedPValue = 0.500798;
      double actualTestStatistic;
      double actualPValue;
      double tolerance = 1E-6;

      //
      // Action
      //
      bool actual = TestRNG.Tests.Runs.Test(random, 100, 0.01, out actualTestStatistic, out actualPValue);

      //
      // Assertions
      //
      Assert.True(actual);
      Assert.True(Math.Abs(expectedTestStatistic - actualTestStatistic) < tolerance);
      Assert.True(Math.Abs(expectedPValue - actualPValue) < tolerance);
   }
}