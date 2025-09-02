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
using System.ComponentModel;
using System.IO;
using System.Text;

namespace TestRNG;

public static class UniformTest
{
   /// <summary>
   /// Tests for uniform distribution of the results of the Random Number Generator.
   /// </summary>
   /// <param name="binCount">The number of bins into which the random numbers are divided.</param>
   /// <param name="callCount">The number of times to call the random number generator.</param>
   /// <param name="outputFileName">The name of the output file in which to store results.</param>
   public static void Test(int binCount, int callCount, string outputFileName)
   {
      int[] bins = new int[binCount];

      Random rnd = new();
      for (int j = 0; j < callCount; j++)
      {
         int value = rnd.Next(binCount);
         bins[value]++;
      }

      double expected = (double)callCount / binCount;
      using (StreamWriter sw = new(outputFileName, false, Encoding.UTF8))
      {
         sw.WriteLine("Bin;Observed;Expected");
         for (int j = 0; j < binCount; j++)
            sw.WriteLine($"{j:N0};{bins[j]:N0};{expected:0.##}");
      }
   }
}