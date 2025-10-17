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
using TestRNG.Statistics;

namespace TestRNG.Tests;

public static class Spectral
{
   public static bool Test(IRandom random, ref int callCount, double sigLevel, out double testStatistic, out double pValue)
   {
      if (callCount < CommandLineArgs.MINIMUM_SPECTRAL_CALL_COUNT)
         callCount = CommandLineArgs.MINIMUM_SPECTRAL_CALL_COUNT;
      else
         callCount = NextPowerOfTwo(callCount);

      // Gather the random data
      double[] data = new double[1 + 2 * callCount];
      for (int j = 1; j <= callCount; j++)
         data[2 * j] = random.NextBit() ? 1 : -1;

      // Transform the data
      FFT(data, callCount, 1);

      double threshold = Math.Sqrt(Math.Log(1 / 0.05) * callCount);

      // The expected number of peaks that are less than the threshold.
      double n0 = 0.95 * callCount / 2.0;

      // Count the observed number of peaks that are less than the threshold.
      int n1 = 0;
      for (int j = 1; j <= callCount; j += 2)
      {
         double magnitude = Math.Sqrt(data[j] * data[j] + data[j + 1] * data[j + 1]);
         if (magnitude < threshold)
            n1++;
      }

      testStatistic = (n1 - n0) / Math.Sqrt(callCount * 0.95 * 0.05 / 4);
      pValue = Normal.ComplementaryErrorFunction(Math.Abs(testStatistic) / Math.Sqrt(2.0));

      return pValue >= sigLevel;
   }

   private static int NextPowerOfTwo(int v)
   {
      v--;
      v |= v >> 1;
      v |= v >> 2;
      v |= v >> 4;
      v |= v >> 8;
      v |= v >> 16;
      v++;

      return v;
   }

   /// <summary>
   /// The Fast Fourier Transform from chapter 12 of Ref. C.
   /// </summary>
   /// <param name="data">A unit-based array of complex values to transform.
   /// The element at index 0 is ignored.</param>
   /// <param name="nn">The total number of complex values in data (1/2 the length of the array).</param>
   /// <param name="isign"></param>
   /// <remarks>
   /// <para>
   /// Replaces data[1..2 * nn] by its discrete Fourier transform, if isign is input as 1; or 
   /// replaces data[1..2 * nn] by nn times its inverse discrete Fourier transform, if isign is input as −1. 
   /// data is a complex array of length nn or, equivalently, a real array of 
   /// length 2*nn.  nn MUST be an integer power of 2 (this is not checked for!).
   /// </para>
   /// </remarks>
   private static void FFT(double[] data, long nn, int isign)
   {
      long n, mmax, m, j, istep, i;
      double wtemp, wr, wpr, wpi, wi, theta;
      double tempr, tempi;
      n = nn << 1;
      j = 1;
      for (i = 1; i < n; i += 2)
      {
         if (j > i)
         {
            double t = data[j];
            data[j] = data[i];
            data[i] = t;
            t = data[j + 1];
            data[j + 1] = data[i + 1];
            data[i + 1] = t;
         }
         m = nn;
         while (m >= 2 && j > m)
         {
            j -= m;
            m >>= 1;
         }
         j += m;
      }

      // Here begins the Danielson-Lanczos section of the routine.
      // Outer loop executed log2 nn times.
      double twoPi = Math.Tau;
      mmax = 2;
      while (n > mmax)
      {
         istep = mmax << 1;

         // Initialize the trigonometric recurrence.
         theta = isign * (twoPi / mmax);
         wtemp = Math.Sin(0.5 * theta);
         wpr = -2.0 * wtemp * wtemp;

         wpi = Math.Sin(theta);
         wr = 1.0;
         wi = 0.0;

         // Here are the two nested inner loops.
         for (m = 1; m < mmax; m += 2)
         {
            for (i = m; i <= n; i += istep)
            {
               j = i + mmax;

               // This is the Danielson - Lanczos for-mula:
               tempr = wr * data[j] - wi * data[j + 1];
               tempi = wr * data[j + 1] + wi * data[j];
               data[j] = data[i] - tempr;
               data[j + 1] = data[i + 1] - tempi;
               data[i] += tempr;
               data[i + 1] += tempi;
            }

            // Trigonometric recurrence.
            wr = (wtemp = wr) * wpr - wi * wpi + wr;
            wi = wi * wpr + wtemp * wpi + wi;
         }
         mmax = istep;
      }
   }
}