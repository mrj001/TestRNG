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
using TestRNG.RNG;
using TestRNG.Utility;

namespace TestRNG.Tests;

public static class BinaryMatrixRank
{
   public const int MINIMUM_MATRIX_COUNT = 38;

   public static bool Test(IRandom rnd, int matrixSize, ref int callCount, double sigLevel, out double testStatistic, out double pValue, out int unusedBitCount)
   {
      // Ensure that Call Count is sufficiently large to generate 38 matrices.
      int totalMatrixBits = matrixSize * matrixSize;
      callCount = Math.Max(callCount, MINIMUM_MATRIX_COUNT * totalMatrixBits);
      unusedBitCount = callCount;
      int matrixCount = unusedBitCount / totalMatrixBits;

      // Generate the matrices and count their ranks.
      int callsRemaining = unusedBitCount;
      int[] f = new int[3];
      while (unusedBitCount >= totalMatrixBits)
      {
         // Generate a matrix
         Matrix matrix = new(matrixSize);
         for (int rw = 0; rw < matrixSize; rw++)
            for (int col = 0; col < matrixSize; col++)
               if (rnd.NextBit())
                  matrix[rw, col] = true;

         // Count the rank
         int r = matrix.GetRank();
         int freqIndex = Math.Min(matrixSize - r, f.Length - 1);
         f[freqIndex]++;

         unusedBitCount -= totalMatrixBits;
      }

      // Calculate Chi-Squared
      double[] probs = new double[] { 0.2888, 0.5776, 0.1336 };
      testStatistic = 0.0;
      for (int j = 0; j < f.Length; j++)
      {
         double expected = probs[j] * matrixCount;
         testStatistic += (f[j] - expected) * (f[j] - expected) / expected;
      }

      // Calculate the p-Value
      pValue = Math.Exp(-testStatistic / 2.0);

      return pValue >= sigLevel;
   }
}