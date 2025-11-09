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
using System.Runtime.CompilerServices;
using System.Text;
using TestRNG.RNG;
using TestRNG.Tests;
using Xunit;

namespace TestTestRNG.Tests;

public class TestBinaryMatrixRank
{
   /// <summary>
   /// This test replicates the example in Section 2.5.8 of Ref. A.
   /// </summary>
   [Fact]
   public void DoBinaryMatrixRank()
   {
      // The following are the values in Section 2.5.8 of Ref. A.
      // The numbers of matrices at each rank were confirmed (via debugger)
      // to match the numbers given in the example.
      // double expectedTestStatistic = 1.2619656;
      // double expectedPValue = 0.532069;
      // However, after double checking the calculation with a spreadsheet,
      // I was unable to duplicated their numbers, and came up with:
      double expectedTestStatistic = 1.2625804;
      double expectedPValue = 0.531905;

      IRandom random = new FakeRandomFile("TestFiles/MillionBitsOfE.gz");
      double tolerance = 1E-6;
      int callCount = 100_000;
      double actualTestStatistic;
      double actualPValue;
      int unusedBitCount;

      //
      // Action
      //
      bool actual = BinaryMatrixRank.Test(random, 32, ref callCount, 0.01, out actualTestStatistic, out actualPValue, out unusedBitCount);

      //
      // Assertions:
      //
      Assert.True(actual);
      Assert.True(Math.Abs(expectedTestStatistic - actualTestStatistic) < tolerance);
      Assert.True(Math.Abs(expectedPValue - actualPValue) < tolerance);
   }
}