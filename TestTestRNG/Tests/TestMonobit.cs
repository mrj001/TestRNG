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
using TestRNG.Tests;
using Xunit;

namespace TestTestRNG.Tests;

public class TestMonobit
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

   [Fact]
   public void Monobit()
   {
      IRandom random = new FakeRandom();
      double expectedPValue = 0.109599;
      double expectedTestStatistic = 1.6;
      double actualPValue;
      double actualTestStatistic;

      //
      // Action
      //
      bool actual = MonobitTest.Test(random, 100, 0.01, out actualTestStatistic, out actualPValue);

      //
      // Assertions
      //
      Assert.True(actual);
      Assert.True(Math.Abs(expectedTestStatistic - actualTestStatistic) < 0.000001);
      Assert.True(Math.Abs(expectedPValue - actualPValue) < 0.000001);
   }
}