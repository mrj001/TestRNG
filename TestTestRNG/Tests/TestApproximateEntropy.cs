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
using System.Reflection;
using TestRNG.RNG;
using TestRNG.Tests;
using Xunit;

namespace TestTestRNG.Tests;

public class TestApproximateEntropy
{
   public static TheoryData<double, double, string, int> DoTestTestData
   {
      get
      {
         TheoryData<double, double, string, int> rv = new();

         // This test is based upon the example in Section 2.12.4 of Ref. A
         // See the Errata for the altered test statistic.
         rv.Add(10.043859, 0.261961, "0100110101", 3);

         // This test is based upon the example in Section 2.12.8 of Ref. A
         rv.Add(5.550792, 0.235301, "1100100100001111110110101010001000100001011010001100001000110100110001001100011001100010100010111000", 2);

         return rv;
      }
   }

   [Theory]
   [MemberData(nameof(DoTestTestData))]
   public void DoTest2(double expectedTestStatistic, double expectedPValue, string randomBitString, int blockSize)
   {
      IRandom random = new FakeRandom(randomBitString);
      int callCount = randomBitString.Length;
      double actualTestStatistic;
      double actualPValue;
      double tolerance = 1E-6;
      MethodInfo? mi = typeof(ApproximateEntropy).GetMethod("TestInternal", BindingFlags.Static | BindingFlags.NonPublic);
      object[] args = new object[] { random, callCount, blockSize, 0.01, 0.0, 0.0 };

      //
      // Action:
      //
      bool? actual = (bool?)(mi?.Invoke(null, args));
      actualTestStatistic = (double)args[4];
      actualPValue = (double)args[5];

      //
      // Assertions
      //
      Assert.NotNull(mi);
      Assert.True(actual.HasValue);
      Assert.True(actual.Value);
      Assert.True(Math.Abs(expectedTestStatistic - actualTestStatistic) < tolerance);
      Assert.True(Math.Abs(expectedPValue - actualPValue) < tolerance);
   }
}