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
using System.Globalization;
using System.Reflection;
using TestRNG.RNG;
using TestRNG.Tests;
using Xunit;

namespace TestTestRNG.Tests;

public class TestLongestRun
{
   [Fact]
   public void Test()
   {
      string randomBits = "11001100000101010110110001001100111000000000001001001101010100010001001111010110100000001101011111001100111001101101100010110010";
      IRandom random = new FakeRandom(randomBits);
      int n = randomBits.Length;
      double expectedTestStatistic = 4.882605;
      double expectedPValue = 0.180598;
      double actualTestStatistic;
      double actualPValue;
      double tolerance = 1E-6;

      //
      // Action
      //
      bool actual = LongestRun.Test(random, LongestRunBlockSize.Small, ref n, 0.01, out actualTestStatistic, out actualPValue);

      //
      // Assertions
      //
      Assert.True(actual);
      Assert.True(Math.Abs(expectedTestStatistic - actualTestStatistic) < tolerance);
      Assert.True(Math.Abs(expectedPValue - actualPValue) < tolerance);
   }

   public static TheoryData<int, string> LongestRunOfOnesTestData
   {
      get
      {
         TheoryData<int, string> rv = new();

         // changes every bit
         rv.Add(1, "10101010");
         rv.Add(1, "01010101");

         // increasing run lengths
         rv.Add(3, "0101101110");

         // decreasing run lengths
         rv.Add(3, "0111011010");
         rv.Add(3, "11101101");

         // longest run at end of block
         rv.Add(4, "01011011101111");

         return rv;
      }
   }

   [Theory]
   [MemberData(nameof(LongestRunOfOnesTestData))]
   public void LongestRunOfOnes(int expected, string randomBits)
   {
      int blockSize = randomBits.Length;
      IRandom random = new FakeRandom(randomBits);
      MethodInfo? mi = typeof(LongestRun).GetMethod("LongestRunOfOnes", BindingFlags.Static | BindingFlags.NonPublic);

      //
      // Action
      //
      int? actualLongestRun = (int?)mi?.Invoke(null, new object[] { random, blockSize });

      //
      // Assertions
      //
      Assert.True(actualLongestRun.HasValue);
      Assert.Equal(expected, actualLongestRun.Value);
   }
}