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
using TestRNG.Tests;
using TestRNG.Utility;

namespace TestRNG;

public class Program
{
   public static void Main(string[] args)
   {
      CommandLineArgs? clArgs = CommandLineArgs.ParseCommandLineArgs(args, Console.Error);
      if (clArgs is null)
         return;

      IRandom random = new SystemRandom();

      switch (clArgs.SelectedTest)
      {
         case TestSelector.Uniform:
            DoUniformTest(random, clArgs);
            break;

         case TestSelector.Monobit:
            DoMonobitTest(random, clArgs);
            break;

         case TestSelector.FrequencyBlock:
            DoFrequencyBlockTest(random, clArgs);
            break;

         case TestSelector.Runs:
            DoRunsTest(random, clArgs);
            break;

         case TestSelector.LongestRun:
            DoLongestRunTest(random, clArgs);
            break;

         case TestSelector.MatrixRank:
            DoBinaryMatrixRankTest(random, clArgs);
            break;

         case TestSelector.Spectral:
            DoSpectralTest(random, clArgs);
            break;

         case TestSelector.NonOverlapping:
            DoNonoverlappingTest(random, clArgs);
            break;

         case TestSelector.Overlapping:
            DoOverlappingTest(random, clArgs);
            break;

         case TestSelector.Maurer:
            DoMaurerTest(random, clArgs);
            break;

         case TestSelector.Linear:
            DoLinearComplexityTest(random, clArgs);
            break;

         case TestSelector.Serial:
            DoSerialTest(random, clArgs);
            break;

         case TestSelector.Entropy:
            DoApproximateEntropyTest(random, clArgs);
            break;

         case TestSelector.Cusum:
            DoCusumTest(random, clArgs);
            break;

         case TestSelector.Excursions:
            DoRandomExcursionsTest(random, clArgs);
            break;

         case TestSelector.ExcursionsVariant:
            DoRandomExcursionsVariantTest(random, clArgs);
            break;

         default:
            break;
      }
   }

   private static void DoUniformTest(IRandom random, CommandLineArgs args)
   {
      Console.WriteLine("Running Uniform test");
      if (!string.IsNullOrEmpty(args.OutputFileName))
         Console.WriteLine($"Output file: {args.OutputFileName}");
      else
         Console.WriteLine("No output file selected.");

      Console.WriteLine($"Bin Count: {args.BinCount:N0}");
      Console.WriteLine($"Call Count: {args.CallCount:N0}");
      Console.WriteLine($"Significance: {args.Significance}");

      UniformTest.Test(random, args.BinCount, args.CallCount, args.Significance, args.OutputFileName);
   }

   private static void DoMonobitTest(IRandom random, CommandLineArgs args)
   {
      Console.WriteLine("Running Monobit test");
      Console.WriteLine($"Call Count: {args.CallCount:N0}");
      Console.WriteLine($"Significance: {args.Significance}");
      if (args.RepeatCount > 1)
         Console.WriteLine($"Repeat Count: {args.RepeatCount:N0}");

      if (args.RepeatCount == 1)
      {
         double testStatistic, pValue;
         bool result = MonobitTest.Test(random, args.CallCount, args.Significance, out testStatistic, out pValue);

         Console.WriteLine($"Test Statistic: {testStatistic}");
         Console.WriteLine($"p-Value: {pValue}");
         Console.WriteLine("Null hypothesis is {0}.", result ? "ACCEPTED" : "REJECTED");
      }
      else
      {
         double[] testStatistic = new double[args.RepeatCount];
         double[] pValues = new double[args.RepeatCount];
         bool[] results = new bool[args.RepeatCount];
         for (int j = 0; j < args.RepeatCount; j++)
            results[j] = MonobitTest.Test(random, args.CallCount, args.Significance, out testStatistic[j], out pValues[j]);

         Combining.CombinePassingResults(Console.Out, results, args.Significance);
         Combining.CheckHistogramOfPValues(Console.Out, pValues, args.Significance);
      }
   }

   private static void DoFrequencyBlockTest(IRandom random, CommandLineArgs clArgs)
   {
      Console.WriteLine("Running Frequency Block Test");
      Console.WriteLine($"Block Size: {clArgs.BlockSize}");
      Console.WriteLine($"Block Count: {clArgs.BlockCount}");

      if (clArgs.RepeatCount == 1)
      {
         double testStatistic, pValue;
         bool result = FrequencyBlock.Test(random, clArgs.BlockSize, clArgs.BlockCount, clArgs.Significance, out testStatistic, out pValue);

         Console.WriteLine($"Test Statistic: {testStatistic}");
         Console.WriteLine($"p-Value: {pValue}");
         Console.WriteLine("Null hypothesis is {0}.", result ? "ACCEPTED" : "REJECTED");
      }
      else
      {
         double[] testStatistic = new double[clArgs.RepeatCount];
         double[] pValues = new double[clArgs.RepeatCount];
         bool[] results = new bool[clArgs.RepeatCount];
         for (int j = 0; j < clArgs.RepeatCount; j++)
            results[j] = FrequencyBlock.Test(random, clArgs.BlockSize, clArgs.BlockCount, clArgs.Significance, out testStatistic[j], out pValues[j]);

         Combining.CombinePassingResults(Console.Out, results, clArgs.Significance);
         Combining.CheckHistogramOfPValues(Console.Out, pValues, clArgs.Significance);
      }
   }

   private static void DoRunsTest(IRandom random, CommandLineArgs clArgs)
   {
      Console.WriteLine("Running Runs Test");
      Console.WriteLine($"Call Count: {clArgs.CallCount:N0}");
      Console.WriteLine($"Significance: {clArgs.Significance}");

      if (clArgs.RepeatCount == 1)
      {
         double testStatistic, pValue;
         bool result = Runs.Test(random, clArgs.CallCount, clArgs.Significance, out testStatistic, out pValue);

         Console.WriteLine($"Test Statistic: {testStatistic}");
         Console.WriteLine($"p-Value: {pValue}");
         Console.WriteLine("Null hypothesis is {0}.", result ? "ACCEPTED" : "REJECTED");
      }
      else
      {
         double[] testStatistic = new double[clArgs.RepeatCount];
         double[] pValues = new double[clArgs.RepeatCount];
         bool[] results = new bool[clArgs.RepeatCount];
         for (int j = 0; j < clArgs.RepeatCount; j++)
            results[j] = Runs.Test(random, clArgs.CallCount, clArgs.Significance, out testStatistic[j], out pValues[j]);

         Combining.CombinePassingResults(Console.Out, results, clArgs.Significance);
         Combining.CheckHistogramOfPValues(Console.Out, pValues, clArgs.Significance);
      }
   }

   private static void DoLongestRunTest(IRandom random, CommandLineArgs clArgs)
   {
      Console.WriteLine("Running Longest Run of Ones Test");
      Console.WriteLine($"Block Size: {clArgs.LongestRunBlockSize}");
      Console.WriteLine($"Call Count: {clArgs.CallCount:N0}");
      Console.WriteLine($"Significance: {clArgs.Significance}");

      if (clArgs.RepeatCount == 1)
      {
         double testStatistic, pValue;
         int callCountBefore = clArgs.CallCount;
         int callCountAfter = callCountBefore;
         bool result = LongestRun.Test(random, clArgs.LongestRunBlockSize, ref callCountAfter, clArgs.Significance, out testStatistic, out pValue);

         if (callCountBefore != callCountAfter)
            Console.WriteLine($"Call Count was adjusted to {callCountAfter:N0}");
         Console.WriteLine($"Test Statistic: {testStatistic}");
         Console.WriteLine($"p-Value: {pValue}");
         Console.WriteLine("Null hypothesis is {0}.", result ? "ACCEPTED" : "REJECTED");
      }
      else
      {
         double[] testStatistics = new double[clArgs.RepeatCount];
         double[] pValues = new double[clArgs.RepeatCount];
         bool[] results = new bool[clArgs.RepeatCount];
         int callCountBefore = clArgs.CallCount;
         int callCountAfter = callCountBefore;

         for (int j = 0; j < clArgs.RepeatCount; j ++)
            results[j] = LongestRun.Test(random, clArgs.LongestRunBlockSize, ref callCountAfter, clArgs.Significance, out testStatistics[j], out pValues[j]);

         if (callCountBefore != callCountAfter)
            Console.WriteLine($"Call Count was adjusted to {callCountAfter:N0}");

         Combining.CombinePassingResults(Console.Out, results, clArgs.Significance);
         Combining.CheckHistogramOfPValues(Console.Out, pValues, clArgs.Significance);
      }
   }

   private static void DoBinaryMatrixRankTest(IRandom random, CommandLineArgs clArgs)
   {
      Console.WriteLine("Binary Matrix Rank Test");
      Console.WriteLine($"Matrix Size: {clArgs.MatrixSize}");
      Console.WriteLine($"Call Count: {clArgs.CallCount:N0}");
      Console.WriteLine($"Significance: {clArgs.Significance}");

      if (clArgs.RepeatCount == 1)
      {
         double testStatistic, pValue;
         int callCountBefore = clArgs.CallCount;
         int callCountAfter = callCountBefore;
         int unusedBitCount;
         bool result = BinaryMatrixRank.Test(random, clArgs.MatrixSize, ref callCountAfter, clArgs.Significance, out testStatistic, out pValue, out unusedBitCount);

         int matrixCount = callCountAfter / (clArgs.MatrixSize * clArgs.MatrixSize);
         Console.WriteLine($"Matrix Count: {matrixCount:N0}");
         if (callCountBefore != callCountAfter)
            Console.WriteLine($"Call Count was adjusted to {callCountAfter:N0}");
         Console.WriteLine($"Unused Bit Count: {unusedBitCount:N0}");
         Console.WriteLine($"Test Statistic: {testStatistic}");
         Console.WriteLine($"p-Value: {pValue}");
         Console.WriteLine("Null hypothesis is {0}.", result ? "ACCEPTED" : "REJECTED");
      }
      else
      {
         double[] testStatistics = new double[clArgs.RepeatCount];
         double[] pValues = new double[clArgs.RepeatCount];
         bool[] results = new bool[clArgs.RepeatCount];
         int callCountBefore = clArgs.CallCount;
         int callCountAfter = callCountBefore;
         int unusedBitCount = 0;

         for (int j = 0; j < clArgs.RepeatCount; j ++)
            results[j] = BinaryMatrixRank.Test(random, clArgs.MatrixSize, ref callCountAfter, clArgs.Significance, out testStatistics[j], out pValues[j], out unusedBitCount);

         int matrixCount = callCountAfter / (clArgs.MatrixSize * clArgs.MatrixSize);
         Console.WriteLine($"Matrix Count: {matrixCount:N0}");
         if (callCountBefore != callCountAfter)
            Console.WriteLine($"Call Count was adjusted to {callCountAfter:N0}");
         Console.WriteLine($"Unused Bit Count: {unusedBitCount:N0}");

         Combining.CombinePassingResults(Console.Out, results, clArgs.Significance);
         Combining.CheckHistogramOfPValues(Console.Out, pValues, clArgs.Significance);
      }
   }

   private static void DoSpectralTest(IRandom random, CommandLineArgs clArgs)
   {
      Console.WriteLine("Spectral Test");
      Console.WriteLine($"Call Count: {clArgs.CallCount:N0}");
      Console.WriteLine($"Significance: {clArgs.Significance}");

      if (clArgs.RepeatCount == 1)
      {
         double testStatistic, pValue;
         int callCountBefore = clArgs.CallCount;
         int callCountAfter = callCountBefore;
         bool result = Spectral.Test(random, ref callCountAfter, clArgs.Significance, out testStatistic, out pValue);

         if (callCountBefore != callCountAfter)
            Console.WriteLine($"Call Count was adjusted to {callCountAfter:N0}");
         Console.WriteLine($"Test Statistic: {testStatistic}");
         Console.WriteLine($"p-Value: {pValue}");
         Console.WriteLine("Null hypothesis is {0}.", result ? "ACCEPTED" : "REJECTED");
      }
      else
      {
         double[] testStatistics = new double[clArgs.RepeatCount];
         double[] pValues = new double[clArgs.RepeatCount];
         bool[] results = new bool[clArgs.RepeatCount];
         int callCountBefore = clArgs.CallCount;
         int callCountAfter = callCountBefore;

         for (int j = 0; j < clArgs.RepeatCount; j ++)
            results[j] = Spectral.Test(random, ref callCountAfter, clArgs.Significance, out testStatistics[j], out pValues[j]);

         if (callCountBefore != callCountAfter)
            Console.WriteLine($"Call Count was adjusted to {callCountAfter:N0}");

         Combining.CombinePassingResults(Console.Out, results, clArgs.Significance);
         Combining.CheckHistogramOfPValues(Console.Out, pValues, clArgs.Significance);
      }
   }

   private static void DoNonoverlappingTest(IRandom random, CommandLineArgs clArgs)
   {
      Console.WriteLine("Non-overlapping Template Matching Test");
      Console.WriteLine($"Call Count: {clArgs.CallCount:N0}");
      Console.WriteLine($"Significance: {clArgs.Significance}");

      int callCountBefore = clArgs.CallCount;
      int callCountAfter = callCountBefore;

      if (clArgs.RepeatCount == 1)
      {
         Dictionary<int, CombinedPValues> result = NonOverlapping.Test(random, ref callCountAfter, clArgs.Significance);

         if (callCountBefore != callCountAfter)
            Console.WriteLine($"Call Count was adjusted to {callCountAfter:N0}");
         string[,] table = new string[result.Count + 1, 3];
         table[0, 0] = "Length";
         table[0, 1] = "Fischer";
         table[0, 2] = "P-Value";
         int rw = 0;
         bool fischerPass = true;
         foreach (int len in result.Keys.OrderBy(x => x))
         {
            rw++;
            table[rw, 0] = len.ToString();
            table[rw, 1] = (result[len].FischerPValue >= clArgs.Significance) ? "ACCEPTED" : "REJECT";
            table[rw, 2] = result[len].FischerPValue.ToString("0.000000");
            fischerPass &= result[len].FischerPValue >= clArgs.Significance;
         }
         UtilityMethods.PrintTable(table);
         Console.WriteLine("Overall, the Null Hypothesis is {0}.", fischerPass ? "ACCEPTED" : "REJECTED");
      }
      else
      {
         int maxTemplateLength = 10;
         int minTemplateLength = 9;
         double[][] pValues = new double[maxTemplateLength + 1][];
         bool[][] results = new bool[maxTemplateLength + 1][];
         for (int j = minTemplateLength; j <= maxTemplateLength; j ++)
         {
            pValues[j] = new double[clArgs.RepeatCount];
            results[j] = new bool[clArgs.RepeatCount];
         }

         // Run the tests Repeat Count times.
         for (int j = 0; j < clArgs.RepeatCount; j++)
         {
            for (int k = minTemplateLength; k <= maxTemplateLength; k++)
            {
               Dictionary<int, CombinedPValues> result = NonOverlapping.Test(random, ref callCountAfter, clArgs.Significance);
               pValues[k][j] = result[k].FischerPValue;
               results[k][j] = result[k].FischerPass;
            }
         }

         // Output combined results
         // Note we add 2 to the difference in the maximum and minimum template lengths -
         //  1 because the limits are both inclusive and 1 for the header row.
         string[,] table = new string[2 + maxTemplateLength - minTemplateLength, 6];
         table[0, 0] = "Template Length";
         table[0, 1] = "Pass proportion";
         table[0, 2] = "Result";
         table[0, 3] = "Uniformity Chi-Squared";
         table[0, 4] = "p-Value";
         table[0, 5] = "Result";
         int rw = 1;
         for (int j = minTemplateLength; j <= maxTemplateLength; j++, rw++)
         {
            table[rw, 0] = j.ToString();
            double passProportion, minAcceptable, maxAcceptable;
            Combining.PassingProportionResult r = Combining.PassingProportion(results[j], clArgs.Significance, out minAcceptable, out maxAcceptable, out passProportion);

            table[rw, 1] = passProportion.ToString("0.000");
            table[rw, 2] = passProportion >= minAcceptable && passProportion <= maxAcceptable ? "PASS" : "FAIL";

            double chiSquared, pValue;
            bool uniform = Combining.HistogramUniformity(pValues[j], clArgs.Significance, out chiSquared, out pValue);
            table[rw, 3] = chiSquared.ToString("0.000");
            table[rw, 4] = pValue.ToString("0.000000");
            table[rw, 5] = uniform ? "PASS" : "FAIL";
         }
         UtilityMethods.PrintTable(table);
      }
   }

   private static void DoOverlappingTest(IRandom random, CommandLineArgs clArgs)
   {
      Console.WriteLine("Overlapping Template Matching Test");
      Console.WriteLine($"Significance: {clArgs.Significance}");

      if (clArgs.RepeatCount == 1)
      {
         double testStatistic, pValue;
         bool result = Overlapping.Test(random, clArgs.Significance, out testStatistic, out pValue);

         Console.WriteLine($"Test Statistic: {testStatistic}");
         Console.WriteLine($"p-Value: {pValue}");
         Console.WriteLine("Null hypothesis is {0}.", result ? "ACCEPTED" : "REJECTED");
      }
      else
      {
         double[] testStatistics = new double[clArgs.RepeatCount];
         double[] pValues = new double[clArgs.RepeatCount];
         bool[] results = new bool[clArgs.RepeatCount];

         for (int j = 0; j < clArgs.RepeatCount; j ++)
            results[j] = Overlapping.Test(random, clArgs.Significance, out testStatistics[j], out pValues[j]);

         Combining.CombinePassingResults(Console.Out, results, clArgs.Significance);
         Combining.CheckHistogramOfPValues(Console.Out, pValues, clArgs.Significance);
      }
   }

   private static void DoMaurerTest(IRandom random, CommandLineArgs clArgs)
   {
      Console.WriteLine("Maurer's \"Universal Statistical\" Test");
      Console.WriteLine($"Significance: {clArgs.Significance}");
      Console.WriteLine($"Block Size: 2**{clArgs.BlockSize}");

      if (clArgs.RepeatCount == 1)
      {
         double testStatistic, pValue;
         bool result = Maurer.Test(random, clArgs.BlockSize, clArgs.Significance, out testStatistic, out pValue);

         Console.WriteLine($"Test Statistic: {testStatistic}");
         Console.WriteLine($"p-Value: {pValue}");
         Console.WriteLine("Null hypothesis is {0}.", result ? "ACCEPTED" : "REJECTED");
      }
      else
      {
         double[] testStatistics = new double[clArgs.RepeatCount];
         double[] pValues = new double[clArgs.RepeatCount];
         bool[] results = new bool[clArgs.RepeatCount];

         for (int j = 0; j < clArgs.RepeatCount; j ++)
            results[j] = Maurer.Test(random, clArgs.BlockSize, clArgs.Significance, out testStatistics[j], out pValues[j]);

         Combining.CombinePassingResults(Console.Out, results, clArgs.Significance);
         Combining.CheckHistogramOfPValues(Console.Out, pValues, clArgs.Significance);
      }
   }

   private static void DoLinearComplexityTest(IRandom random, CommandLineArgs clArgs)
   {
      Console.WriteLine("Linear Complexity Test");
      Console.WriteLine($"Significance: {clArgs.Significance}");
      Console.WriteLine($"Block Size: {clArgs.BlockSize}");
      Console.WriteLine($"Block Count: {clArgs.BlockCount}");

      if (clArgs.RepeatCount == 1)
      {
         double testStatistic, pValue;
         bool result = LinearComplexity.Test(random, clArgs.BlockSize, clArgs.BlockCount, clArgs.Significance, out testStatistic, out pValue);

         Console.WriteLine($"Test Statistic: {testStatistic}");
         Console.WriteLine($"p-Value: {pValue}");
         Console.WriteLine("Null hypothesis is {0}.", result ? "ACCEPTED" : "REJECTED");
      }
      else
      {
         double[] testStatistics = new double[clArgs.RepeatCount];
         double[] pValues = new double[clArgs.RepeatCount];
         bool[] results = new bool[clArgs.RepeatCount];

         for ( int j = 0; j < clArgs.RepeatCount; j ++)
            results[j] = LinearComplexity.Test(random, clArgs.BlockSize, clArgs.BlockCount, clArgs.Significance, out testStatistics[j], out pValues[j]);

         Combining.CombinePassingResults(Console.Out, results, clArgs.Significance);
         Combining.CheckHistogramOfPValues(Console.Out, pValues, clArgs.Significance);
      }
   }

   private static void DoSerialTest(IRandom random, CommandLineArgs clArgs)
   {
      Console.WriteLine("Serial Test");
      Console.WriteLine($"Significance: {clArgs.Significance}");
      Console.WriteLine($"Block Size: {clArgs.BlockSize}");
      Console.WriteLine($"Call Count: {clArgs.CallCount:N0}");

      int blockSizeBefore = clArgs.BlockSize;
      int blockSizeAfter = blockSizeBefore;
      int callCountBefore = clArgs.CallCount;
      int callCountAfter = callCountBefore;

      if (clArgs.RepeatCount == 1)
      {
         double testStatistic1, pValue1;
         double testStatistic2, pValue2;
         bool result = Serial.Test(random, ref callCountAfter, ref blockSizeAfter,
                  clArgs.Significance, out testStatistic1, out pValue1, out testStatistic2,
                  out pValue2);

         if (blockSizeAfter != blockSizeBefore)
            Console.WriteLine($"Block Size was adjusted to {blockSizeAfter}");
         if (callCountAfter != callCountBefore)
            Console.WriteLine($"Call was adjusted to {callCountAfter:N0}");
         Console.WriteLine($"Test Statistic #1: {testStatistic1}");
         Console.WriteLine($"p-Value #1: {pValue1}");
         Console.WriteLine($"Test Statistic #2: {testStatistic2}");
         Console.WriteLine($"p-Value #2: {pValue2}");
         Console.WriteLine("Null hypothesis is {0}.", result ? "ACCEPTED" : "REJECTED");
      }
      else
      {
         double testStatistic1, testStatistic2;
         double[] pValues1 = new double[clArgs.RepeatCount];
         double[] pValues2 = new double[clArgs.RepeatCount];
         bool[] results = new bool[clArgs.RepeatCount];

         for (int j = 0; j < clArgs.RepeatCount;j  ++)
            results[j] = Serial.Test(random, ref callCountAfter, ref blockSizeAfter, clArgs.Significance,
                     out testStatistic1, out pValues1[j], out testStatistic2, out pValues2[j]);

         if (blockSizeAfter != blockSizeBefore)
            Console.WriteLine($"Block Size was adjusted to {blockSizeAfter}");
         if (callCountAfter != callCountBefore)
            Console.WriteLine($"Call was adjusted to {callCountAfter:N0}");

         Combining.CombinePassingResults(Console.Out, results, clArgs.Significance);
         Console.WriteLine("First set of p-Values");
         Combining.CheckHistogramOfPValues(Console.Out, pValues1, clArgs.Significance);
         Console.WriteLine("Second set of p-Values");
         Combining.CheckHistogramOfPValues(Console.Out, pValues2, clArgs.Significance);
      }
   }

   private static void DoApproximateEntropyTest(IRandom random, CommandLineArgs clArgs)
   {
      Console.WriteLine("Approximate Entropy Test");
      Console.WriteLine($"Significance: {clArgs.Significance}");
      Console.WriteLine($"Block Size: {clArgs.BlockSize}");
      Console.WriteLine($"Call Count: {clArgs.CallCount:N0}");

      int callCountBefore = clArgs.CallCount;
      int callCountAfter = callCountBefore;

      if (clArgs.RepeatCount == 1)
      {
         double testStatistic, pValue;
         bool result = ApproximateEntropy.Test(random, ref callCountAfter, clArgs.BlockSize,
                  clArgs.Significance, out testStatistic, out pValue);

         if (callCountAfter != callCountBefore)
            Console.WriteLine($"Call was adjusted to {callCountAfter:N0}");
         Console.WriteLine($"Test Statistic: {testStatistic}");
         Console.WriteLine($"p-Value: {pValue}");
         Console.WriteLine("Null hypothesis is {0}.", result ? "ACCEPTED" : "REJECTED");
      }
      else
      {
         double[] testStatistics = new double[clArgs.RepeatCount];
         double[] pValues = new double[clArgs.RepeatCount];
         bool[] results = new bool[clArgs.RepeatCount];

         for (int j = 0; j < clArgs.RepeatCount; j++)
            results[j] = ApproximateEntropy.Test(random, ref callCountAfter, clArgs.BlockSize,
                  clArgs.Significance, out testStatistics[j], out pValues[j]);

         if (callCountAfter != callCountBefore)
            Console.WriteLine($"Call was adjusted to {callCountAfter:N0}");

         Combining.CombinePassingResults(Console.Out, results, clArgs.Significance);
         Combining.CheckHistogramOfPValues(Console.Out, pValues, clArgs.Significance);
      }
   }

   private static void DoCusumTest(IRandom random, CommandLineArgs clArgs)
   {
      Console.WriteLine("Cumulative Sums (Cusum) Test");
      Console.WriteLine($"Significance: {clArgs.Significance}");
      Console.WriteLine($"Call Count: {clArgs.CallCount:N0}");
      Console.WriteLine($"Mode: {clArgs.CumulativeSumsMode}");

      if (clArgs.RepeatCount == 1)
      {
         double testStatistic;
         double pValue;
         bool result = CumulativeSums.Test(random, clArgs.CallCount, clArgs.CumulativeSumsMode, clArgs.Significance, out testStatistic, out pValue);

         Console.WriteLine($"Test Statistic: {testStatistic}");
         Console.WriteLine($"p-Value: {pValue}");
         Console.WriteLine("Null hypothesis is {0}.", result ? "ACCEPTED" : "REJECTED");
      }
      else
      {
         double[] testStatistics = new double[clArgs.RepeatCount];
         double[] pValues = new double[clArgs.RepeatCount];
         bool[] results = new bool[clArgs.RepeatCount];

         for (int j = 0; j < clArgs.RepeatCount; j ++)
            results[j] = CumulativeSums.Test(random, clArgs.CallCount, clArgs.CumulativeSumsMode, clArgs.Significance, out testStatistics[j], out pValues[j]);

         Combining.CombinePassingResults(Console.Out, results, clArgs.Significance);
         Combining.CheckHistogramOfPValues(Console.Out, pValues, clArgs.Significance);
      }
   }

   private static void DoRandomExcursionsTest(IRandom random, CommandLineArgs clArgs)
   {
      Console.WriteLine("Random Excursions Test");
      Console.WriteLine($"Significance: {clArgs.Significance}");
      Console.WriteLine($"Call Count: {clArgs.CallCount:N0}");

      if (clArgs.RepeatCount == 1)
      {
         double pValue;
         double[]? testStatistics;
         double[]? pValues;
         bool result = RandomExcursions.Test(random, clArgs.CallCount, clArgs.Significance, out testStatistics, out pValues, out pValue);

         Console.WriteLine($"Combined p-Value: {pValue}");
         Console.WriteLine("Overall Null Hypothesis is {0}.", result ? "ACCEPTED" : "REJECTED");

         if (testStatistics is not null)
         {
            string[,] table = new string[9, 4];
            table[0, 0] = "State";
            table[0, 1] = "Chi Squared";
            table[0, 2] = "P-Value";
            table[0, 3] = "Conclusion";
            int stateIndex = 0;
            for (int x = -4; x <= 4; x++)
            {
               // skip zero
               if (x == 0)
                  continue;

               table[stateIndex + 1, 0] = x.ToString();
               table[stateIndex + 1, 1] = testStatistics[stateIndex].ToString("0.000000");
               table[stateIndex + 1, 2] = pValues![stateIndex].ToString("0.000000");
               table[stateIndex + 1, 3] = pValues[stateIndex] >= clArgs.Significance ? "Random" : "Non-Random";

               stateIndex++;
            }

            UtilityMethods.PrintTable(table);
         }
         else
         {
            Console.WriteLine("There were too few cycles to complete the test.");
         }
      }
      else
      {
         // The first index is the number of the run.
         // The second index identifies the State of the p-Value;
         double[]?[] testStatistics = new double[clArgs.RepeatCount][];
         double[]?[] pValues = new double[clArgs.RepeatCount][];
         bool[] results = new bool[clArgs.RepeatCount];

         for (int j = 0; j < clArgs.RepeatCount; j++)
            results[j] = RandomExcursions.Test(random, clArgs.CallCount, clArgs.Significance, out testStatistics[j], out pValues[j], out _);

         // The proportion passing is straightforward - failures due to not enough
         // cycles are still failures.
         Combining.CombinePassingResults(Console.Out, results, clArgs.Significance);

         // Count the number of tests that had enough cycles
         int count = pValues.Where(x => x is not null).Count();
         Console.WriteLine($"There were {count:N0} test runs with enough cycles.");

         // For each state, check the uniformity of the pValues.
         double[] uniformityTestStatistics = new double[RandomExcursions.STATE_COUNT];
         double[] uniformityPValues = new double[RandomExcursions.STATE_COUNT];
         Combining.PassingProportionResult[] passingProportionResults = new Combining.PassingProportionResult[RandomExcursions.STATE_COUNT];
         double[] observedProportions = new double[RandomExcursions.STATE_COUNT];
         double minAcceptable = 0.0, maxAcceptable = 0.0;
         for (int stateIndex = 0; stateIndex < RandomExcursions.STATE_COUNT; stateIndex++)
         {
            double[] tmpPValues = new double[count];
            bool[] tmpPass = new bool[count];
            for (int j = 0, idx = 0; j < clArgs.RepeatCount; j++)
            {
               if (pValues[j] is not null)
               {
                  tmpPValues[idx] = pValues[j]![stateIndex];
                  tmpPass[idx] = tmpPValues[idx] >= clArgs.Significance;
                  idx++;
               }
            }

            Combining.HistogramUniformity(tmpPValues, clArgs.Significance, out uniformityTestStatistics[stateIndex], out uniformityPValues[stateIndex]);
            passingProportionResults[stateIndex] = Combining.PassingProportion(tmpPass, clArgs.Significance, out minAcceptable, out maxAcceptable, out observedProportions[stateIndex]);
         }

         // Output
         Console.WriteLine("For each state:");
         Console.WriteLine($"Minimum acceptable success proportion: {minAcceptable:0.000}");
         Console.WriteLine($"Maximum acceptable success proportion: {maxAcceptable:0.000}");
         string[,] table = new string[1 + RandomExcursions.STATE_COUNT, 6];
         table[0, 0] = "State";
         table[0, 1] = "Proportion";
         table[0, 2] = "Pass/Fail";
         table[0, 3] = "Chi-Square";
         table[0, 4] = "P-Value";
         table[0, 5] = "Pass/Fail";
         for (int stateIndex = 0; stateIndex < RandomExcursions.STATE_COUNT; stateIndex ++)
         {
            table[stateIndex + 1, 0] = RandomExcursions.IndexToState(stateIndex).ToString();
            table[stateIndex + 1, 1] = observedProportions[stateIndex].ToString("0.000");
            table[stateIndex + 1, 2] = (observedProportions[stateIndex] >= minAcceptable && observedProportions[stateIndex] <= maxAcceptable) ? "PASS" : "FAIL";
            table[stateIndex + 1, 3] = uniformityTestStatistics[stateIndex].ToString("0.00");
            table[stateIndex + 1, 4] = uniformityPValues[stateIndex].ToString("0.000000");
            table[stateIndex + 1, 5] = uniformityPValues[stateIndex] >= clArgs.Significance ? "PASS" : "FAIL";
         }
         UtilityMethods.PrintTable(table);
      }
   }

   private static void DoRandomExcursionsVariantTest(IRandom random, CommandLineArgs clArgs)
   {
      Console.WriteLine("Random Excursions Test");
      Console.WriteLine($"Significance: {clArgs.Significance}");
      Console.WriteLine($"Call Count: {clArgs.CallCount:N0}");

      double pValue;
      double[]? testStatistics;
      double[]? pValues;
      bool result = RandomExcursionsVariant.Test(random, clArgs.CallCount, clArgs.Significance, out testStatistics, out pValues, out pValue);

      Console.WriteLine($"Combined p-Value: {pValue}");
      Console.WriteLine("Overall Null Hypothesis is {0}.", result ? "ACCEPTED" : "REJECTED");

      if (testStatistics is not null)
      {
         string[,] table = new string[19, 4];
         table[0, 0] = "State";
         table[0, 1] = "Counts";
         table[0, 2] = "P-Value";
         table[0, 3] = "Conclusion";
         int stateIndex = 0;
         for (int x = -9; x <= 9; x++)
         {
            // skip zero
            if (x == 0)
               continue;

            table[stateIndex + 1, 0] = x.ToString();
            table[stateIndex + 1, 1] = testStatistics[stateIndex].ToString("N0");
            table[stateIndex + 1, 2] = pValues![stateIndex].ToString("0.000000");
            table[stateIndex + 1, 3] = pValues[stateIndex] >= clArgs.Significance ? "Random" : "Non-Random";

            stateIndex++;
         }

         UtilityMethods.PrintTable(table);
      }
   }
}
