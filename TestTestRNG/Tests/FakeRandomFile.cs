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

using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using TestRNG.RNG;

namespace TestTestRNG.Tests;

/// <summary>
/// This FakeRandom class returns the "random" bits from a file.
/// </summary>
public class FakeRandomFile : IRandom
{
   private List<bool> _bits;
   private int index = 0;

   public FakeRandomFile(string filename)
   {
      using (FileStream fs = new(filename, FileMode.Open, FileAccess.Read))
      using (GZipStream gz = new(fs, CompressionMode.Decompress))
      using (StreamReader sr = new(gz))
      {
         _bits = new();
         int ch;
         do
         {
            ch = sr.Read();
            _bits.Add(ch == '1' ? true : false);
         } while (ch >= 0);
      }
   }

   public int Next(int maxValue)
   {
      throw new System.NotImplementedException();
   }

   public bool NextBit()
   {
      // NOTE: This will throw an exception if called too many times.
      bool rv = _bits[index];
      index++;
      return rv;
   }
}
