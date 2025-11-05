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
using System.Linq;

namespace TestRNG.Statistics;

/// <summary>
/// This class supports evaluating the results of many runs per Section 4.2.1 and 4.2.2
/// of Ref. A.
/// </summary>
public static class Combining
{

   public enum PassingProportionResult
   {
      /// <summary>
      /// This value indicates that the combined test results fail due to the 
      /// proportion of passing pValues being too low.
      /// </summary>
      FailTooLow,

      /// <summary>
      /// This value indicates that the combined test results pass - the proportion
      /// of passing values is within the expected range.
      /// </summary>
      Pass,

      /// <summary>
      /// This value indicates that the combined test results fail due to more than
      /// the expected number of passing results.
      /// </summary>
      FailTooHigh
   }

   /// <summary>
   /// Checks if the proportion of sequences passing a test is acceptable.
   /// </summary>
   /// <param name="pass">A sequence of boolean values, one per test run, indicating whether each test run passed or failed.</param>
   /// <param name="sigLevel">The significance level of the tests.</param>
   /// <param name="minAcceptable">Returns the minimum acceptable proportion of passing sequences.</param>
   /// <param name="maxAcceptable">Returns the maximum acceptable proportion of passing sequences.</param>
   /// <param name="observedProportion">returns the fraction of the pass values that passed.</param>
   /// <returns></returns>
   /// <remarks>
   /// <para>
   /// This implements the test specified in Section 4.2.1 of Ref. A.
   /// </para>
   /// </remarks>
   public static PassingProportionResult PassingProportion(bool[] pass, double sigLevel,
            out double minAcceptable, out double maxAcceptable, out double observedProportion)
   {
      int repeatCount = pass.Length;
      int successCount = pass.Where(x => x).Count();
      double pHat = 1.0 - sigLevel;
      double t = 3.0 * Math.Sqrt(pHat * (1 - pHat) / repeatCount);
      minAcceptable = pHat - t;
      maxAcceptable = pHat + t;
      observedProportion = ((double)successCount) / repeatCount;

      if (observedProportion >= minAcceptable)
      {
         if (observedProportion <= maxAcceptable)
         {
            return PassingProportionResult.Pass;
         }
         else
         {
            return PassingProportionResult.FailTooHigh;
         }
      }
      else  // observedProportion < minAcceptable
      {
         return PassingProportionResult.FailTooLow;
      }
   }

   /// <summary>
   /// Convenience method that combines a typical call to PassingProportion with output
   /// of the results.
   /// </summary>
   /// <param name="tw">A TextWriter to which output is sent.</param>
   /// <param name="pass">A sequence of boolean values, one per test run, indicating whether each test run passed or failed.</param>
   /// <param name="sigLevel">The significance level of the tests.</param>
   public static void CombinePassingResults(TextWriter tw, bool[] pass, double sigLevel)
   {
      double minAcceptable;
      double maxAcceptable;
      double observedProportion;
      tw.WriteLine("RESULTS:");
      PassingProportionResult proportionResult = PassingProportion(pass,
               sigLevel, out minAcceptable, out maxAcceptable, out observedProportion);
      tw.WriteLine($"Acceptable proportion of passing sequences is from {minAcceptable:0.000000} to {maxAcceptable:0.000000}");
      tw.WriteLine($"Observed proportion: {observedProportion:0.000000}");
      tw.WriteLine($"Result: {proportionResult}");
      tw.WriteLine();
   }

   /// <summary>
   /// This test checks for a uniform distribution of pValues resulting from 
   /// multiple runs of any given test.
   /// </summary>
   /// <returns>True if the distribution of pValues looks uniform; false if not.</returns>
   /// <remarks>
   /// <param name="pValues">An array containing the pValues from many runs of a test.</param>
   /// <param name="sigLevel">The significance level at which the tests were conducted.</param>
   /// <param name="chiSquared">The calculated test statistic of the uniformity test.</param>
   /// <param name="pValue">The calculated pValue of the uniformity test.</param>
   /// <para>
   /// This implements the test specified in Section 4.2.2 of Ref. A.
   /// </para>
   /// </remarks>
   public static bool HistogramUniformity(double[] pValues, double sigLevel, out double chiSquared, out double pValue)
   {
      int binCount = 10;
      int repeatCount = pValues.Length;

      // Form the Histogram
      int[] histogram = new int[binCount];
      for (int j = 0; j < repeatCount; j++)
      {
         int bin = (int)Math.Floor(pValues[j] * binCount);
         histogram[bin]++;
      }

      // Do a Chi-Squared test on the Histogram.
      chiSquared = 0.0;
      for (int j = 0; j < binCount; j++)
      {
         double expected = ((double)repeatCount) / binCount;
         double t = (histogram[j] - expected);
         chiSquared += t * t / expected;
      }
      pValue = Gamma.IncompleteGammaQ((binCount - 1.0) / 2.0, chiSquared / 2.0);

      // TODO.  It's not clear where Section 4.2.2 of Ref. A gets this value.  It
      // could simply be a desire to have a very low risk of a Type I error, or
      // it could be the square of their typical significance level.  In any
      // case, the value used in Ref. A is hard-coded here.
      return pValue >= 0.0001;
   }

   /// <summary>
   /// A convenience method that combines a typical call to HistogramUniformity with
   /// output of the results.
   /// </summary>
   /// <param name="tw">A TextWriter to which output is sent.</param>
   /// <param name="pValues">An array containing the pValues from many runs of a test.</param>
   /// <param name="sigLevel">The significance level at which the tests were conducted.</param>
   public static void CheckHistogramOfPValues(TextWriter tw, double[] pValues, double sigLevel)
   {
      tw.WriteLine("Checking histogram for uniformity:");
      double chiSquared;
      double uniformityPValue;
      bool uniformityResult = HistogramUniformity(pValues, sigLevel, out chiSquared, out uniformityPValue);
      tw.WriteLine($"Chi-Squared: {chiSquared:0.000000}");
      tw.WriteLine($"Uniformity p-Value: {uniformityPValue:0.000000}");
      if (uniformityResult)
         tw.WriteLine("p-Values are uniformly distributed.");
      else
         tw.WriteLine("p-Values are NOT uniformly distributed");
   }
}