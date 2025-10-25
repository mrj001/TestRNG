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
using System.IO.Compression;

namespace TestTestRNG.Utility;

public static class GenerateTestFiles
{
   // Reference:
   //  Stanley Rabinowitz and Stan Wagon,
   //  A Spigot Algorithm For The Digits Of Pi.
   //  The American Mathematical Monthly, Vol. 102, No. 3 (Mar. 1995) pp. 195-203
   // Accessed via: http://www.jstor.org/stable/2975006 on Monday, March 9, 2015.
   //
   // Note: The output of this method was confirmed by an independent implementation
   // (conversion to base 2 from an independently sourced base 10) and found to
   // be correct.
   public static void EFromSpigot(int numBits, string filename)
   {
      using (FileStream fs = new(filename, FileMode.Create, FileAccess.Write))
      using (GZipStream gz = new(fs, CompressionLevel.SmallestSize))
      using (StreamWriter sw = new(gz))
      {
         sw.Write("10");   // seed with the integer portion.
         numBits -= 2;

         // The reference indicates that n+2 mixed-radix digits are sufficient to obtain
         // n digits in base 10, with a footnote indicating that one should add a safety
         // margin of about 6, in case of a sequence of 9s.
         // We are interested in the first 'numBits' bits of e, so we convert this to a
         // number of base 10 digits and add a (much larger than needed) safety margin.
         //
         // Each index in the array corresponds to a denominator of the mixed radix
         // which is 2 greater than the index.  (eg. index 0 corresponds to the 1/2 place;
         // index 1 to the 1/3 place, etc.)
         int numMixedRadixDigits = (int)Math.Ceiling(500 + numBits * Math.Log(2.0) / Math.Log(10.0));
         int[] a = new int[numMixedRadixDigits];
         for (int j = 0; j < numMixedRadixDigits; j++)
            a[j] = 1;

         // Generate the first numBits bits of e
         while (numBits > 0)
         {
            // multiply every place by the base (2)
            for (int j = 0; j < numMixedRadixDigits; j++)
               a[j] *= 2;

            // Adjust all the digit values, except 1/2
            int carry = 0;
            for (int j = numMixedRadixDigits - 1; j > 0; j--)
            {
               int newVal = (a[j] + carry) % (j + 2);
               carry = (a[j] + carry) / (j + 2);
               a[j] = newVal;
            }

            // Adjust the 1/2 digit and generate a bit
            int nextBit = (a[0] + carry) / 2;
            if (nextBit < 0 || nextBit > 1) throw new ApplicationException();
            a[0] = (a[0] + carry) % 2;
            sw.Write(nextBit == 0 ? '0' : '1');
            numBits--;
         }
      }
   }
}