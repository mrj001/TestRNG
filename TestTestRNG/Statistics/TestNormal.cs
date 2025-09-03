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
using TestRNG.Statistics;
using Xunit;

namespace TestTestRNG;

public class TestNormal
{
   public static TheoryData<double, double, double> GaussTestData
   {
      get
      {
         TheoryData<double, double, double> rv = new();

         rv.Add(0.5, 0.0, 0.0);
         rv.Add(1.0, 0.0, 6.2);
         rv.Add(0.0, 0.0, -6.9);

         // The following values were obtained from a table in 
         // CRC Standard Mathematical Tables, 28th edition, pages 551 to 560.
         rv.Add(0.9032, 0.00005, 1.3);
         rv.Add(0.1056, 0.00005, -1.25);

         rv.Add(0.9992, 0.00005, 3.14);
         rv.Add(0.0040, 0.00005, -2.65);

         return rv;
      }
   }

   [Theory]
   [MemberData(nameof(GaussTestData))]
   public void Gauss(double expected, double tolerance, double x)
   {
      double actual = Normal.Gauss(x);

      Assert.True(Math.Abs(expected - actual) <= tolerance);
   }
}
