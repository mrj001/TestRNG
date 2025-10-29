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
using System.Collections.Generic;
using System.Linq;
using TestRNG.RNG;
using TestRNG.Statistics;

namespace TestRNG.Tests;

public static class LinearComplexity
{
   public const int MINIMUM_BLOCK_COUNT = 200;
   public const int MINIMUM_BLOCK_SIZE = 500;
   public const int MAXIMUM_BLOCK_SIZE = 5000;

   public static bool Test(IRandom random, int blockSize, int blockCount, double sigLevel, out double testStatistic, out double pValue)
   {
      // callCount = blockCount * blockSize;

      // For each block
      int[] coefficients;
      int[] v = new int[7];
      for (int i = 0; i < blockCount; i++)
      {
         int li = BerlekampMassey(random, blockSize, out coefficients);

         double mu = blockSize / 2.0;
         mu += (9 + ((blockSize + 1) & 1) == 0 ? 1 : -1) / 36.0;
         mu -= (blockSize / 3.0 + 2.0 / 9.0) / Math.Pow(2.0, blockSize);

         double ti = ((blockSize & 1) == 0 ? 1 : -1) * (li - mu) + 2.0 / 9.0;

         if (ti <= -2.5)
            v[0]++;
         else if (ti <= -1.5)
            v[1]++;
         else if (ti <= -0.5)
            v[2]++;
         else if (ti <= 0.5)
            v[3]++;
         else if (ti <= 1.5)
            v[4]++;
         else if (ti <= 2.5)
            v[5]++;
         else
            v[6]++;
      }

      // BUGS
      // 1? is Berlekamp Massey correct?

      // Compute the test statistic
      testStatistic = 0.0;
      // pi is the probabilities per Sections 2.10 and 3.10 of Ref. A.
      double[] pi = new double[] { 0.010417, 0.03125, 0.125, 0.5, 0.25, 0.0625, 0.020833 };
      for (int i = 0; i < v.Length; i++)
      {
         double npi = blockCount * pi[i];
         testStatistic += (v[i] - npi) * (v[i] - npi) / npi;
      }
      pValue = Gamma.IncompleteGammaQ((v.Length - 1) / 2.0, testStatistic / 2.0);

      return pValue >= sigLevel;
   }

   //--------------------------------------------------------------------
   // References:
   //--------------------------------------------------------------------
   //
   // 1. James L. Massey, Shift-Register Synthesis and BCG Decoding,
   //    IEEE Transactions on Information Theory,
   //    Volume IT-15, No. 1, January 1969.
   //    https://crypto.stanford.edu/~mironov/cs359/massey.pdf
   //    (Accessed on 2025-10-24)
   //
   //  2. https://github.com/D3vNull41/BerlekampMasseyC/blob/main/src/bm.c
   //     (Accessed on 2025-10-24)
   //
   //  3. Erin Casey, Berlekamp-Massey Algorithm
   //     https://www-users.cse.umn.edu/~garrett/students/reu/MB_algorithm.pdf
   //     (Accessed on 2025-10-24)
   //
   //--------------------------------------------------------------------
   private static int BerlekampMassey(IRandom random, int bitCount, out int[] coefficients)
   {
      // Read the bits from the Random Stream
      int[] s = new int[bitCount];
      for (int j = 0; j < bitCount; j++)
         s[j] = random.NextBit() ? 1 : 0;

      // Step 1: Initialization
      // bitCount is n in Reference #1
      // List<int> c = new() { 1 };   // C in Reference #1
      // List<int> b = new() { 1 };   // B in Reference #1
      int[] c = new int[bitCount];    // C in Reference #1
      c[0] = 1;
      int[] b = new int[bitCount];   // B in Reference #1
      b[0] = 1;
      int x = 1;                   // x in Reference #1
                                   // A count of the bits we've advanced in the sequence since a length change.
      int l = 0;                   // L in Reference #1
                                   // This is the length of the shift register so far.
      int b1 = 1;                  // b in Reference #1
      int bitIndex = 0;            // N in Reference #1
      int m = 0;                   // Reference #1: shift register location before the last length change

      // Find first one bit
      while (s[bitIndex] == 0 && bitIndex < bitCount)
         bitIndex++;
      if (bitIndex == bitCount)
         throw new ArgumentException("Entire sequence is zeroes.");
      m = bitIndex;
      bitIndex++;
      c[bitIndex] = 1;
      l = bitIndex;

      while (bitIndex < bitCount)
      {
         int d = s[bitIndex];
         // Step 2
         // Calculate the next discrepancy.
         for (int i = 1; i <= l; i++)
            d ^= c[i] * s[bitIndex - i];

         if (d != 0)
         {
            if (2 * l > bitIndex)
            {
               // Step 4
               // Length of the register does not change
               for (int i = 0, iul = l; i <= iul; i++)
                  if (bitIndex - m + i < bitCount)
                     c[bitIndex - m + i] ^= b[i];

               // BEGIN debugging
               // recalculate discrepancy
               int newD = s[bitIndex];
               for (int i = 1; i <= l; i++)
                  newD ^= c[i] * s[bitIndex - i];
               if (newD != 0)
                  throw new ApplicationException("Annihilation violation");
               // END debugging

               x++;
            }
            else // 2 * l <= bitIndex
            {
               // Step 5: Length change
               // Copy C to temporary storage
               int[] t = (int[])c.Clone();

               int newLen = bitIndex + 1 - l;
               for (int i = 0, iul = l; i <= iul; i++)
                  if (bitIndex - m + i < bitCount)
                     c[bitIndex - m + i] ^= b[i];

               m = bitIndex;
               l = newLen;
               b = t;
               b1 = d;
               x = 0;

               // BEGIN debugging
               // recalculate discrepancy
               int newD = 0;
               for (int i = 0; i <= l; i++)
                  newD ^= c[i] * s[bitIndex - i];
               if (newD != 0)
                  throw new ApplicationException("Annihilation violation");
               // END debugging
            }
         }
         else
         {
            // Step 3
            x++;
         }

         // Step 6
         bitIndex++;
      }

      coefficients = new int[l + 1];
      for (int j = 0; j <= l; j++)
         coefficients[j] = c[j];

      return l;
   }
}