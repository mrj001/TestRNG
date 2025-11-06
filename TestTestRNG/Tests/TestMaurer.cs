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

public class TestMaurer
{
   // This test is based on the example in Section 2.9.4 of Ref. A
   [Fact]
   public void DoTest()
   {
      IRandom random = new FakeRandom("01011010011101010111");
      double actualTestStatistic;
      double actualPValue;
      double expectedPValue = 0.662374;   // See errata file.
      double expectedTestStatistic = 1.1949875;
      double tolerance = 1E-6;
      MethodInfo? mi = typeof(Maurer).GetMethod("TestInternal", BindingFlags.Static | BindingFlags.NonPublic);

      //
      // Action
      //
      object[] args = new object[]
      {
         random,
         2,        // blockSize
         4,        // initializationBlockCount
         6,        // testBlockCount
         0.01,     // sigLevel
         0.0,      // out testStatistic
         0.0       // out pValue
      };
      bool? actual = (bool?)(mi?.Invoke(null, args));
      actualTestStatistic = (double)args[5];
      actualPValue = (double)args[6];

      //
      // Assertions
      //
      Assert.NotNull(mi);
      Assert.True(actual.HasValue);
      Assert.True(actual.Value);
      Assert.True(Math.Abs(expectedTestStatistic - actualTestStatistic) < tolerance);
      Assert.True(Math.Abs(expectedPValue - actualPValue) < tolerance);
   }

   [Fact]
   public void DoTest2()
   {
      IRandom random = new FakeRandomFile("TestFiles/MillionBitsOfE.gz");
      double actualTestStatistic;
      double actualPValue;

      //
      // Action
      //
      bool actual = Maurer.Test(random, 7, 0.01, out actualTestStatistic, out actualPValue);

      //
      // Assertions
      //
      Assert.True(actual);
      Assert.True(actualPValue < 0.999);
   }
}