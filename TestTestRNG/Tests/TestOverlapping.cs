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
using System.Linq;
using System.Reflection;
using TestRNG.RNG;
using TestRNG.Tests;
using TestTestRNG.Utility;
using Xunit;

namespace TestTestRNG.Tests;

public class TestOverlapping
{
   [Fact(Skip = "Generates test file.")]
   public void GenerateMillionBitsOfE()
   {
      GenerateTestFiles.EFromSpigot(1_000_000, "TestFiles/MillionBitsOfE.gz");
   }
   
   [Fact]
   public void DoTest()
   {
      int bitsPerBlock = 1032;
      int blockCount = 1_000_000 / bitsPerBlock;
      int templateLength = 9;
      double sigLevel = 0.01;
      int[] actualV;
      double actualTestStatistic;
      double actualPValue;
      IRandom random = new FakeRandomFile("TestFiles/MillionBitsOfE.gz");
      int[] expectedV = new int[] { 329, 164, 150, 111, 78, 136 };
      double expectedTestStatistic = 8.965859;
      double expectedPValue = 0.110434;
      double tolerance = 1E-6;

      //
      // Action
      //
      bool actual = Overlapping.Test(random, blockCount, bitsPerBlock, templateLength,
               sigLevel, out actualV, out actualTestStatistic, out actualPValue);

      //
      // Assertions
      //
      Assert.True(actual);
      Assert.True(expectedV.SequenceEqual(actualV));
      Assert.True(Math.Abs(expectedTestStatistic - actualTestStatistic) < tolerance);
      Assert.True(Math.Abs(expectedPValue - actualPValue) < tolerance);
   }


   [Fact]
   public void CalculatePiHamanoKaneko()
   {
      int k = 6;
      int bitsPerBlock = 1032;
      int templateLength = 9;
      double[] expectedP = new double[] { 0.364091, 0.185659, 0.139381, 0.100571, 0.0704323, 0.139865 };
      double tolerance = 1E-6;
      MethodInfo? mi = typeof(Overlapping).GetMethod("CalculatePiHamanoKaneko", BindingFlags.Static | BindingFlags.NonPublic);

      //
      // Action
      //
      double[]? actualP = (double[]?)mi?.Invoke(null, new object[] { k, bitsPerBlock, templateLength });

      //
      // Assertions
      //
      Assert.NotNull(mi);
      Assert.NotNull(actualP);
      Assert.Equal(k, actualP.Length);
      for (int j = 0; j < k; j++)
         Assert.True(Math.Abs(expectedP[j] - actualP[j]) < tolerance);
   }
}
