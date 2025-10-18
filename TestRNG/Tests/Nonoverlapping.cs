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
using System.Collections;
using System.Collections.Generic;
using TestRNG.RNG;
using TestRNG.Statistics;

namespace TestRNG.Tests;

public static class NonOverlapping
{
   private static readonly TemplateLibraries _templateLibraries = new();

   /// <summary>
   /// 
   /// </summary>
   /// <param name="random">The Random Number Generator to test.</param>
   /// <param name="callCount">The number of times to call the Random Number Generator.  If this is not
   /// a multiple of the block count, it will be adjusted upwards to the next multiple of the block count.</param>
   /// <param name="sigLevel"></param>
   /// <returns>A Dictionary<int, bool>.  The key is the template length; the value is a pass/fail.</returns>
   public static Dictionary<int, bool> Test(IRandom random, ref int callCount, double sigLevel)
   {
      int blockCount = 8;
      if (callCount % blockCount != 0)
         callCount += blockCount - callCount % blockCount;
      bool[] bitStream = new bool[callCount];

      // Copy the output of the Random Number Generator to an array, so we can use the same
      // output over and over again.
      for (int j = 0; j < callCount; j++)
         bitStream[j] = random.NextBit();

      return Test(bitStream, blockCount, sigLevel);
   }

   /// <summary>
   /// Runs the Nonoverlapping test using all templates of all lengths over the given bitStream
   /// </summary>
   /// <param name="bitStream"></param>
   /// <param name="blockCount">Corresponds to "N" in Section 2.7 of Ref. A</param>
   /// <param name="sigLevel"></param>
   /// <returns>A Dictionary<int, bool>.  The key is the template length; the value is a pass/fail.</returns>
   private static Dictionary<int, bool> Test(bool[] bitStream, int blockCount, double sigLevel)
   {
      Dictionary<int, bool> rv = new(20);

      // For each Template Length
      for (int templateLength = 2; templateLength <= 10; templateLength++)
      {
         bool pass = (1.0 - sigLevel) < TestForEachTemplate(bitStream, blockCount, templateLength, sigLevel);
         rv.Add(templateLength, pass);
      }

      return rv;
   }

   /// <summary>
   /// Runs all the templates of a given length.
   /// </summary>
   /// <param name="bitStream"></param>
   /// <param name="blockCount">Corresponds to "N" in Section 2.7 of Ref. A</param>
   /// <param name="templateLength"></param>
   /// <param name="sigLevel"></param>
   /// <returns>The ratio of passing templates to all templates.  A value of 0.0 indicates that all templates
   /// failed, while a value of 1.0 indicates that all templates passed.</returns>
   private static double TestForEachTemplate(bool[] bitStream, int blockCount, int templateLength, double sigLevel)
   {
      double testStatistic;
      double pValue;
      double passRatio;
      int templateCount = 0;
      int passCount = 0;

      foreach (int template in _templateLibraries.Get(templateLength))
      {
         templateCount++;
         if (TestForEachBlock(bitStream, blockCount, templateLength, template, sigLevel, out testStatistic, out pValue))
            passCount++;
      }

      passRatio = ((double)passCount) / templateCount;

      return passRatio;
   }

   private static bool TestForEachBlock(bool[] bitStream, int blockCount, int templateLength,
            int template, double sigLevel, out double testStatistic, out double pValue)
   {
      int blockLength = bitStream.Length / blockCount;
      bool[] bitBlock = new bool[blockLength];
      int[] w = new int[blockCount];   // count of matches within each block

      for (int blockIndex = 0; blockIndex < blockCount; blockIndex++)
      {
         // copy the block
         int blockStart = blockIndex * blockLength;
         for (int j = 0; j < blockLength; j++)
            bitBlock[j] = bitStream[blockStart + j];

         w[blockIndex] = CountMatchesWithinBlock(bitBlock, templateLength, template);
      }

      // Calculate mu and variance
      double mu = (blockLength - templateLength + 1) / Math.Pow(2, templateLength);
      double variance = blockLength * (1.0 / Math.Pow(2, templateLength) - (2 * templateLength - 1) / Math.Pow(2, 2 * templateLength));

      // compute testStatistic
      double chiSquared = 0.0;
      foreach (int wj in w)
         chiSquared += (wj - mu) * (wj - mu) / variance;
      testStatistic = chiSquared;

      pValue = Gamma.IncompleteGammaQ(blockCount / 2.0, chiSquared / 2.0);

      return pValue >= sigLevel;
   }

   /// <summary>
   /// Counts the template matches within a block of random bits.
   /// </summary>
   /// <param name="bitBlock">A block of bits to be tested.  The Length of this array
   /// corresponds to "M" in Section 2.7 of Ref. A</param>
   /// <param name="templateLength">Corresponds to "m" in Section 2.7 of Ref. A</param>
   /// <param name="template">Corresponds to "B" in Section 2.7  of Ref. A</param>
   /// <returns></returns>
   private static int CountMatchesWithinBlock(bool[] bitBlock, int templateLength, int template)
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
         {
            bitIndex += templateLength;
            matchCount++;
         }
         else
         {
            bitIndex++;
         }
      }

      return matchCount;
   }

   private class TemplateLibrary : IEnumerable<int>
   {
      private readonly int[] _templates;

      /// <summary>
      /// Constructs a TemplateLibrary of Templates with the specified length.
      /// </summary>
      /// <param name="m">The length, in bits, of the Templates.</param>
      public TemplateLibrary(int m)
      {
         List<int> templates = new();

         // Any template starting with a leading zero, will be periodic
         // if it ends with a zero, as that would constitute a prefix of length 1.
         // Therefore, only test odd numbers with a leading zero.
         // Check only odd numbers for periodicity
         int mask = (1 << m) - 1;
         int jul = 1 << m;
         for (int j = 1; j <= jul; j += 2)
            if (!CheckPeriodic(j, m))
               templates.Add(j);

         // Add the complements of all the found aperiodic templates.
         jul = templates.Count;
         for (int j = 0; j < jul; j++)
            templates.Add((~templates[j]) & mask);

         templates.Sort();
         _templates = templates.ToArray();
      }

      private static bool CheckPeriodic(int bits, int m)
      {
         // NOTE: We do not have to test maskLength m -1 because, by construction,
         // the values passed in avoid that possible periodicity.
         for (int maskLength = 1; maskLength < m; maskLength++)
         {
            int mask = ((1 << maskLength) - 1) << (m - maskLength);
            int repeatBits = bits & mask;
            bool periodic = true;
            do
            {
               mask >>= maskLength;
               repeatBits >>= maskLength;
               periodic = repeatBits == (bits & mask);
            } while (periodic && mask != 0);
            if (periodic)
               return true;
         }

         return false;
      }

      public int Count()
      {
         return _templates.Length;
      }

      public IEnumerator<int> GetEnumerator()
      {
         for (int j = 0; j < _templates.Length; j++)
            yield return _templates[j];
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }
   }

   private class TemplateLibraries
   {
      private readonly Dictionary<int, TemplateLibrary> _cache = new(20);

      public TemplateLibrary Get(int len)
      {
         if (!_cache.ContainsKey(len))
            _cache.Add(len, new TemplateLibrary(len));

         return _cache[len];
      }
   }
}