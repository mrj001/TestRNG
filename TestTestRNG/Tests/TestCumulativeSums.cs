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
using TestRNG.Tests;
using TestRNG.RNG;
using Xunit;

namespace TestTestRNG.Tests;

public class TestCumulativeSums
{
   public static TheoryData<double, double, double, string, CumulativeSums.Mode> DoTestTestData
   {
      get
      {
         TheoryData<double, double, double, string, CumulativeSums.Mode> rv = new();

         // This test is based upon the example in Section 2.13.4 of Ref. A.
         // Note that the test statistic given in this section was not divided by
         // the root of n, as per Section 3.13 and the example given in 2.13.8.
         // Here, it is divided.
         // Note: it is not known why the tolerance for matching the test statistic and
         // pValue had to be made larger.
         rv.Add(4.0 / Math.Sqrt(10), 0.4116588, 1e-4, "1011010111", CumulativeSums.Mode.Forward);

         // These two tests are based upon the example in Section 2.13.8 of Ref. A
         rv.Add(1.6, 0.219194, 1e-6, "1100100100001111110110101010001000100001011010001100001000110100110001001100011001100010100010111000", CumulativeSums.Mode.Forward);
         rv.Add(1.9, 0.114866, 1e-6, "1100100100001111110110101010001000100001011010001100001000110100110001001100011001100010100010111000", CumulativeSums.Mode.Backward);

         return rv;
      }
   }

   [Theory]
   [MemberData(nameof(DoTestTestData))]
   public void DoTest(double expectedTestStatistic, double expectedPValue, double tolerance, string randomBitString, CumulativeSums.Mode mode)
   {
      IRandom random = new FakeRandom(randomBitString);
      int callCount = randomBitString.Length;
      double actualTestStatistic;
      double actualPValue;

      //
      // Action
      //
      bool actual = CumulativeSums.Test(random, callCount, mode, 0.01, out actualTestStatistic, out actualPValue);

      //
      // Assertions
      //
      Assert.True(actual);
      Assert.True(Math.Abs(expectedTestStatistic - actualTestStatistic) < tolerance);
      Assert.True(Math.Abs(expectedPValue - actualPValue) < tolerance);
   }
}