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
using System.IO;
using System.IO.Compression;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Xml.Schema;
using TestRNG.RNG;
using TestRNG.Tests;
using Xunit;

namespace TestTestRNG.Tests;

public class TestLinearComplexity
{
   [Fact]
   public void DoTest()
   {
      IRandom random = new FakeRandomFile("TestFiles/MillionBitsOfE.gz");
      double expectedTestStatistic = 2.700348;
      double actualTestStatistic;
      double expectedPValue = 0.845406;
      double actualPValue;
      double tolerance = 1E-6;
      int blockSize = 1000;
      int blockCount = 1000;
      double sigLevel = 0.01;

      //
      // Action
      //
      bool actual = LinearComplexity.Test(random, blockSize, blockCount, sigLevel, out actualTestStatistic, out actualPValue);

      //
      // Assertions:
      //
      Assert.True(actual);
      Assert.True(Math.Abs(expectedTestStatistic - actualTestStatistic) < tolerance);
      Assert.True(Math.Abs(expectedPValue - actualPValue) < tolerance);
   }

   public static TheoryData<int, string, string> BerlekampMasseyTestData
   {
      get
      {
         TheoryData<int, string, string> rv = new();

         // These two tests used under GPL from:
         // https://github.com/D3vNull41/BerlekampMasseyC/blob/main/tests/test_bm.c
         // (Accessed on 2025-10-24)
         // These were independently confirmed using:
         // https://mypage.cuhk.edu.cn/academics/wkshum/sage/Berlekamp_Massey.html
         // (Accessed on 2025-10-24)
         rv.Add(4, "11100", "10011011");
         rv.Add(3, "1111", "11001100");

         // This is from the example in Section 2.10.4(2) of Ref. A
         // confirmect using the same online calculator as above.
         rv.Add(4, "10011", "1101011110001");

         // This test case was harvested from the first 11 bits of the second block
         // of the 1,000,000 bits of e test.
         //  The correct answer was determined by the above online calculator.
         rv.Add(7, "10011011", "00100100111");

         return rv;
      }
   }

   [Theory]
   [MemberData(nameof(BerlekampMasseyTestData))]
   public void BerlekampMassey(int expectedLength, string expectedCoefficients, string bitString)
   {
      IRandom random = new FakeRandom(bitString);
      MethodInfo? mi = typeof(LinearComplexity).GetMethod("BerlekampMassey", BindingFlags.Static | BindingFlags.NonPublic);
      object?[] args = new object?[] { random, bitString.Length, null };

      //
      // Action
      //
      int? actual = (int?)(mi?.Invoke(null, args));

      //
      // Assertions
      //
      Assert.NotNull(mi);
      Assert.True(actual.HasValue);
      Assert.Equal(expectedLength, actual.Value);
      for (int j = 0; j < expectedLength; j++)
         Assert.Equal(expectedCoefficients[j], ((int[])args[2]!)[j] == 1 ? '1' : '0');
   }

   [Fact]
   public void BerlekampMassey2()
   {
      string data;
      int expectedLength = 14;
      int charsToRead = 27;     // at 26, length is 13; at 27, it should be 14.

      using (FileStream fs = new("TestFiles/MillionBitsOfE.gz", FileMode.Open, FileAccess.Read))
      using (GZipStream gz = new(fs, CompressionMode.Decompress))
      using (StreamReader sr = new(gz))
      {
         StringBuilder sb = new(charsToRead);
         for (int j = 0; j < charsToRead; j++)
            sb.Append((char)sr.Read());
         data = sb.ToString();
      }

      IRandom random = new FakeRandom(data);
      MethodInfo? mi = typeof(LinearComplexity).GetMethod("BerlekampMassey", BindingFlags.Static | BindingFlags.NonPublic);
      object?[] args = new object?[] { random, data.Length, null };

      //
      // Action
      //
      int? actual = (int?)(mi?.Invoke(null, args));

      //
      // Assertions
      //
      Assert.NotNull(mi);
      Assert.True(actual.HasValue);
      Assert.Equal(expectedLength, actual.Value);
   }
}