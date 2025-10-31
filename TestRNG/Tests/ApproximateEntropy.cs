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
using TestRNG.Statistics;

namespace TestRNG.Tests;

public static class ApproximateEntropy
{
   public const int DEFAULT_BLOCK_SIZE = 3;
   public const int MINIMUM_BLOCK_SIZE = 2;
   public const int MAXIMUM_BLOCK_SIZE = 16;
   public const int DEFAULT_CALL_COUNT = 1_000_000;

   public static bool Test(IRandom random, ref int callCount, int blockSize, double sigLevel,
            out double testStatistic, out double pValue)
   {
      // Ensure call count is large enough
      callCount = Math.Max(callCount, 1 << (blockSize + 5));

      return TestInternal(random, ref callCount, blockSize, sigLevel, out testStatistic, out pValue);
   }

   private static bool TestInternal(IRandom random, ref int callCount, int blockSize, double sigLevel,
            out double testStatistic, out double pValue)
   {
      // Copy the random sequence to a local array
      bool[] sequence = new bool[callCount + blockSize];
      for (int j = 0; j < callCount; j++)
         sequence[j] = random.NextBit();
      // copy the "m + 1" first bits to the end of the sequence.
      for (int j = 0; j < blockSize; j++)
         sequence[callCount + j] = sequence[j];

      // The first index is the added number in "m + 0", "m + 1"
      // The second index is the number formed by the "m - xxx" consecutive bits.
      int vLen = 2;
      long[][] v = new long[vLen][];
      for (int j = 0; j < vLen; j++)
         v[j] = new long[1 << (blockSize + j)];

      // Determine the frequency of all possible overlapping blockSize-bit blocks
      for (int j = 0; j < callCount; j++)
         for (int k = 0; k < vLen; k++)
         {
            // Determine the value of the current set of blockSize + k bits.
            int value = 0;
            for (int i = 0; i < blockSize + k; i++)
               if (sequence[j + i])
                  value |= 1 << i;
            // Increment this count.
            v[k][value]++;
         }

      // Compute Cim for blockSize
      double[] cim = new double[1 << blockSize];
      int jul = cim.Length;
      for (int j = 0; j < jul; j++)
         cim[j] = ((double)v[0][j]) / callCount;

      // Compute Psi^m
      double psim = 0.0;
      for (int j = 0; j < jul; j++)
         if (v[0][j] != 0)
            psim += cim[j] * Math.Log(cim[j]);

      // Compute cim for blockSize + 1
      cim = new double[1 << (blockSize + 1)];
      jul = cim.Length;
      for (int j = 0; j < jul; j++)
         cim[j] = ((double)v[1][j]) / callCount;

      // Compute Psi+m+1
      double psiMPlusOne = 0.0;
      for (int j = 0; j < jul; j++)
         if (v[1][j] != 0)
            psiMPlusOne += cim[j] * Math.Log(cim[j]);

      // Compute the test statistic
      double apEnOfM = psim - psiMPlusOne;
      testStatistic = 2 * callCount * (Math.Log(2) - apEnOfM);
      pValue = Gamma.IncompleteGammaQ(1 << (blockSize - 1), testStatistic / 2.0);

      return pValue >= sigLevel;
   }
}