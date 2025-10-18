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
using System.Dynamic;

namespace TestRNG.Statistics;

//
// Reference:
// Fischer's Method, Wikipedia
// https://en.wikipedia.org/wiki/Fisher%27s_method
// Accessed on 2025-10-18
//
public class CombinedPValues
{
   private readonly double _sigLevel;
   private readonly double _fischerPValue;
   private readonly bool _fischerPass;
   private readonly double _passRatio;
   private readonly bool _naivePass;

   public CombinedPValues(double[] pValues, double sigLevel)
   {
      double fischerChiSquare = 0.0;
      int passCount = 0;
      int count = 0;

      foreach (double pValue in pValues)
      {
         count++;

         // Count passing values for Naive combination
         if (pValue >= sigLevel)
            passCount++;

         // Sum logarithms for Fischer's Method
         fischerChiSquare += Math.Log(pValue);
      }

      // Finish Naive combination
      _passRatio = ((double)passCount) / count;
      _naivePass = _passRatio >= (1.0 - sigLevel);

      // Finish Fischer's method
      fischerChiSquare *= -2.0;
      _fischerPValue = Gamma.IncompleteGammaQ(count, fischerChiSquare / 2.0);
      _fischerPass = _fischerPValue >= sigLevel;

      _sigLevel = sigLevel;
   }

   /// <summary>
   /// Gets the Significance Level.
   /// </summary>
   public double Significance { get => _sigLevel; }

   /// <summary>
   /// Gets the combined P-Value arrived at via Fischer's Method.
   /// </summary>
   public double FischerPValue { get => _fischerPValue; }

   /// <summary>
   /// Gets whether or not the combined Fischer P-Value passes the Null Hypothesis
   /// </summary>
   public bool FischerPass { get => _fischerPass; }

   /// <summary>
   /// Gets the fraction of the given P-Values that were greater than or equal to the 
   /// given Significance Level.
   /// </summary>
   public double PassRatio { get => _passRatio; }

   /// <summary>
   /// Returns whether or not the set of given P-Values passes a naive combination
   /// of them.
   /// </summary>
   /// <remarks>
   /// <para>
   /// If the proportion of failing p-Values in the given set is less than the 
   /// significance level, the set of p-Values is deemed to have passed.  As there
   /// will normally be some variation in this ratio, it can be expected to falsely
   /// reject the set more often than desirable.  Additionally, if the significance
   /// level is 0.01, then in a set of 99 p-Values, only one failing value will fail
   /// the set.
   /// </para>
   /// </remarks>
   public bool NaivePass { get => _naivePass; }
}