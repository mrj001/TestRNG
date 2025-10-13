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

namespace TestTestRNG.Statistics;

public class TestGamma
{
   public static TheoryData<double, double, double> IncompleteGammaQTestData
   {
      get
      {
         TheoryData<double, double, double> rv = new();

         // These two test cases are from the examples in Section 2.2 of Ref. A.
         rv.Add(0.801252, 1.5, 0.5);
         rv.Add(0.706438, 5, 3.6);

         return rv;
      }
   }

   [Theory]
   [MemberData(nameof(IncompleteGammaQTestData))]
   public void IncompleteGammaQ(double expected, double a, double x)
   {
      //
      // Action
      //
      double actual = Gamma.IncompleteGammaQ(a, x);

      //
      // Assertions
      //
      double diff = Math.Abs(expected - actual);
      Assert.True(Math.Abs(expected - actual) < 0.000001);
   }
}