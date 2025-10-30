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
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using TestRNG.RNG;
using TestRNG.Statistics;

namespace TestRNG.Tests;

public static class Serial
{
   public const int DEFAULT_CALL_COUNT = 1_000_000;
   public const int DEFAULT_BLOCK_SIZE = 3;
   public const int MINIMUM_BLOCK_SIZE = 2;
   public const int MAXIMUM_BLOCK_SIZE = 16;

   public static bool Test(IRandom random, ref int callCount, ref int blockSize, double sigLevel,
            out double testStatistic1, out double pValue1,
            out double testStatistic2, out double pValue2)
   {
      // Clamp the Block Size
      blockSize = Math.Min(MAXIMUM_BLOCK_SIZE, Math.Max(MINIMUM_BLOCK_SIZE, blockSize));

      // Ensure call count is large enough
      callCount = Math.Max(callCount, 1 << (blockSize + 2));

      return TestInternal(random, ref callCount, ref blockSize, sigLevel, out testStatistic1, out pValue1, out testStatistic2, out pValue2);
   }

   private static bool TestInternal(IRandom random, ref int callCount, ref int blockSize, double sigLevel,
            out double testStatistic1, out double pValue1,
            out double testStatistic2, out double pValue2)
   {
      // Copy the random sequence to a local array
      bool[] sequence = new bool[callCount + blockSize - 1];
      for (int j = 0; j < callCount; j++)
         sequence[j] = random.NextBit();
      // copy the "m - 1" first bits to the end of the sequence.
      for (int j = 0; j < blockSize - 1; j++)
         sequence[callCount + j] = sequence[j];

      // The first index is the subtracted number in "m - 0", "m - 1", "m - 2"
      // The second index is the number formed by the "m - xxx" consecutive bits.
      int vLen = 3;
      long[][] v = new long[vLen][];
      for (int j = 0; j < vLen; j++)
         v[j] = new long[1 << (blockSize - j)];

      // Determine the frequency of all possible overlapping blockSize-bit blocks
      for (int j = 0; j < callCount; j++)
         for (int k = 0; k < vLen; k++)
         {
            // Determine the value of the current set of blockSize - k bits.
            int value = 0;
            for (int i = 0; i < blockSize - k; i++)
               if (sequence[j + i])
                  value |= 1 << i;
            // Increment this count.
            v[k][value]++;
         }

      // Compute Psi squared sub m, sub m - 1 and sub m - 2
      double[] psiSquared = new double[vLen];
      for (int j = 0; j < vLen; j++)
      {
         for (int k = 0, kul = 1 << (blockSize - j); k < kul; k++)
            psiSquared[j] += v[j][k] * v[j][k];
         psiSquared[j] *= 1 << (blockSize - j);
         psiSquared[j] /= callCount;
         psiSquared[j] -= callCount;
      }

      // Compute the test Statistics
      double deltaPsi = psiSquared[0] - psiSquared[1];
      double delta2Psi = psiSquared[0] - 2 * psiSquared[1] + psiSquared[2];
      testStatistic1 = deltaPsi;
      testStatistic2 = delta2Psi;

      // Compute the p-Values
      double dof = 1 << (blockSize - 2);
      pValue1 = Gamma.IncompleteGammaQ(dof, deltaPsi / 2.0);
      dof = Math.Pow(2.0, blockSize - 3);
      pValue2 = Gamma.IncompleteGammaQ(dof, delta2Psi / 2.0);

      // Section 2.11.5 gives no guidance as to how to combine the two p-Values.
      // Here it is assumed that BOTH must pass.
      return pValue1 >= sigLevel && pValue2 >= sigLevel;
   }
}