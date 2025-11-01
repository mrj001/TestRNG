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
using Newtonsoft.Json.Serialization;
using TestRNG.RNG;
using TestRNG.Statistics;
using TestRNG.Tests;
using Xunit;

namespace TestTestRNG.Tests;

public class TestRandomExcursions
{
   [Fact]
   public void DoTest()
   {
      string randomBitString = "0110110101";
      int callCount = randomBitString.Length;
      IRandom random = new FakeRandom(randomBitString);
      double[]? testStatistics;
      double[]? pValues;
      double pValue;

      //
      // Action
      //
      bool actual = RandomExcursions.Test(random, callCount, 0.01, out testStatistics, out pValues, out pValue);

      //
      // Assertions
      //
      // Note that while this test is based upon the example in Section 2.14.4 of Ref. A
      // it is expected to fail since there will not be enough sequences.
      Assert.False(actual);
      Assert.Null(testStatistics);
      Assert.Null(pValues);
      Assert.Equal(0.0, pValue);
   }

   // This test is based upon the example in Section 2.14.8 of Ref. A
   [Fact]
   public void DoTest2()
   {
      IRandom random = new FakeRandomFile("TestFiles/MillionBitsOfE.gz");
      int callCount = 1_000_000;
      double sigLevel = 0.01;
      double[]? testStatistics;
      double[]? pValues;
      double pValue;
      double[] expectedTestStatistics = new double[] { 3.835698, 7.318707, 7.861927, 15.692617, 2.485906, 5.429381, 2.404171, 2.393928 };
      double[] expectedPValues = new double[] { 0.573306, 0.197996, 0.164011, 0.007779, 0.778616, 0.365752, 0.790853, 0.792378 };
      bool[] expectedConclusion = new bool[] { true, true, true, false, true, true, true, true };
      double tolerance = 1e-6;

      //
      // Action
      //
      bool actual = RandomExcursions.Test(random, callCount, sigLevel, out testStatistics, out pValues, out pValue);

      //
      // Assertions:
      //
      Assert.True(actual);
      for (int j = 0; j < 8; j++)
      {
         Assert.True(Math.Abs(expectedTestStatistics[j] - testStatistics![j]) < tolerance);
         Assert.True(Math.Abs(expectedPValues[j] - pValues![j]) < tolerance);
         Assert.Equal(expectedConclusion[j], pValues[j] >= sigLevel);
      }
   }
}
