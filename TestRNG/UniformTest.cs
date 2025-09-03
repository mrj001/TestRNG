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
using TestRNG.Statistics;

namespace TestRNG;

public static class UniformTest
{
   /// <summary>
   /// Tests for uniform distribution of the results of the Random Number Generator.
   /// </summary>
   /// <param name="binCount">The number of bins into which the random numbers are divided.</param>
   /// <param name="callCount">The number of times to call the random number generator.</param>
   /// <param name="sigLevel">The significance level to use.</param>
   /// <param name="outputFileName">The name of the output file in which to store the raw results.  
   /// These results include only  the bin counts (observed) and the expected values.  If null, no 
   /// output file is created.</param>
   public static void Test(int binCount, int callCount, double sigLevel, string? outputFileName)
   {
      int[] bins = new int[binCount];

      Random rnd = new();
      for (int j = 0; j < callCount; j++)
      {
         int value = rnd.Next(binCount);
         bins[value]++;
      }

      // Calculate the Chi Squared Test Statistic.
      double expected = (double)callCount / binCount;
      double testStat = 0;
      for (int j = 0; j < binCount; j++)
         testStat += (expected - bins[j]) * (expected - bins[j]) / expected;

      // Convert the test statistic to a probability
      double p = ChiSquare.ChiProb(testStat, binCount - 1);

      // Output the results
      Console.WriteLine("Null Hypothesis:  The RNG produces a uniform distribution.");
      Console.WriteLine("Alternate Hypothesis:  The RNG is non-uniform.");
      Console.WriteLine($"Significance Level:  {sigLevel:P}");
      Console.WriteLine($"Test Statistic:  {testStat:#,##0.000}");
      Console.WriteLine($"p-value: {p:P4}");
      if (p > sigLevel)
         Console.WriteLine("p-Value is greater than significance level: ACCEPT Null Hypothesis.");
      else
         Console.WriteLine("p-Value is less than or equal to the significance level: REJECT Null Hypothesis.");

         // Optionally, output the bin counts to a file.
      if (!string.IsNullOrEmpty(outputFileName))
         using (StreamWriter sw = new(outputFileName, false, Encoding.UTF8))
         {
            sw.WriteLine("Bin;Observed;Expected");
            for (int j = 0; j < binCount; j++)
               sw.WriteLine($"{j:N0};{bins[j]:N0};{expected:0.##}");
         }
   }
}