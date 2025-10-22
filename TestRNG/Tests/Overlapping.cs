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

//--------------------------------------------------------------------
// References:
//--------------------------------------------------------------------
// 1.
// On the revision of NIST 800-22 Test Suites,
// Katarzyna Anna Kowalska, Davide Fogliano, Jose Garcia Coello
// KK DF JGC - MARCH 2022
// https://eprint.iacr.org/2022/540.pdf (accessed on 2025-10-18)
//
// 2.
// Correction of Overlapping Template Matching Test Include in NIST
// Randomness Test Suite,
// Kenji Hamano and Toshinobu Kaneko
// IEICE Transactions on Fundamentals of Electronic Communications
// and Computer Science, September 2007.
// https://www.researchgate.net/publication/220241122_Correction_of_Overlapping_Template_Matching_Test_Included_in_NIST_Randomness_Test_Suite?enrichId=rgreq-eea38c54f76af9dde82ddb629caf5f21-XXX&enrichSource=Y292ZXJQYWdlOzIyMDI0MTEyMjtBUzoxMTQ4ODY4NDM1NzIyMjZAMTQwNDQwMjU2NzI1Mw%3D%3D&el=1_x_2&_esc=publicationCoverPdf
// (Accessed on 2025-10-19)
//--------------------------------------------------------------------

using System;
using System.Numerics;
using TestRNG.RNG;
using TestRNG.Statistics;

namespace TestRNG.Tests;

public static class Overlapping
{
   public static bool Test(IRandom random, double sigLevel, out double testStatistic, out double pValue)
   {
      // This version uses the parameters recommended by NIST in Section 2.8 of Ref. A.
      int blockCount = 968;
      int bitsPerBlock = 1032;
      int templateLength = 9;
      int[] v;

      return Test(random, blockCount, bitsPerBlock, templateLength, sigLevel, out v, out testStatistic, out pValue);
   }

   // This is a more heavily parameterized version to support unit testing.
   public static bool Test(IRandom random, int blockCount, int bitsPerBlock, int templateLength,
            double sigLevel, out int[] v, out double testStatistic, out double pValue)
   {
      int template = (1 << templateLength) - 1;

      // Per Reference #2, these values given by NIST in Sections 2.8 and 3.8 of Ref. A are for
      // a bit block length of 1032 bits with a template length of 9 bits.  Ref. A does not
      // specifically state this.  Instead, it implies that these values can be used for both 
      // template lengths of 9 and 10.
      // I've noted that they change significantly for a template length of 10.
      // double[] p = new double[] { 0.364091, 0.185659, 0.139381, 0.100571, 0.0704323, 0.139865 };
      double[] p = CalculatePiHamanoKaneko(6, bitsPerBlock, templateLength);
      v = new int[p.Length];

      for (int j = 0; j < blockCount; j++)
      {
         bool[] bitBlock = new bool[bitsPerBlock];
         for (int k = 0; k < bitsPerBlock; k++)
            bitBlock[k] = random.NextBit();
         int matchCount = CountMatchesWithinBlock(bitBlock, templateLength, template);
         matchCount = Math.Min(matchCount, v.Length - 1);
         v[matchCount]++;
      }

      testStatistic = 0.0;
      for (int j = 0; j < v.Length; j++)
         testStatistic += (v[j] - blockCount * p[j]) * (v[j] - blockCount * p[j]) / (blockCount * p[j]);

      pValue = Gamma.IncompleteGammaQ((p.Length - 1) / 2.0, testStatistic / 2.0);

      return pValue >= sigLevel;
   }

   /// <summary>
   /// </summary>
   /// <param name="k">The number of probabilities to calculate.</param>
   /// <param name="N">The number of Bits Per Block (N in the Reference #1)</param>
   /// <param name="m">The length of the Template in bits (m in the Reference #1).</param>
   /// <returns>
   /// An array containing the specified number of probabilities.
   /// </returns>
   /// <remarks>
   /// <para>
   /// This code is adapted from the C code presented in Reference #1.  I've fixed one
   /// IndexOutOfRangeException, with reference to Reference #2.  I've noted that the T
   /// values are all integers.  They're larger than supported by the .NET double data
   /// type, so I've used BigInteger instead.
   /// </para>
   /// </remarks>
   private static double[] CalculatePiHamanoKaneko(int k, int bitsPerBlock, int templateLength)
   {
      // Relative to Reference #2, every access to the second index has to have
      // one added to it.
      BigInteger[,] t = new BigInteger[k, bitsPerBlock + 2];

      // Compute T0(j) (j <= bitsPerBlock) using Eq. (2).
      for (int n = -1; n <= bitsPerBlock; n++)
      {
         if (n == -1 || n == 0)
            t[0, n + 1] = 1;
         else if (n <= (templateLength - 1))
            t[0, n + 1] = 2 * t[0, n];
         else
            t[0, n + 1] = 2 * t[0, n] - t[0, n - templateLength];
      }

      // Compute T[1, j] (j <= bitsPerBlock) using Eq. (3) and the values for T0.
      for (int n = -1; n <= bitsPerBlock; n++)
      {
         if (n <= (templateLength - 1))
         {
            t[1, n + 1] = 0;
         }
         else if (n == templateLength)
         {
            t[1, n + 1] = 1;
         }
         else if (n == templateLength + 1)
         {
            t[1, n + 1] = 2;
         }
         else
         {
            BigInteger sum = 0;
            for (int j = -1; j <= (n - templateLength - 1); j++)
               sum += t[0, j + 1] * t[0, n - templateLength - 1 - j];
            t[1, n + 1] = sum;
         }
      }

      //Compute T[a, j] (j <= n) using Eq. (5) and the values for T0, T_i (2 <= i <= k).
      for (int a = 2; a <= (k - 2); a++)
      {
         for (int n = -1; n <= bitsPerBlock; n++)
         {
            BigInteger sum = 0;
            for (int j = -1; j <= (n - (2 * templateLength) - a); j++)
               sum += t[0, j + 1] * t[a - 1, n - templateLength - 1 - j];

            // On the right hand side, the code in the Reference #1, uses
            // _i(n-1) as the second index.  However, when n == -1 (the first
            // time through the loop), this produces an IndexOutOfRangeException
            // as _i(n-1) == -1.
            // We note that Reference #2 indicates that T(x) is defined as 
            // zero when x < 0.
            t[a, n + 1] = (n >= 0 ? t[a - 1, n] : 0) + sum;
         }
      }

      // Compute T[k-1, n] using Eq. (1) and the values for Ti (0 <= i <= k - 1)
      double pi_sum = 0;
      //Compute the first pi K values:
      double[] pi = new double[k];
      for (int i = 0; i <= (k - 2); i++)
      {
         // The code in the Reference #1 uses the "long double" method
         //  powl(x,y) to calculate Math.Pow(2.0, bitsPerBlock),
         // but there is no "long double" type in .NET.  For the double
         // type, this calculation overflows and produces double.Infinity.
         // So we use logs.
         // pi[i] = t[i, bitsPerBlock + 1] / Math.Pow(2.0, bitsPerBlock);
         double numLog = BigInteger.Log(t[i, bitsPerBlock + 1]);
         double denLog = Math.Log(2.0) * bitsPerBlock;
         pi[i] = Math.Exp(numLog - denLog);
         pi_sum += pi[i];
      }
      pi[k - 1] = 1 - pi_sum;

      return pi;
   }

   /// <summary>
   /// Counts the template matches within a block of random bits.
   /// </summary>
   /// <param name="bitBlock">A block of bits to be tested.  The Length of this array
   /// corresponds to "M" in Section 2.7 of Ref. A</param>
   /// <param name="templateLength">Corresponds to "m" in Section 2.7 of Ref. A</param>
   /// <param name="template">Corresponds to "B" in Section 2.7  of Ref. A</param>
   /// <returns></returns>
   public static int CountMatchesWithinBlock(bool[] bitBlock, int templateLength, int template)
   {
      bool[] localTemplate = new bool[templateLength];
      for (int j = 0; j < templateLength; j++)
         localTemplate[j] = (template & (1 << j)) != 0;

      // determine how many times the template matches within the given block.
      int bitIndex = 0;
      int matchCount = 0;
      while (bitIndex < bitBlock.Length - templateLength)
      {
         // Check for a match
         bool match = true;
         int j = 0;
         while (j < templateLength && match)
         {
            match = bitBlock[bitIndex + j] == localTemplate[j];
            j++;
         }

         if (match)
            matchCount++;
         bitIndex++;
      }

      return matchCount;
   }

}