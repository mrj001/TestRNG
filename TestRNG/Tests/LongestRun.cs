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
using System.Runtime.InteropServices;
using TestRNG.RNG;
using TestRNG.Statistics;

namespace TestRNG.Tests;

public enum LongestRunBlockSize : int
{
   Small = 8,

   Medium = 128,

   Large = 10_000
}

public static class LongestRun
{
   /// <summary>
   /// Performs the Longest Run of Ones in a Block Test (Section 2.4 of Ref. A)
   /// </summary>
   /// <param name="random"></param>
   /// <param name="blockSize"></param>
   /// <param name="callCount">Number of times to call the Random Number Generator.  
   /// This is subject to adjustment to a minimum acceptable value.</param>
   /// <param name="testStatistic"></param>
   /// <param name="pValue"></param>
   /// <returns></returns>
   /// <exception cref="ApplicationException"></exception>
   public static bool Test(IRandom random, LongestRunBlockSize blockSize, ref int callCount, double sigLevel, out double testStatistic, out double pValue)
   {
      switch (blockSize)
      {
         case LongestRunBlockSize.Small:
            return TestSmallBlock(random, ref callCount, sigLevel, out testStatistic, out pValue);

         case LongestRunBlockSize.Medium:
            return TestMediumBlock(random, ref callCount, sigLevel, out testStatistic, out pValue);

         case LongestRunBlockSize.Large:
            return TestLargeBlock(random, ref callCount, sigLevel, out testStatistic, out pValue);

         default:
            throw new ApplicationException();
      }
   }

   private static bool TestSmallBlock(IRandom random, ref int callCount, double sigLevel, out double testStatistic, out double pValue)
   {
      callCount = Math.Max(callCount, 128);
      int blockSize = (int)LongestRunBlockSize.Small;
      int blockCount = callCount / blockSize;
      int[] v = new int[4];

      // Calculate the contents of the v Array
      for (int j = 0; j < blockCount; j++)
      {
         int longestRun = LongestRunOfOnes(random, blockSize);
         if (longestRun <= 1)
            v[0]++;
         else if (longestRun == 2)
            v[1]++;
         else if (longestRun == 3)
            v[2]++;
         else
            v[3]++;
      }

      // Calculate the test statistic
      double[] pi = new double[] { 0.2148, 0.3672, 0.2305, 0.1875 };
      testStatistic = CalculateTestStatistic(v, pi, blockCount);

      // Calculate the p-Value
      pValue = Gamma.IncompleteGammaQ((v.Length - 1) / 2.0, testStatistic / 2.0);

      return pValue >= sigLevel;
   }

   private static bool TestMediumBlock(IRandom random, ref int callCount, double sigLevel, out double testStatistic, out double pValue)
   {
      callCount = Math.Max(callCount, 6272);
      int blockSize = (int)LongestRunBlockSize.Medium;
      int blockCount = callCount / blockSize;
      int[] v = new int[6];

      // Calculate the contents of the v Array
      for (int j = 0; j < blockCount; j++)
      {
         int longestRun = LongestRunOfOnes(random, blockSize);
         if (longestRun <= 4)
            v[0]++;
         else if (longestRun == 5)
            v[1]++;
         else if (longestRun == 6)
            v[2]++;
         else if (longestRun == 7)
            v[3]++;
         else if (longestRun == 8)
            v[4]++;
         else
            v[5]++;
      }

      // Calculate the test statistic
      double[] pi = new double[] { 0.1174, 0.2430, 0.2493, 0.1752, 0.1027, 0.1124 };
      testStatistic = CalculateTestStatistic(v, pi, blockCount);

      // Calculate the p-Value
      pValue = Gamma.IncompleteGammaQ((v.Length - 1) / 2.0, testStatistic / 2.0);

      return pValue >= sigLevel;
   }

   private static bool TestLargeBlock(IRandom random, ref int callCount, double sigLevel, out double testStatistic, out double pValue)
   {
      callCount = Math.Max(callCount, 750_000);
      int blockSize = (int)LongestRunBlockSize.Large;
      int blockCount = callCount / blockSize;
      int[] v = new int[7];

      // Calculate the contents of the v Array
      for (int j = 0; j < blockCount; j++)
      {
         int longestRun = LongestRunOfOnes(random, blockSize);
         if (longestRun <= 10)
            v[0]++;
         else if (longestRun == 11)
            v[1]++;
         else if (longestRun == 12)
            v[2]++;
         else if (longestRun == 13)
            v[3]++;
         else if (longestRun == 14)
            v[4]++;
         else if (longestRun == 15)
            v[5]++;
         else
            v[6]++;
      }

      // Calculate the test statistic
      double[] pi = new double[] { 0.0882, 0.2092, 0.2483, 0.1933, 0.1208, 0.0675, 0.0727 };
      testStatistic = CalculateTestStatistic(v, pi, blockCount);

      // Calculate the p-Value
      pValue = Gamma.IncompleteGammaQ((v.Length - 1) / 2.0, testStatistic / 2.0);

      return pValue >= sigLevel;
   }

   private static int LongestRunOfOnes(IRandom random, int blockSize)
   {
      bool last = random.NextBit();
      int curRun = last ? 1 : 0;
      int rv = curRun;
      for (int j = 1; j < blockSize; j++)
      {
         bool current = random.NextBit();
         if (last)
         {
            if (current)
            {  // continuing a run
               curRun += 1;
            }
            else
            {  // ending the current run
               rv = Math.Max(rv, curRun);
               curRun = 0;
            }
         }
         else
         {
            if (current)
            {  // starting a new run
               curRun = 1;
            }
            // else Run of zeroes: nothing to do.
         }
         last = current;
      }

      // In case the longest run is at the end of a block.
      rv = Math.Max(rv, curRun);

      return rv;
   }

   private static double CalculateTestStatistic(int[] v, double[] pi, double blockCount)
   {
      double testStatistic = 0.0;
      for (int j = 0; j < v.Length; j++)
      {
         double t1 = pi[j] * blockCount;
         double t2 = v[j] - t1;
         testStatistic += t2 * t2 / t1;
      }

      return testStatistic;
   }
}