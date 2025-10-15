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
using TestRNG.RNG;
using TestRNG.Tests;

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

      double testStatistic, pValue;
      bool result = MonobitTest.Test(random, args.CallCount, args.Significance, out testStatistic, out pValue);

      Console.WriteLine($"Test Statistic: {testStatistic}");
      Console.WriteLine($"p-Value: {pValue}");
      Console.WriteLine("Null hypothesis is {0}.", result ? "ACCEPTED" : "REJECTED");
   }

   private static void DoFrequencyBlockTest(IRandom random, CommandLineArgs clArgs)
   {
      Console.WriteLine("Running Frequency Block Test");
      Console.WriteLine($"Block Size: {clArgs.BlockSize}");
      Console.WriteLine($"Block Count: {clArgs.blockCount}");

      double testStatistic, pValue;
      bool result = FrequencyBlock.Test(random, clArgs.BlockSize, clArgs.blockCount, clArgs.Significance, out testStatistic, out pValue);

      Console.WriteLine($"Test Statistic: {testStatistic}");
      Console.WriteLine($"p-Value: {pValue}");
      Console.WriteLine("Null hypothesis is {0}.", result ? "ACCEPTED" : "REJECTED");
   }

   private static void DoRunsTest(IRandom random, CommandLineArgs clArgs)
   {
      Console.WriteLine("Running Runs Test");
      Console.WriteLine($"Call Count: {clArgs.CallCount:N0}");
      Console.WriteLine($"Significance: {clArgs.Significance}");

      double testStatistic, pValue;
      bool result = Runs.Test(random, clArgs.CallCount, clArgs.Significance, out testStatistic, out pValue);

      Console.WriteLine($"Test Statistic: {testStatistic}");
      Console.WriteLine($"p-Value: {pValue}");
      Console.WriteLine("Null hypothesis is {0}.", result ? "ACCEPTED" : "REJECTED");
   }

   private static void DoLongestRunTest(IRandom random, CommandLineArgs clArgs)
   {
      Console.WriteLine("Running Longest Run of Ones Test");
      Console.WriteLine($"Block Size: {clArgs.LongestRunBlockSize}");
      Console.WriteLine($"Call Count: {clArgs.CallCount:N0}");
      Console.WriteLine($"Significance: {clArgs.Significance}");

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

   private static void DoBinaryMatrixRankTest(IRandom random, CommandLineArgs clArgs)
   {
      Console.WriteLine("Binary Matrix Rank Test");
      Console.WriteLine($"Matrix Size: {clArgs.MatrixSize}");
      Console.WriteLine($"Call Count: {clArgs.CallCount:N0}");
      Console.WriteLine($"Significance: {clArgs.Significance}");

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
}
