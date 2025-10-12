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
using System.IO;

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
   private readonly int _callCount = 0;

   private const string SIGNIFICANCE_SHORT = "-s";
   private const string SIGNIFICANCE_LONG = "--significance";
   private const double DEFAULT_SIGNIFICANCE = 0.05;
   private readonly double _significance = 0.05;

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
      tw.WriteLine("\tuniform");
      tw.WriteLine();
      tw.WriteLine($"Uniform test arguments:");
      tw.WriteLine($"   [{BIN_COUNT_SHORT} | {BIN_COUNT_LONG} BinCount]");
      tw.WriteLine("      BinCount must be at least 2.");
      tw.WriteLine($"      If not specified, defaults to {DEFAULT_BIN_COUNT:N0}");
      tw.WriteLine();
      tw.WriteLine($"   [{CALL_COUNT_SHORT} | {CALL_COUNT_LONG} CallCount]");
      tw.WriteLine("      The number of times to call the Random Number Generator.");
      tw.WriteLine($"      If not specified, defaults to {DEFAULT_CALL_COUNT:N0}");
      tw.WriteLine();
      tw.WriteLine($"   [{SIGNIFICANCE_SHORT} | {SIGNIFICANCE_LONG} Significance]");
      tw.WriteLine("      The statistical significance level.");
      tw.WriteLine($"      If not specified, defaults to {DEFAULT_SIGNIFICANCE}");
      tw.WriteLine();
   }
   #endregion

   public TestSelector SelectedTest { get => _selectedTest; }

   public string? OutputFileName { get => _outputFileName; }

   public int BinCount { get => _binCount; }

   public int CallCount { get => _callCount; }
   
   public double Significance { get => _significance; }
}