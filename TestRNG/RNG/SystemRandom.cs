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
using System.Runtime.CompilerServices;

namespace TestRNG.RNG;

/// <summary>
/// A wrapper to present the System.Random class via the IRandom interface.
/// </summary>
public class SystemRandom : IRandom
{
   private readonly Random _random;

   public SystemRandom()
   {
      _random = new Random();
   }

   public SystemRandom(int seed)
   {
      _random = new Random(seed);
   }

   private const int _firstBitMask = 0x00000001;
   private const int _lastBitMask = 0x40000000;
   private int _bitMask = _firstBitMask;
   private int _randomBits = 0;

   /// <inheritdoc/>
   public bool NextBit()
   {
      if (_bitMask == _firstBitMask)
         _randomBits = _random.Next();

      bool rv = (_randomBits & _bitMask) != 0;
      _bitMask = _bitMask == _lastBitMask ? _firstBitMask : _bitMask << 1;

      return rv;
   }

   /// <inheritdoc/>
   public int Next(int maxValue)
   {
      return _random.Next(maxValue);
   }
}