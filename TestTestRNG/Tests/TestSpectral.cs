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
using TestRNG.Tests;
using Xunit;

namespace TestTestRNG.Tests;

public class TestSpectral
{
   private class FakeRandom : IRandom
   {
      private readonly Random _random = new(1234);
      private int _bitSerial = 0;


      public int Next(int maxValue)
      {
         throw new NotImplementedException();
      }

      public bool NextBit()
      {
         bool rv = _random.Next(2) != 0;

         // Force a failure of the Spectral Test by inserting specific
         // bits with a specific frequency.
         rv = _bitSerial % 101 < 2 ? false : rv;
         rv = _bitSerial % 107 < 2 ? true : rv;

         _bitSerial++;
         return rv;
      }
   }

   [Fact]
   public void DoTest()
   {
      IRandom random = new FakeRandom();
      double testStatistic;
      double pValue;
      int callCount = 1024;

      //
      // Action
      //
      bool actual = Spectral.Test(random, ref callCount, 0.01, out testStatistic, out pValue);

      //
      // Assertions
      //
      Assert.False(actual);
   }
}