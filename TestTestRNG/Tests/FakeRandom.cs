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

using TestRNG.RNG;

namespace TestTestRNG.Tests;

/// <summary>
/// This FakeRandom class returns the "random" bits passed to the constructor.
/// </summary>
public class FakeRandom : IRandom
{
   private readonly string _bitString;
   private int index = 0;

   public FakeRandom(string bitString)
   {
      _bitString = bitString;
   }

   public int Next(int maxValue)
   {
      throw new System.NotImplementedException();
   }

   public bool NextBit()
   {
      // NOTE: This will throw an exception if called too many times.
      bool rv = _bitString[index] == '1';
      index++;
      return rv;
   }
}
