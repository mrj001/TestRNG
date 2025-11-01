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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using TestRNG.RNG;
using TestRNG.Statistics;

namespace TestRNG.Tests;

public static class RandomExcursions
{
   public const int DEFAULT_CALL_COUNT = 1_000_000;

   public static bool Test(IRandom random, int callCount, double sigLevel,
            [NotNullWhen(true)] out double[]? testStatistics, [NotNullWhen(true)] out double[]? pValues,
            out double pValue)
   {
      // TODO: There are too many hard-coded values (4, 8, Probabilities)

      // default output values
      pValue = 0.0;
      testStatistics = null;
      pValues = null;

      // Convert the random bit stream to a list of Sequences
      List<List<int>> sequences = new();
      int callNumber = 0;
      List<int> sequence = new();
      sequence.Add(0);
      while (callNumber < callCount)
      {
         int sum = 0;
         do
         {
            sum += random.NextBit() ? 1 : -1;
            sequence.Add(sum);
            callNumber++;
         } while (callNumber < callCount && sum != 0);
         if (callNumber == callCount && sum != 0)
            sequence.Add(0);
         sequences.Add(sequence);
         sequence = new();
      }
      // Assert that enough sequences were found.
      // Here, the bigJ == J (in Ref. A)
      int bigJ = sequences.Count;
      if (bigJ < Math.Max(0.005 * Math.Sqrt(callCount), 500))
      {   // Too few sequences - reject hypothesis
         return false;
      }

      // The first index is the state (-4,-3,-2,-1,1,2,3,4 map to the 8 indices)
      // The second index is the number of cycles in which that state occurs
      int maxClass = 5;
      int[,] v = new int[8, maxClass + 1];
      int stateIndex = 0;
      for (int x = -4; x <= 4; x++)
      {
         // skip zero
         if (x == 0)
            continue;

         // For each sequence
         for (int j = 0; j < bigJ; j++)
         {
            sequence = sequences[j];
            // Count the number of times this state occurred.
            int count = sequence.Where(y => y == x).Count();
            if (count > maxClass)
               count = maxClass;
            v[stateIndex, count]++;
         }

         stateIndex++;
      }

      // Assertion for each value of x, there are "J" counts
      for (stateIndex = 0; stateIndex < v.GetLength(0); stateIndex++)
      {
         int sum = 0;
         for (int j = 0; j <= maxClass; j++)
            sum += v[stateIndex, j];

         if (sum != bigJ)
            throw new ApplicationException();
      }

      // Probabilities
      // These have been sourced from Section 3.13 of Ref. A where we've copied
      // the states for x = 1, 2, 3, 4 and assumed symmetry for negative x values.
      // The indices are identical to those of the v[,] array
      double[,] pi = new double[,]
      {
         { 0.875, 0.0156, 0.0137, 0.012, 0.0105, 0.0733 },
         { 0.8333, 0.0278, 0.0231, 0.0193, 0.0161, 0.0804 },
         { 0.75, 0.0625, 0.0469, 0.0352, 0.0264, 0.0791 },
         { 0.5, 0.25, 0.125, 0.0625, 0.03125, 0.03125 },
         { 0.5, 0.25, 0.125, 0.0625, 0.03125, 0.03125 },
         { 0.75, 0.0625, 0.0469, 0.0352, 0.0264, 0.0791 },
         { 0.8333, 0.0278, 0.0231, 0.0193, 0.0161, 0.0804 },
         { 0.875, 0.0156, 0.0137, 0.012, 0.0105, 0.0733 },
      };

      // For each state, compute a test statistic and pValue
      stateIndex = 0;
      testStatistics = new double[8];
      pValues = new double[8];
      for (int x = -4; x <= 4; x++)
      {
         // skip zero
         if (x == 0)
            continue;

         for (int j = 0; j <= maxClass; j++)
         {
            double expected = bigJ * pi[stateIndex, j];
            testStatistics[stateIndex] += (v[stateIndex, j] - expected) * (v[stateIndex, j] - expected) / expected;
         }

         pValues[stateIndex] = Gamma.IncompleteGammaQ(maxClass / 2.0, testStatistics[stateIndex] / 2.0);
         stateIndex++;
      }

      // TODO: Can the Test Statistics be meaningfully combined?
      CombinedPValues combinedPValues = new(pValues, sigLevel);
      pValue = combinedPValues.FischerPValue;

      return pValue >= sigLevel;
   }
}