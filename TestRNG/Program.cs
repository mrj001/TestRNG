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

namespace TestRNG;

public class Program
{
   public static void Main(string[] args)
   {
      CommandLineArgs? clArgs = CommandLineArgs.ParseCommandLineArgs(args, Console.Error);
      if (clArgs is null)
         return;

      switch (clArgs.SelectedTest)
      {
         case TestSelector.Uniform:
            DoUniformTest(clArgs);
            break;

         default:
            break;
      }
   }

   private static void DoUniformTest(CommandLineArgs args)
   {
      Console.WriteLine("Running Uniform test");
      if (!string.IsNullOrEmpty(args.OutputFileName))
         Console.WriteLine($"Output file: {args.OutputFileName}");
      else
         Console.WriteLine("No output file selected.");

      Console.WriteLine($"Bin Count: {args.BinCount:N0}");
      Console.WriteLine($"Call Count: {args.CallCount:N0}");
      Console.WriteLine($"Significance: {args.Significance}");

      UniformTest.Test(args.BinCount, args.CallCount, args.Significance, args.OutputFileName);
   }
}
