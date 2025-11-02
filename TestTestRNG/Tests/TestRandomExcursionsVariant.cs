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

public class TestRandomExcursionsVariant
{
   /// <summary>
   /// This test is based upon the example in Section 2.15.4 of Ref. A
   /// </summary>
   [Fact]
   public void DoTest()
   {
      string randomBitString = "0110110101";
      int callCount = randomBitString.Length;
      IRandom random = new FakeRandom(randomBitString);
      double[]? testStatistics;
      double[]? pValues;
      double pValue;
      double expectedPValue = 0.683091;
      double tolerance = 1e-6;

      //
      // Action
      //
      bool actual = RandomExcursionsVariant.Test(random, callCount, 0.01, out testStatistics, out pValues, out pValue);

      //
      // Assertions
      //
      Assert.NotNull(testStatistics);
      Assert.NotNull(pValues);
      Assert.True(Math.Abs(expectedPValue - pValues[RandomExcursionsVariant.StateToIndex(1)]) < tolerance);
   }

   // This test is based upon the example in Section 2.15.8 of Ref. A
   [Fact]
   public void DoTest2()
   {
      IRandom random = new FakeRandomFile("TestFiles/MillionBitsOfE.gz");
      int callCount = 1_000_000;
      double sigLevel = 0.01;
      double[]? testStatistics;
      double[]? pValues;
      double pValue;
      double[] expectedTestStatistics = new double[] { 1450, 1435, 1380, 1366, 1412, 1475, 1480, 1468, 1502, 1409, 1369, 1396, 1479, 1599, 1628, 1619, 1620, 1610 };
      double[] expectedPValues = new double[] { 0.858946, 0.794755, 0.576249, 0.493417, 0.633873, 0.917283, 0.934708, 0.816012, 0.826009, 0.137861, 0.200642, 0.441254, 0.939291, 0.505683, 0.445935, 0.512207, 0.538635, 0.59393 };
      bool[] expectedConclusion = new bool[] { true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true };
      double tolerance = 1e-6;

      //
      // Action
      //
      bool actual = RandomExcursionsVariant.Test(random, callCount, sigLevel, out testStatistics, out pValues, out pValue);

      //
      // Assertions:
      //
      Assert.True(actual);
      for (int j = 0; j < expectedTestStatistics.Length; j++)
      {
         Assert.True(Math.Abs(expectedTestStatistics[j] - testStatistics![j]) < tolerance);
         Assert.True(Math.Abs(expectedPValues[j] - pValues![j]) < tolerance);
         Assert.Equal(expectedConclusion[j], pValues[j] >= sigLevel);
      }
   }
}