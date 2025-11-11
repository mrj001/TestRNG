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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TestRNG.Tests;
using Xunit;

namespace TestTestRNG.Tests;

public class TestNonoverlapping
{
   public static TheoryData<int, bool[], int, int> CountMatchesWithinBlockTestData
   {
      get
      {
         TheoryData<int, bool[], int, int> rv = new();

         // These test cases are from the example in Section 2.7.4 of Ref. A.
         rv.Add(2, new bool[] { true, false, true, false, false, true, false, false, true, false }, 3, 1);
         rv.Add(1, new bool[] { true, true, true, false, false, true, false, true, true, false }, 3, 1);

         return rv;
      }
   }

   [Theory]
   [MemberData(nameof(CountMatchesWithinBlockTestData))]
   public void CountMatchesWithinBlock(int expected, bool[] randomBits, int templateLength, int template)
   {
      MethodInfo? mi = typeof(NonOverlapping).GetMethod("CountMatchesWithinBlock", BindingFlags.Static | BindingFlags.NonPublic);

      //
      // Action
      //
      int? actual = (int?)mi?.Invoke(null, new object[] { randomBits, templateLength, template });

      //
      // Assertions
      //
      Assert.NotNull(mi);
      Assert.True(actual.HasValue);
      Assert.Equal(expected, actual.Value);
   }

   public static TheoryData<bool, double, double, bool[], int, int, int, double> TestForEachBlockTestData
   {
      get
      {
         TheoryData<bool, double, double, bool[], int, int, int, double> rv = new();

         rv.Add(true, 2.133333, 0.344154,
                  new bool[] { true, false, true, false, false, true, false, false, true, false, true, true, true, false, false, true, false, true, true, false },
                  2, 3, 1, 0.01);

         return rv;
      }
   }

   [Theory]
   [MemberData(nameof(TestForEachBlockTestData))]
   public void TestForEachBlock(bool expected, double expectedTestStatistic,
            double expectedPValue, bool[] bitStream, int blockCount, int templateLength,
            int template, double sigLevel)
   {
      double tolerance = 1E-6;
      double actualTestStatistic = 0.0;
      double actualPValue = 0.0;
      MethodInfo? mi = typeof(NonOverlapping).GetMethod("TestForEachBlock", BindingFlags.Static | BindingFlags.NonPublic);
      object[] args = new object[] { bitStream, blockCount, templateLength,
            template, sigLevel, actualTestStatistic, actualPValue };

      //
      // Action
      //
      bool? actual = (bool?)(mi?.Invoke(null, args));
      actualTestStatistic = (double)args[5];
      actualPValue = (double)args[6];

      //
      // Assertions
      //
      Assert.NotNull(mi);
      Assert.True(actual.HasValue);
      Assert.Equal(expected, actual.Value);
      Assert.True(Math.Abs(expectedTestStatistic - actualTestStatistic) < tolerance);
      Assert.True(Math.Abs(expectedPValue - actualPValue) < tolerance);
   }

   [Fact]
   public void TemplateLibrary_Ctor()
   {
      // Get the type of the TemplateLibrary internal class.
      Type t = Assembly.GetAssembly(typeof(NonOverlapping))!
                .GetTypes()
                .Where(t => t.IsClass && t.Name == "TemplateLibrary").First();

      // Get the constructor taking one int as an argument
      ConstructorInfo? ci = t.GetConstructor(new[] { typeof(int) });

      // Expected sequence
      int[] expectedTemplates = new int[] { 1, 3, 4, 6 };

      //
      // Action: invoke the constructor
      //
      object o = ci!.Invoke(new object[] { 3 });

      //
      // Assertions
      //
      IEnumerable<int>? j = o as IEnumerable<int>;
      Assert.NotNull(j);
      Assert.NotEmpty(j);
      Assert.True(expectedTemplates.SequenceEqual(j));
   }

   public static TheoryData<int, int> TemplateLibrary_Ctor2TestData
   {
      get
      {
         TheoryData<int, int> rv = new();

         rv.Add(148, 9);
         rv.Add(284, 10);

         return rv;
      }
   }

   [Theory]
   [MemberData(nameof(TemplateLibrary_Ctor2TestData))]
   public void TemplateLibrary_Ctor2(int expectedCount, int templateLength)
   {
      // Get the type of the TemplateLibrary internal class.
      Type t = Assembly.GetAssembly(typeof(NonOverlapping))!
                .GetTypes()
                .Where(t => t.IsClass && t.Name == "TemplateLibrary").First();

      // Get the constructor taking one int as an argument
      ConstructorInfo? ci = t.GetConstructor(new[] { typeof(int) });

      //
      // Action: invoke the constructor
      //
      object o = ci!.Invoke(new object[] { templateLength });

      //
      // Assertions
      //
      IEnumerable<int>? j = o as IEnumerable<int>;
      Assert.NotNull(j);
      Assert.Equal(expectedCount, j.Count());
   }

   public static TheoryData<bool, int, int> TemplateLibrary_CheckPeriodicTestData
   {
      get
      {
         TheoryData<bool, int, int> rv = new();

         rv.Add(false, 3, 3);
         rv.Add(true, 3, 5);

         return rv;
      }
   }

   [Theory]
   [MemberData(nameof(TemplateLibrary_CheckPeriodicTestData))]
   public void TemplateLibrary_CheckPeriodic(bool expected, int templateLength, int template)
   {
      // Get the type of the TemplateLibrary internal class.
      Type t = Assembly.GetAssembly(typeof(NonOverlapping))!
                .GetTypes()
                .Where(t => t.IsClass && t.Name == "TemplateLibrary").First();

      // Get the CheckPeriodic method
      MethodInfo? mi = t.GetMethod("CheckPeriodic", BindingFlags.Static | BindingFlags.NonPublic);

      //
      // Action:
      //
      bool? actual = (bool?)(mi?.Invoke(null, new object[] { template, templateLength }));

      //
      // Assertions
      //
      Assert.NotNull(mi);
      Assert.True(actual.HasValue);
      Assert.Equal(expected, actual.Value);
   }
}