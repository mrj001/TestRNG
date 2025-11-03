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
using System.ComponentModel.Design;
using System.IO;
using TestRNG.Tests;

namespace TestRNG;

public class CommandLineArgs
{
   private readonly string? _outputFileName = null;
   private readonly TestSelector _selectedTest;

   private const string BIN_COUNT_SHORT = "-b";
   private const string BIN_COUNT_LONG = "--bins";
   private const int DEFAULT_BIN_COUNT = 2;
   private readonly int _binCount = DEFAULT_BIN_COUNT;

   private const string CALL_COUNT_SHORT = "-c";
   private const string CALL_COUNT_LONG = "--calls";
   private const int DEFAULT_CALL_COUNT = 100_000;
   private const int DEFAULT_RUNS_CALL_COUNT = 100;
   public const int MINIMUM_SPECTRAL_CALL_COUNT = 1024;
   private const int DEFAULT_SPECTRAL_CALL_COUNT = 1024;
   private const int DEFAULT_NONOVERLAPPING_CALL_COUNT = 8_000;
   private readonly int _callCount = 0;

   private const string SIGNIFICANCE_SHORT = "-s";
   private const string SIGNIFICANCE_LONG = "--significance";
   private const double DEFAULT_SIGNIFICANCE = 0.05;
   private readonly double _significance = 0.05;

   private const string BLOCK_COUNT_SHORT = "-bc";
   private const string BLOCK_COUNT_LONG = "--blockcount";
   private const int BLOCK_COUNT_DEFAULT = 100;
   private readonly int _blockCount = 0;

   private const string BLOCK_SIZE_SHORT = "-bs";
   private const string BLOCK_SIZE_LONG = "-blocksize";
   private const int BLOCK_SIZE_DEFAULT = 31;
   private readonly int _blockSize = 0;

   private const string MATRIX_SIZE_SHORT = "-ms";
   private const string MATRIX_SIZE_LONG = "--matrixsize";
   private const int MATRIX_SIZE_DEFAULT = 32;
   private readonly int _matrixSize = 0;

   private const string CUMULATIVE_SUMS_MODE_SHORT = "-m";
   private const string CUMULATIVE_SUMS_MODE_LONG = "--mode";
   private CumulativeSums.Mode _mode = CumulativeSums.Mode.Forward;

   private const LongestRunBlockSize BLOCK_SIZE_LONGEST_RUNS_DEFAULT = LongestRunBlockSize.Small;
   private LongestRunBlockSize _longestRunsBlockSize = BLOCK_SIZE_LONGEST_RUNS_DEFAULT;

   #region Construction
   private CommandLineArgs(string[] args)
   {
      if (args.Length == 0)
         throw new ArgumentException("Missing required arguments.");

      int argIndex = 0;

      // Parse optional output file name
      if (args[argIndex] == "-o")
      {
         argIndex++;
         if (args.Length == argIndex)
            throw new ArgumentException("Missing output file name");
         _outputFileName = args[argIndex];
         argIndex++;
      }

      // Parse the test name
      try
      {
         _selectedTest = Enum.Parse<TestSelector>(args[argIndex], true);
         argIndex++;
      }
      catch (Exception ex)
      {
         throw new ArgumentException($"Invalid test name: '{args[argIndex]}'", ex);
      }

      // Parse test-specific Arguments
      switch (_selectedTest)
      {
         case TestSelector.Uniform:
            ParseUniformTestArgs(args, ref argIndex, out _binCount, out _callCount, out _significance);
            break;

         case TestSelector.Monobit:
            ParseMonobitTestArgs(args, ref argIndex, out _callCount, out _significance);
            break;

         case TestSelector.FrequencyBlock:
            ParseFrequencyBlockTestArgs(args, ref argIndex, out _blockSize, out _blockCount, out _significance);
            break;

         case TestSelector.Runs:
            ParseRunsTestArgs(args, ref argIndex, out _callCount, out _significance);
            break;

         case TestSelector.LongestRun:
            ParseLongestRunTestArgs(args, ref argIndex, out _longestRunsBlockSize, out _callCount, out _significance);
            break;

         case TestSelector.MatrixRank:
            ParseBinaryMatrixRankTestArgs(args, ref argIndex, out _callCount, out _matrixSize, out _significance);
            break;

         case TestSelector.Spectral:
            ParseSpectralTestArgs(args, ref argIndex, out _callCount, out _significance);
            break;

         case TestSelector.NonOverlapping:
            ParseNonoverlappingTestArgs(args, ref argIndex, out _callCount, out _significance);
            break;

         case TestSelector.Overlapping:
            ParseNonoverlappingTestArgs(args, ref argIndex, out _significance);
            break;

         case TestSelector.Maurer:
            ParseMaurerTestArgs(args, ref argIndex, out _blockSize, out _significance);
            break;

         case TestSelector.Linear:
            ParseLinearComplexityTestArgs(args, ref argIndex, out _blockSize, out _blockCount, out _significance);
            break;

         case TestSelector.Serial:
            ParseSerialTestArgs(args, ref argIndex, out _callCount, out _blockSize, out _significance);
            break;

         case TestSelector.Entropy:
            ParseApproximateEntropyTestArgs(args, ref argIndex, out _callCount, out _blockSize, out _significance);
            break;

         case TestSelector.Cusum:
            ParseCusumTestArgs(args, ref argIndex, out _callCount, out _mode, out _significance);
            break;

         case TestSelector.Excursions:
            ParseRandomExcursionsTestArgs(args, ref argIndex, out _callCount, out _significance);
            break;

         case TestSelector.ExcursionsVariant:
            ParseRandomExcursionsVariantTestArgs(args, ref argIndex, out _callCount, out _significance);
            break;

         default:
            break;
      }
   }

   private static void ParseUniformTestArgs(string[] args, ref int argIndex, out int binCount, out int callCount, out double significance)
   {
      binCount = DEFAULT_BIN_COUNT;
      callCount = DEFAULT_CALL_COUNT;
      significance = DEFAULT_SIGNIFICANCE;

      while (argIndex < args.Length)
      {
         if (args[argIndex] == BIN_COUNT_SHORT || args[argIndex] == BIN_COUNT_LONG)
         {
            argIndex++;
            binCount = ParseIntegerArg(args[argIndex - 1], args, ref argIndex);
         }
         else if (args[argIndex] == CALL_COUNT_SHORT || args[argIndex] == CALL_COUNT_LONG)
         {
            argIndex++;
            callCount = ParseIntegerArg(args[argIndex - 1], args, ref argIndex);
         }
         else if (args[argIndex] == SIGNIFICANCE_SHORT || args[argIndex] == SIGNIFICANCE_SHORT)
         {
            argIndex++;
            significance = ParseDoubleArg(args[argIndex - 1], args, ref argIndex);
         }
         else
         {
            throw new ArgumentException($"Unknown argument: '{args[argIndex]}");
         }
      }
   }

   private static void ParseMonobitTestArgs(string[] args, ref int argIndex, out int callCount, out double significance)
   {
      callCount = DEFAULT_CALL_COUNT;
      significance = DEFAULT_SIGNIFICANCE;

      while (argIndex < args.Length)
      {
         if (args[argIndex] == CALL_COUNT_SHORT || args[argIndex] == CALL_COUNT_LONG)
         {
            argIndex++;
            callCount = ParseIntegerArg(args[argIndex - 1], args, ref argIndex);
         }
         else if (args[argIndex] == SIGNIFICANCE_SHORT || args[argIndex] == SIGNIFICANCE_SHORT)
         {
            argIndex++;
            significance = ParseDoubleArg(args[argIndex - 1], args, ref argIndex);
         }
         else
         {
            throw new ArgumentException($"Unknown argument: '{args[argIndex]}");
         }
      }
   }

   private static void ParseFrequencyBlockTestArgs(string[] args, ref int argIndex, out int blockSize, out int blockCount, out double significance)
   {
      blockSize = BLOCK_SIZE_DEFAULT;
      blockCount = BLOCK_COUNT_DEFAULT;
      significance = DEFAULT_SIGNIFICANCE;

      while (argIndex < args.Length)
      {
         if (args[argIndex] == BLOCK_SIZE_SHORT || args[argIndex] == BLOCK_SIZE_LONG)
         {
            argIndex++;
            blockSize = ParseIntegerArg(args[argIndex - 1], args, ref argIndex);
         }
         else if (args[argIndex] == BLOCK_COUNT_SHORT || args[argIndex] == BLOCK_COUNT_LONG)
         {
            argIndex++;
            blockCount = ParseIntegerArg(args[argIndex - 1], args, ref argIndex);
         }
         else if (args[argIndex] == SIGNIFICANCE_SHORT || args[argIndex] == SIGNIFICANCE_SHORT)
         {
            argIndex++;
            significance = ParseDoubleArg(args[argIndex - 1], args, ref argIndex);
         }
         else
         {
            throw new ArgumentException($"Unknown argument: '{args[argIndex]}'");
         }
      }
   }

   private static void ParseRunsTestArgs(string[] args, ref int argIndex, out int callCount, out double significance)
   {
      callCount = DEFAULT_RUNS_CALL_COUNT;
      significance = DEFAULT_SIGNIFICANCE;

      while (argIndex < args.Length)
      {
         if (args[argIndex] == CALL_COUNT_SHORT || args[argIndex] == CALL_COUNT_LONG)
         {
            argIndex++;
            callCount = ParseIntegerArg(args[argIndex - 1], args, ref argIndex);
         }
         else if (args[argIndex] == SIGNIFICANCE_SHORT || args[argIndex] == SIGNIFICANCE_SHORT)
         {
            argIndex++;
            significance = ParseDoubleArg(args[argIndex - 1], args, ref argIndex);
         }
         else
         {
            throw new ArgumentException($"Unknown argument: '{args[argIndex]}'");
         }
      }
   }

   private static void ParseLongestRunTestArgs(string[] args, ref int argIndex, out LongestRunBlockSize blockSize, out int callCount, out double significance)
   {
      callCount = 0;
      significance = DEFAULT_SIGNIFICANCE;
      blockSize = BLOCK_SIZE_LONGEST_RUNS_DEFAULT;

      while (argIndex < args.Length)
      {
         if (args[argIndex] == CALL_COUNT_SHORT || args[argIndex] == CALL_COUNT_LONG)
         {
            argIndex++;
            callCount = ParseIntegerArg(args[argIndex - 1], args, ref argIndex);
         }
         else if (args[argIndex] == SIGNIFICANCE_SHORT || args[argIndex] == SIGNIFICANCE_SHORT)
         {
            argIndex++;
            significance = ParseDoubleArg(args[argIndex - 1], args, ref argIndex);
         }
         else if (args[argIndex] == BLOCK_SIZE_SHORT || args[argIndex] == BLOCK_SIZE_LONG)
         {
            argIndex++;
            blockSize = Enum.Parse<LongestRunBlockSize>(args[argIndex], true);
            argIndex++;
         }
         else
         {
            throw new ArgumentException($"Unknown argument: '{args[argIndex]}'");
         }
      }
   }

   private static void ParseBinaryMatrixRankTestArgs(string[] args, ref int argIndex, out int callCount, out int matrixSize, out double significance)
   {
      callCount = 0;
      significance = DEFAULT_SIGNIFICANCE;
      matrixSize = MATRIX_SIZE_DEFAULT;


      while (argIndex < args.Length)
      {
         if (args[argIndex] == CALL_COUNT_SHORT || args[argIndex] == CALL_COUNT_LONG)
         {
            argIndex++;
            callCount = ParseIntegerArg(args[argIndex - 1], args, ref argIndex);
         }
         else if (args[argIndex] == SIGNIFICANCE_SHORT || args[argIndex] == SIGNIFICANCE_SHORT)
         {
            argIndex++;
            significance = ParseDoubleArg(args[argIndex - 1], args, ref argIndex);
         }
         else if (args[argIndex] == MATRIX_SIZE_SHORT || args[argIndex] == MATRIX_SIZE_LONG)
         {
            argIndex++;
            matrixSize = ParseIntegerArg(args[argIndex - 1], args, ref argIndex);
         }
         else
         {
            throw new ArgumentException($"Unknown argument: '{args[argIndex]}'");
         }
      }
   }

   private static void ParseSpectralTestArgs(string[] args, ref int argIndex, out int callCount, out double significance)
   {
      callCount = DEFAULT_SPECTRAL_CALL_COUNT;
      significance = DEFAULT_SIGNIFICANCE;

      while (argIndex < args.Length)
      {
         if (args[argIndex] == CALL_COUNT_SHORT || args[argIndex] == CALL_COUNT_LONG)
         {
            argIndex++;
            callCount = ParseIntegerArg(args[argIndex - 1], args, ref argIndex);
         }
         else if (args[argIndex] == SIGNIFICANCE_SHORT || args[argIndex] == SIGNIFICANCE_SHORT)
         {
            argIndex++;
            significance = ParseDoubleArg(args[argIndex - 1], args, ref argIndex);
         }
         else
         {
            throw new ArgumentException($"Unknown argument: '{args[argIndex]}'");
         }
      }
   }

   private static void ParseNonoverlappingTestArgs(string[] args, ref int argIndex, out int callCount, out double significance)
   {
      callCount = DEFAULT_NONOVERLAPPING_CALL_COUNT;
      significance = DEFAULT_SIGNIFICANCE;

      while (argIndex < args.Length)
      {
         if (args[argIndex] == CALL_COUNT_SHORT || args[argIndex] == CALL_COUNT_LONG)
         {
            argIndex++;
            callCount = ParseIntegerArg(args[argIndex - 1], args, ref argIndex);
         }
         else if (args[argIndex] == SIGNIFICANCE_SHORT || args[argIndex] == SIGNIFICANCE_SHORT)
         {
            argIndex++;
            significance = ParseDoubleArg(args[argIndex - 1], args, ref argIndex);
         }
         else
         {
            throw new ArgumentException($"Unknown argument: '{args[argIndex]}'");
         }
      }
   }

   private static void ParseNonoverlappingTestArgs(string[] args, ref int argIndex, out double significance)
   {
      significance = DEFAULT_SIGNIFICANCE;

      while (argIndex < args.Length)
      {
         if (args[argIndex] == SIGNIFICANCE_SHORT || args[argIndex] == SIGNIFICANCE_SHORT)
         {
            argIndex++;
            significance = ParseDoubleArg(args[argIndex - 1], args, ref argIndex);
         }
         else
         {
            throw new ArgumentException($"Unknown argument: '{args[argIndex]}'");
         }
      }
   }

   private static void ParseMaurerTestArgs(string[] args, ref int argIndex, out int blockSize, out double significance)
   {
      blockSize = Maurer.BLOCK_SIZE_DEFAULT;
      significance = DEFAULT_SIGNIFICANCE;

      while (argIndex < args.Length)
      {
         if (args[argIndex] == BLOCK_SIZE_SHORT || args[argIndex] == BLOCK_SIZE_LONG)
         {
            argIndex++;
            blockSize = ParseIntegerArg(args[argIndex - 1], args, ref argIndex);
         }
         if (args[argIndex] == SIGNIFICANCE_SHORT || args[argIndex] == SIGNIFICANCE_SHORT)
         {
            argIndex++;
            significance = ParseDoubleArg(args[argIndex - 1], args, ref argIndex);
         }
         else
         {
            throw new ArgumentException($"Unknown argument: '{args[argIndex]}'");
         }
      }
   }

   private static void ParseLinearComplexityTestArgs(string[] args, ref int argIndex, out int blockSize, out int blockCount, out double significance)
   {
      significance = DEFAULT_SIGNIFICANCE;
      blockSize = LinearComplexity.MINIMUM_BLOCK_SIZE;
      blockCount = LinearComplexity.MINIMUM_BLOCK_COUNT;

      while (argIndex < args.Length)
      {
         if (args[argIndex] == BLOCK_SIZE_SHORT || args[argIndex] == BLOCK_SIZE_LONG)
         {
            argIndex++;
            blockSize = ParseIntegerArg(args[argIndex - 1], args, ref argIndex);
         }
         if (args[argIndex] == BLOCK_COUNT_SHORT || args[argIndex] == BLOCK_COUNT_LONG)
         {
            argIndex++;
            blockCount = ParseIntegerArg(args[argIndex - 1], args, ref argIndex);
         }
         if (args[argIndex] == SIGNIFICANCE_SHORT || args[argIndex] == SIGNIFICANCE_SHORT)
         {
            argIndex++;
            significance = ParseDoubleArg(args[argIndex - 1], args, ref argIndex);
         }
         else
         {
            throw new ArgumentException($"Unknown argument: '{args[argIndex]}'");
         }
      }
   }

   private static void ParseSerialTestArgs(string[] args, ref int argIndex, out int callCount, out int blockSize, out double significance)
   {
      significance = DEFAULT_SIGNIFICANCE;
      callCount = Serial.DEFAULT_CALL_COUNT;
      blockSize = Serial.DEFAULT_BLOCK_SIZE;

      while (argIndex < args.Length)
      {
         if (args[argIndex] == BLOCK_SIZE_SHORT || args[argIndex] == BLOCK_SIZE_LONG)
         {
            argIndex++;
            blockSize = ParseIntegerArg(args[argIndex - 1], args, ref argIndex);
         }
         if (args[argIndex] == CALL_COUNT_SHORT || args[argIndex] == CALL_COUNT_LONG)
         {
            argIndex++;
            callCount = ParseIntegerArg(args[argIndex - 1], args, ref argIndex);
         }
         if (args[argIndex] == SIGNIFICANCE_SHORT || args[argIndex] == SIGNIFICANCE_SHORT)
         {
            argIndex++;
            significance = ParseDoubleArg(args[argIndex - 1], args, ref argIndex);
         }
         else
         {
            throw new ArgumentException($"Unknown argument: '{args[argIndex]}'");
         }
      }
   }

   private static void ParseApproximateEntropyTestArgs(string[] args, ref int argIndex, out int callCount, out int blockSize, out double significance)
   {
      significance = DEFAULT_SIGNIFICANCE;
      callCount = ApproximateEntropy.DEFAULT_CALL_COUNT;
      blockSize = ApproximateEntropy.DEFAULT_BLOCK_SIZE;

      while (argIndex < args.Length)
      {
         if (args[argIndex] == BLOCK_SIZE_SHORT || args[argIndex] == BLOCK_SIZE_LONG)
         {
            argIndex++;
            blockSize = ParseIntegerArg(args[argIndex - 1], args, ref argIndex);
         }
         if (args[argIndex] == CALL_COUNT_SHORT || args[argIndex] == CALL_COUNT_LONG)
         {
            argIndex++;
            callCount = ParseIntegerArg(args[argIndex - 1], args, ref argIndex);
         }
         if (args[argIndex] == SIGNIFICANCE_SHORT || args[argIndex] == SIGNIFICANCE_LONG)
         {
            argIndex++;
            significance = ParseDoubleArg(args[argIndex - 1], args, ref argIndex);
         }
         else
         {
            throw new ArgumentException($"Unknown argument: '{args[argIndex]}'");
         }
      }
   }

   private static void ParseCusumTestArgs(string[] args, ref int argIndex, out int callCount, out CumulativeSums.Mode mode, out double significance)
   {
      significance = DEFAULT_SIGNIFICANCE;
      callCount = CumulativeSums.DEFAULT_CALL_COUNT;
      mode = CumulativeSums.Mode.Forward;

      while (argIndex < args.Length)
      {
         if (args[argIndex] == CALL_COUNT_SHORT || args[argIndex] == CALL_COUNT_LONG)
         {
            argIndex++;
            callCount = ParseIntegerArg(args[argIndex - 1], args, ref argIndex);
         }
         if (args[argIndex] == CUMULATIVE_SUMS_MODE_SHORT || args[argIndex] == CUMULATIVE_SUMS_MODE_LONG)
         {
            argIndex++;
            mode = Enum.Parse<CumulativeSums.Mode>(args[argIndex], true);
            argIndex++;
         }
         if (args[argIndex] == SIGNIFICANCE_SHORT || args[argIndex] == SIGNIFICANCE_LONG)
         {
            argIndex++;
            significance = ParseDoubleArg(args[argIndex - 1], args, ref argIndex);
         }
         else
         {
            throw new ArgumentException($"Unknown argument: '{args[argIndex]}'");
         }
      }
   }

   private static void ParseRandomExcursionsTestArgs(string[] args, ref int argIndex, out int callCount, out double significance)
   {
      significance = DEFAULT_SIGNIFICANCE;
      callCount = CumulativeSums.DEFAULT_CALL_COUNT;

      while (argIndex < args.Length)
      {
         if (args[argIndex] == CALL_COUNT_SHORT || args[argIndex] == CALL_COUNT_LONG)
         {
            argIndex++;
            callCount = ParseIntegerArg(args[argIndex - 1], args, ref argIndex);
         }
         if (args[argIndex] == SIGNIFICANCE_SHORT || args[argIndex] == SIGNIFICANCE_LONG)
         {
            argIndex++;
            significance = ParseDoubleArg(args[argIndex - 1], args, ref argIndex);
         }
         else
         {
            throw new ArgumentException($"Unknown argument: '{args[argIndex]}'");
         }
      }
   }

   private static void ParseRandomExcursionsVariantTestArgs(string[] args, ref int argIndex, out int callCount, out double significance)
   {
      significance = DEFAULT_SIGNIFICANCE;
      callCount = CumulativeSums.DEFAULT_CALL_COUNT;

      while (argIndex < args.Length)
      {
         if (args[argIndex] == CALL_COUNT_SHORT || args[argIndex] == CALL_COUNT_LONG)
         {
            argIndex++;
            callCount = ParseIntegerArg(args[argIndex - 1], args, ref argIndex);
         }
         if (args[argIndex] == SIGNIFICANCE_SHORT || args[argIndex] == SIGNIFICANCE_LONG)
         {
            argIndex++;
            significance = ParseDoubleArg(args[argIndex - 1], args, ref argIndex);
         }
         else
         {
            throw new ArgumentException($"Unknown argument: '{args[argIndex]}'");
         }
      }
   }

   private static int ParseIntegerArg(string arg, string[] args, ref int argIndex)
   {
      if (argIndex >= args.Length)
         throw new ArgumentException($"Missing integer value for argument: '{arg}'");

      int rv;
      if (!int.TryParse(args[argIndex], out rv))
         throw new ArgumentException($"Unable to parse '{args[argIndex]}' as an integer argument.");

      argIndex++;

      return rv;
   }

   private static double ParseDoubleArg(string arg, string[] args, ref int argIndex)
   {
      if (argIndex >= args.Length)
         throw new ArgumentException($"Missing double value for argument: '{arg}]");

      double rv;
      if (!double.TryParse(args[argIndex], out rv))
         throw new ArgumentException($"Unable to parse '{args[argIndex]}' as a double argument.");

      argIndex++;

      return rv;
   }

   public static CommandLineArgs? ParseCommandLineArgs(string[] args, TextWriter? tw)
   {
      // Check for help request
      if (args.Length > 0 && (args[0] == "-h" || args[0] == "--help"))
      {
         PrintUsage(tw ?? Console.Error, null);
         return null;
      }

      try
      {
         return new CommandLineArgs(args);
      }
      catch (ArgumentException ex)
      {
         if (tw is not null)
            PrintUsage(tw, ex.Message);
         else
            Console.Error.WriteLine(ex.Message);

         return null;
      }
   }

   private static void PrintUsage(TextWriter tw, string? message)
   {
      if (!string.IsNullOrEmpty(message))
         tw.WriteLine(message);

      tw.WriteLine("Usage:");
      tw.WriteLine("TestRNG -h | --help");
      tw.WriteLine("\tPrints this help message and exits.");
      tw.WriteLine();
      tw.WriteLine("TestRNG [-o outputfilename] TestName {Test-specific args}");
      tw.WriteLine("\t-o specifies the name of a results file to be created.");
      tw.WriteLine("\t\tIf this file exists, it will be overwritten.");
      tw.WriteLine("\t\tIf this option is not given, no output file is created.");
      tw.WriteLine();
      tw.WriteLine("TestName is one of");

      foreach (TestSelector ts in Enum.GetValues<TestSelector>())
         tw.WriteLine($"\t{ts.ToString().ToLower()}");
      tw.WriteLine();

      tw.WriteLine($"Uniform test arguments:");
      tw.WriteLine($"   [{BIN_COUNT_SHORT} | {BIN_COUNT_LONG} BinCount]");
      tw.WriteLine("      BinCount must be at least 2.");
      tw.WriteLine($"      If not specified, defaults to {DEFAULT_BIN_COUNT:N0}");
      tw.WriteLine();
      PrintCallCountHelp(tw);
      tw.WriteLine();
      PrintSignificanceHelp(tw);
      tw.WriteLine();
      tw.WriteLine("Monobit test arguments:");
      PrintCallCountHelp(tw);
      tw.WriteLine();
      PrintSignificanceHelp(tw);
      tw.WriteLine();

      tw.WriteLine("Frequency Block Test arguments:");
      tw.WriteLine($"   [{BLOCK_SIZE_SHORT} | {BLOCK_SIZE_LONG}]");
      tw.WriteLine("      Specifies the number of bits in a block.");
      tw.WriteLine($"      If not specified, defaults to {BLOCK_SIZE_DEFAULT}");
      tw.WriteLine();
      tw.WriteLine($"   [{BLOCK_COUNT_SHORT} | {BLOCK_COUNT_LONG}]");
      tw.WriteLine("      Specifies the number of block to use in the test.");
      tw.WriteLine($"      If not specified, defaults to {BLOCK_COUNT_DEFAULT}");
      tw.WriteLine();
      PrintSignificanceHelp(tw);
      tw.WriteLine();

      tw.WriteLine("Runs Test arguments:");
      tw.WriteLine($"   [{CALL_COUNT_SHORT} | {CALL_COUNT_LONG}]");
      tw.WriteLine("      Specifies the number of times to call the Random Number Generator.");
      tw.WriteLine($"      If not specified, defaults to {DEFAULT_RUNS_CALL_COUNT}");
      tw.WriteLine();
      PrintSignificanceHelp(tw);
      tw.WriteLine();

      tw.WriteLine("Longest Run of Ones in a Block Test arguments:");
      tw.WriteLine($"   [{BLOCK_SIZE_SHORT} | {BLOCK_SIZE_LONG} Small | Medium | Large]");
      tw.WriteLine("      Specifies the number of bits in a block.");
      tw.WriteLine($"         Small = {(int)LongestRunBlockSize.Small:N0}");
      tw.WriteLine($"         Medium = {(int)LongestRunBlockSize.Medium:N0}");
      tw.WriteLine($"         Large = {(int)LongestRunBlockSize.Large:N0}");
      tw.WriteLine($"      If not specified, defaults to {BLOCK_SIZE_LONGEST_RUNS_DEFAULT}");
      tw.WriteLine();
      tw.WriteLine($"   [{CALL_COUNT_SHORT} | {CALL_COUNT_LONG} CallCount]");
      tw.WriteLine("      The number of times to call the Random Number Generator.");
      tw.WriteLine("      If not specified, defaults to a minimum value determined by the block size.");
      tw.WriteLine();
      PrintSignificanceHelp(tw);
      tw.WriteLine();

      tw.WriteLine("Binary Matrix Rank test arguments:");
      tw.WriteLine($"   [{MATRIX_SIZE_SHORT} | {MATRIX_SIZE_LONG}]");
      tw.WriteLine("      Specifies the number of rows and columns in a square matrix.");
      tw.WriteLine($"      If not specified, defaults to {MATRIX_SIZE_DEFAULT}");
      tw.WriteLine();
      tw.WriteLine($"   [{CALL_COUNT_SHORT} | {CALL_COUNT_LONG} CallCount]");
      tw.WriteLine("      The number of times to call the Random Number Generator.");
      tw.WriteLine($"      If not specified, defaults to enough times to generate {BinaryMatrixRank.MINIMUM_MATRIX_COUNT} matrices.");
      tw.WriteLine("      The specified number is subject to upward revision if required to achieve");
      tw.WriteLine($"      minimum of {BinaryMatrixRank.MINIMUM_MATRIX_COUNT} matrices.  As many matrices as can be generated within the");
      tw.WriteLine("      given number of calls will be generated.  Some trailing bits may be dropped.");
      tw.WriteLine();
      PrintSignificanceHelp(tw);
      tw.WriteLine();

      tw.WriteLine("Spectral Test Arguments:");
      tw.WriteLine($"   [{CALL_COUNT_SHORT} | {CALL_COUNT_LONG} CallCount]");
      tw.WriteLine("      The number of times to call the Random Number Generator.");
      tw.WriteLine($"      If not specified, defaults to {DEFAULT_SPECTRAL_CALL_COUNT:N0}");
      tw.WriteLine("      If the specified number is not a power of two, it will be adjusted upward");
      tw.WriteLine("      to the next power of two.");
      tw.WriteLine();
      PrintSignificanceHelp(tw);
      tw.WriteLine();

      tw.WriteLine("Non-overlapping Template Matching Test Arguments:");
      tw.WriteLine($"   [{CALL_COUNT_SHORT} | {CALL_COUNT_LONG} CallCount]");
      tw.WriteLine("      The number of times to call the Random Number Generator.");
      tw.WriteLine($"      If not specified, defaults to {DEFAULT_NONOVERLAPPING_CALL_COUNT:N0}");
      tw.WriteLine("      If the specified number is not a multiple of 8, it will be adjusted upward");
      tw.WriteLine("      to the next multiple of 8.");
      tw.WriteLine();
      PrintSignificanceHelp(tw);
      tw.WriteLine();

      tw.WriteLine("Overlapping Template Matching Test Arguments:");
      tw.WriteLine("   The code uses a 9-bit run of ones as the template, with 968 blocks");
      tw.WriteLine("   of length 1032 bits with 5 degrees of freedom.");
      tw.WriteLine();
      PrintSignificanceHelp(tw);
      tw.WriteLine();

      tw.WriteLine("Maurer's \"Universal Statistical\" Test");
      tw.WriteLine($"   [{BLOCK_SIZE_SHORT} | {BLOCK_SIZE_LONG} L]");
      tw.WriteLine($"      L will be a number between {Maurer.BLOCK_SIZE_MIN} and {Maurer.BLOCK_SIZE_MAX}");
      tw.WriteLine("      The number of bits in a block will be 2**L.");
      tw.WriteLine($"      If not specified, defaults to {Maurer.BLOCK_SIZE_MIN}");
      tw.WriteLine();
      PrintSignificanceHelp(tw);
      tw.WriteLine();

      tw.WriteLine("Linear Complexity Test");
      tw.WriteLine($"   [{BLOCK_SIZE_SHORT} | {BLOCK_SIZE_LONG} M]");
      tw.WriteLine($"      M must be a number between {LinearComplexity.MINIMUM_BLOCK_SIZE} and {LinearComplexity.MAXIMUM_BLOCK_SIZE}");
      tw.WriteLine($"      If not specified, defaults to {LinearComplexity.MINIMUM_BLOCK_SIZE}");
      tw.WriteLine();
      tw.WriteLine($"   [{BLOCK_COUNT_SHORT} | {BLOCK_COUNT_LONG}]");
      tw.WriteLine("      Specifies the number of blocks to run the test on.");
      tw.WriteLine($"      This must be at least {LinearComplexity.MINIMUM_BLOCK_COUNT}");
      tw.WriteLine($"      If not specified, defaults to {LinearComplexity.MINIMUM_BLOCK_COUNT}");
      tw.WriteLine();
      PrintSignificanceHelp(tw);
      tw.WriteLine();

      tw.WriteLine("Serial Test");
      tw.WriteLine($"   [{BLOCK_SIZE_SHORT} | {BLOCK_SIZE_LONG} M]");
      tw.WriteLine($"      M must be a number between {Serial.MINIMUM_BLOCK_SIZE} and {Serial.MAXIMUM_BLOCK_SIZE}");
      tw.WriteLine($"      If not specified, defaults to {Serial.DEFAULT_BLOCK_SIZE}");
      tw.WriteLine();
      tw.WriteLine($"   [{CALL_COUNT_SHORT} | {CALL_COUNT_LONG} CallCount]");
      tw.WriteLine("      The number of times to call the Random Number Generator.");
      tw.WriteLine($"      If not specified, defaults to {Serial.DEFAULT_CALL_COUNT:N0}");
      tw.WriteLine();
      PrintSignificanceHelp(tw);
      tw.WriteLine();

      tw.WriteLine("Approximate Entropy Test");
      tw.WriteLine($"   [{BLOCK_SIZE_SHORT} | {BLOCK_SIZE_LONG} M]");
      tw.WriteLine($"      M must be a number between {ApproximateEntropy.MINIMUM_BLOCK_SIZE} and {ApproximateEntropy.MAXIMUM_BLOCK_SIZE}");
      tw.WriteLine($"      If not specified, defaults to {ApproximateEntropy.DEFAULT_BLOCK_SIZE}");
      tw.WriteLine();
      tw.WriteLine($"   [{CALL_COUNT_SHORT} | {CALL_COUNT_LONG} CallCount]");
      tw.WriteLine("      The number of times to call the Random Number Generator.");
      tw.WriteLine($"      If not specified, defaults to {ApproximateEntropy.DEFAULT_CALL_COUNT:N0}");
      tw.WriteLine();
      PrintSignificanceHelp(tw);
      tw.WriteLine();

      tw.WriteLine("Cumulative Sums (Cusum) Test");
      tw.WriteLine($"   [{CALL_COUNT_SHORT} | {CALL_COUNT_LONG} CallCount]");
      tw.WriteLine("      The number of times to call the Random Number Generator.");
      tw.WriteLine($"      If not specified, defaults to {CumulativeSums.DEFAULT_CALL_COUNT:N0}");
      tw.WriteLine();
      tw.WriteLine($"      [{CUMULATIVE_SUMS_MODE_SHORT} | {CUMULATIVE_SUMS_MODE_LONG} mode]");
      tw.WriteLine("      mode must be one of forward or backwared.");
      tw.WriteLine($"      If not specified, defaults to {CumulativeSums.Mode.Forward}");
      tw.WriteLine();
      PrintSignificanceHelp(tw);
      tw.WriteLine();

      tw.WriteLine("Random Excursions Test");
      tw.WriteLine($"   [{CALL_COUNT_SHORT} | {CALL_COUNT_LONG} CallCount]");
      tw.WriteLine("      The number of times to call the Random Number Generator.");
      tw.WriteLine($"      If not specified, defaults to {RandomExcursions.DEFAULT_CALL_COUNT:N0}");
      tw.WriteLine();
      PrintSignificanceHelp(tw);
      tw.WriteLine();

      tw.WriteLine("Random Excursions Variant Test");
      tw.WriteLine($"   [{CALL_COUNT_SHORT} | {CALL_COUNT_LONG} CallCount]");
      tw.WriteLine("      The number of times to call the Random Number Generator.");
      tw.WriteLine($"      If not specified, defaults to {RandomExcursionsVariant.DEFAULT_CALL_COUNT:N0}");
      tw.WriteLine();
      PrintSignificanceHelp(tw);
      tw.WriteLine();
   }

   private static void PrintCallCountHelp(TextWriter tw)
   {
      tw.WriteLine($"   [{CALL_COUNT_SHORT} | {CALL_COUNT_LONG} CallCount]");
      tw.WriteLine("      The number of times to call the Random Number Generator.");
      tw.WriteLine($"      If not specified, defaults to {DEFAULT_CALL_COUNT:N0}");
   }

   private static void PrintSignificanceHelp(TextWriter tw)
   {
      tw.WriteLine($"   [{SIGNIFICANCE_SHORT} | {SIGNIFICANCE_LONG} Significance]");
      tw.WriteLine("      The statistical significance level.");
      tw.WriteLine($"      If not specified, defaults to {DEFAULT_SIGNIFICANCE}");
   }

   #endregion

   public TestSelector SelectedTest { get => _selectedTest; }

   public string? OutputFileName { get => _outputFileName; }

   public int BinCount { get => _binCount; }

   public int CallCount { get => _callCount; }

   public double Significance { get => _significance; }

   public int BlockSize { get => _blockSize; }

   public LongestRunBlockSize LongestRunBlockSize { get => _longestRunsBlockSize; }

   public int BlockCount { get => _blockCount; }
   
   public int MatrixSize { get => _matrixSize; }

   public CumulativeSums.Mode CumulativeSumsMode { get => _mode; }
}