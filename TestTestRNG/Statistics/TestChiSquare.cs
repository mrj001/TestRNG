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

public class TestChiSquare
{
   [Fact]
   public void ChiSquare_BadX_Throws()
   {
      Assert.Throws<ArgumentException>(() => ChiSquare.ChiProb(-0.1, 24));
   }

   [Fact]
   public void ChiSquare_BadDegreesOfFreedom_Throws()
   {
      Assert.Throws<ArgumentException>(() => ChiSquare.ChiProb(42, 0));
   }

   public static TheoryData<double, double, double, int> ChiProbTestData
   {
      get
      {
         TheoryData<double, double, double, int> rv = new();

         // These x values were calculated with the ChiInv method of
         // Libre Office Calc.
         rv.Add(0.999999960837868, 1e-6, 2.84774721864565, 24);
         rv.Add(0.05, 1e-6, 36.4150285018073, 24);

         rv.Add(0.05, 1e-6, 125.458419408482, 101);

         // The following value was calculated using the online p-value
         // calculator at: https://www.socscistatistics.com/pvalues/chidistribution.aspx
         // On 2025-09-03
         rv.Add(.083265, 1e-6, 3, 1);

         return rv;
      }
   }

   [Theory]
   [MemberData(nameof(ChiProbTestData))]
   public void TestChiProb(double expected, double tolerance, double x, int df)
   {
      double actual = ChiSquare.ChiProb(x, df);

      Assert.True(Math.Abs(expected - actual) <= tolerance);
   }
}