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
using System.Runtime.CompilerServices;
using TestRNG;
using TestRNG.RNG;
using TestRNG.Tests;
using Xunit;

namespace TestTestRNG.Tests;

public class TestSerial
{
   // This test uses the example from Sections 2.11.4 through 2.11.6 of Ref. A
   [Fact]
   public void DoTest()
   {
      string randomBitString = "0011011101";
      IRandom random = new FakeRandom(randomBitString);
      int callCount = randomBitString.Length;
      int blockSize = 3;
      double actualTestStatistic1;
      double actualPValue1;
      double actualTestStatistic2;
      double actualPValue2;
      double expectedTestStatistic1 = 1.6;
      double expectedPValue1 = 0.808792;    // See Errata for Section 2.11.4(5)
      double expectedTestStatistic2 = 0.8;
      double expectedPValue2 = 0.670320;    // See Errata for Section 2.11.4(5)
      double tolerance = 1E-6;
      MethodInfo? mi = typeof(Serial).GetMethod("TestInternal", BindingFlags.Static | BindingFlags.NonPublic);
      object[] args = new object[] { random, callCount, blockSize, 0.01, 0.0, 0.0, 0.0, 0.0 };

      //
      // Action:
      //
      //bool actual = Serial.Test(random, ref callCount, ref blockSize, 0.01, out actualTestStatistic1, out actualPValue1, out actualTestStatistic2, out actualPValue2);
      bool? actual = (bool?)(mi?.Invoke(null, args));
      actualTestStatistic1 = (double)args[4];
      actualPValue1 = (double)args[5];
      actualTestStatistic2 = (double)args[6];
      actualPValue2 = (double)args[7];

      //
      // Assertions
      //
      Assert.NotNull(mi);
      Assert.True(actual.HasValue);
      Assert.True(actual.Value);
      Assert.True(Math.Abs(expectedTestStatistic1 - actualTestStatistic1) < tolerance);
      Assert.True(Math.Abs(expectedPValue1 - actualPValue1) < tolerance);
      Assert.True(Math.Abs(expectedTestStatistic2 - actualTestStatistic2) < tolerance);
      Assert.True(Math.Abs(expectedPValue2 - actualPValue2) < tolerance);
   }

   // This test is based upon the example from Section 2.11.8 of Ref. A
   [Fact]
   public void DoTest2()
   {
      IRandom random = new FakeRandomFile("TestFiles/MillionBitsOfE.gz");
      int callCount = 1_000_000;
      int blockSize = 2;
      double actualTestStatistic1;
      double actualPValue1;
      double actualTestStatistic2;
      double actualPValue2;
      double expectedTestStatistic1 = 0.339764;
      double expectedPValue1 = 0.843764;
      double expectedTestStatistic2 = 0.336400;
      double expectedPValue2 = 0.561915;
      double tolerance = 1E-6;

      //
      // Action:
      //
      bool actual = Serial.Test(random, ref callCount, ref blockSize, 0.01, out actualTestStatistic1, out actualPValue1, out actualTestStatistic2, out actualPValue2);

      //
      // Assertions
      //
      Assert.True(actual);
      Assert.True(Math.Abs(expectedTestStatistic1 - actualTestStatistic1) < tolerance);
      Assert.True(Math.Abs(expectedPValue1 - actualPValue1) < tolerance);
      Assert.True(Math.Abs(expectedTestStatistic2 - actualTestStatistic2) < tolerance);
      Assert.True(Math.Abs(expectedPValue2 - actualPValue2) < tolerance);
   }
}