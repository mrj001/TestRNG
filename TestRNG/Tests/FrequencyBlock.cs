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

using TestRNG.RNG;
using TestRNG.Statistics;

namespace TestRNG.Tests;

public static class FrequencyBlock
{
   /// <summary>
   /// Performs the Frequency Test within a Block of Section 2.2 of Ref A.
   /// </summary>
   /// <param name="random"></param>
   /// <param name="blockSize"></param>
   /// <param name="blockCount"></param>
   /// <param name="sigLevel"></param>
   /// <param name="testStatistic"></param>
   /// <param name="pValue"></param>
   /// <returns></returns>
   public static bool Test(IRandom random, int blockSize, int blockCount, double sigLevel, out double testStatistic, out double pValue)
   {
      testStatistic = 0.0;

      // Calculate the test statistic
      for (int blockIndex = 0; blockIndex < blockCount; blockIndex++)
      {
         int oneCount = 0;
         for (int j = 0; j < blockSize; j++)
         {
            if (random.NextBit())
               oneCount++;
         }
         double proportion = ((double)oneCount) / blockSize;
         double ts = proportion - 0.5;
         testStatistic += ts * ts;
      }
      testStatistic *= 4.0 * blockSize;

      // Calculate the p-Value
      pValue = Gamma.IncompleteGammaQ(blockCount / 2.0, testStatistic / 2.0);

      return pValue >= sigLevel;
   }
}