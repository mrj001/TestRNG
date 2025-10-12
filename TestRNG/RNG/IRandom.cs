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

namespace TestRNG.RNG;

/// <summary>
/// An interface to support injection of various RNG implementations into 
/// test methods.
/// </summary>
/// <remarks>
/// <para>
/// This is largely based upon a subset of the methods of System.Random.
/// </para>
/// </remarks>
public interface IRandom
{
   /// <summary>
   /// Gets the next bit from a random stream of bits.
   /// </summary>
   /// <returns>A random bit.</returns>
   public bool NextBit();

   /// <summary>
   /// Returns a random integer that is greater than or equal to zero and strictly less than maxValue.
   /// </summary>
   /// <param name="maxValue"></param>
   /// <returns></returns>
   public int Next(int maxValue);

}