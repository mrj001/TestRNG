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

public static class Maurer
{
   public const int BLOCK_SIZE_MIN = 6;
   public const int BLOCK_SIZE_MAX = 16;
   public const int BLOCK_SIZE_DEFAULT = BLOCK_SIZE_MIN;


   // The following values are from a table in Section 2.9.4(5) of Ref. A
   private static double[] expectedValue = new double[]
   {
      0,0,         // Unused L == 0 and 1
      1.5374383,   // L == 2 to support unit testing
      0,0,0,       // unused L == 3 to 5 
      5.2177052,   // L == 6
      6.1962507,   // L == 7
      7.1836656,   // L == 8
      8.1764248,   // L == 9
      9.1723243,   // L == 10
      10.170032,   // L == 11
      11.168765,   // L == 12
      12.168070,   // L == 13
      13.167693,   // L == 14
      14.167488,   // L == 15
      15.167379    // L == 16
   };
   private static double[] variance = new double[]
   {
      0,0,     // Unused L == 0 and 1
      1.338,   // L == 2 to support unit testing
      0,0,0,   // unused L == 3 to 5 
      2.954,   // L == 6
      3.125,   // L == 7
      3.238,   // L == 8
      3.311,   // L == 9
      3.356,   // L == 10
      3.384,   // L == 11
      3.401,   // L == 12
      3.410,   // L == 13
      3.416,   // L == 14
      3.419,   // L == 15
      3.421    // L == 16
   };

   public static bool Test(IRandom random, int blockSize, double sigLevel, out double testStatistic, out double pValue)
   {
      if (blockSize < BLOCK_SIZE_MIN || blockSize > BLOCK_SIZE_MAX)
         throw new ArgumentException($"{nameof(blockSize)} == {blockSize} is not within the required range of {BLOCK_SIZE_MIN} to {BLOCK_SIZE_MAX}");

      int initializationBlockCount = 10 * (1 << blockSize);        // Q in Ref. A
      int testBlockCount = 1000 * (1 << blockSize);                // K in Ref. A

      return TestInternal(random, blockSize, initializationBlockCount, testBlockCount, sigLevel, out testStatistic, out pValue);
   }

   private static bool TestInternal(IRandom random, int blockSize, int initializationBlockCount,
            int testBlockCount, double sigLevel,
            out double testStatistic, out double pValue)
   {
      int numBlocks = initializationBlockCount + testBlockCount;   // n in Ref. A

      // An array of bit vectors, each of size blockSize
      // bit (blockSize - 1) is the first random bit, bit (blockSize -2) follows, and so on
      // down to the last random bit of the block in bit 0.
      ushort[] bitStream = new ushort[numBlocks];

      // Convert the random bit stream into an array of bit vectors
      for (int j = 0; j < numBlocks; j++)
         for (int k = blockSize - 1; k >= 0; k--)
            if (random.NextBit())
               bitStream[j] |= (ushort)(1 << k);

      // Initialize the table.
      // NOTE:  That bitVectorIndex will be zero based, not unit based as
      // in Ref. A.   Thus, if a particular pattern did not appear in the initialization
      // phase, we must have -1 in that place in order to get the correct difference.
      int[] table = new int[1 << blockSize];
      for (int j = 0; j < table.Length; j++)
         table[j] = -1;

      // Process the Initialization segment
      int bitVectorIndex = 0;
      while (bitVectorIndex < initializationBlockCount)
      {
         table[bitStream[bitVectorIndex]] = bitVectorIndex;
         bitVectorIndex++;
      }

      // Process the Test Segment
      double sum = 0.0;
      while (bitVectorIndex < numBlocks)
      {
         ushort curBitVector = bitStream[bitVectorIndex];
         sum += Math.Log2(bitVectorIndex - table[curBitVector]);
         table[curBitVector] = bitVectorIndex;

         bitVectorIndex++;
      }
      testStatistic = sum / testBlockCount;   // f_n in Ref.A

      // Find the p-Value
      pValue = Normal.ComplementaryErrorFunction(Math.Abs((testStatistic - expectedValue[blockSize]) / Math.Sqrt(2 * variance[blockSize])));

      return pValue >= sigLevel;
   }
}