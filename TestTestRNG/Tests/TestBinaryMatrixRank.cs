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
   /// The following bits of e were obtained from Wolfram-Alpha
   /// </summary>
   /// <remarks>
   /// The leading two bits are the integer portion.  The remaining bits
   /// are the fractional portion.
   /// </remarks>
   private const string bitsFromWolframAlpha = "1010110111111000010101000101100010100010101110110100101010011";

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

      IRandom random = EFromSpigot();
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

   // Reference:
   //  Stanley Rabinowitz and Stan Wagon,
   //  A Spigot Algorithm For The Digits Of Pi.
   //  The American Mathematical Monthly, Vol. 102, No. 3 (Mar. 1995) pp. 195-203
   // Accessed via: http://www.jstor.org/stable/2975006 on Monday, March 9, 2015.
   private static IRandom EFromSpigot()
   {
      StringBuilder rv = new(100_000);
      rv.Append("10");   // seed with the integer portion.

      // The reference indicates that n+2 mixed-radix digits are sufficient to obtain
      // n digits in base 10, with a footnote indicating that one should add a safety
      // margin of about 6, in case of a sequence of 9s.
      // We are interested in the first 100,000 bits of e.  This corresponds to
      // approximately 30,103 decimal digits.  Here, we use an absurdly large safety
      // factor.
      //
      // Each index in the array corresponds to a denominator of the mixed radix
      // which is 2 greater than the index.  (eg. index 0 corresponds to the 1/2 place;
      // index 1 to the 1/3 place, etc.)
      int numMixedRadixDigits = 30_200;
      int[] a = new int[numMixedRadixDigits];
      for (int j = 0; j < numMixedRadixDigits; j++)
         a[j] = 1;

      // Generate the first 100,000 bits of e
      while (rv.Length < 100_000)
      {
         // multiply every place by the base (2)
         for (int j = 0; j < numMixedRadixDigits; j++)
            a[j] *= 2;

         // Adjust all the digit values, except 1/2
         int carry = 0;
         for (int j = numMixedRadixDigits - 1; j > 0; j--)
         {
            int newVal = (a[j] + carry) % (j + 2);
            carry = (a[j] + carry) / (j + 2);
            a[j] = newVal;
         }

         // Adjust the 1/2 digit and generate a bit
         int nextBit = (a[0] + carry) / 2;
         if (nextBit < 0 || nextBit > 1) throw new ApplicationException();
         a[0] = (a[0] + carry) % 2;
         rv.Append(nextBit == 0 ? '0' : '1');
      }

      // Assert that the first few digits match the reference value
      for (int j = 0; j < bitsFromWolframAlpha.Length; j++)
         if (rv[j] != bitsFromWolframAlpha[j])
            throw new ApplicationException($"Mismatch in bit number {j}");

      return new FakeRandom(rv.ToString());
   }
}