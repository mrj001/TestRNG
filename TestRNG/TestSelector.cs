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

namespace TestRNG;

public enum TestSelector
{
   /// <summary>
   /// Specifies the Uniform Test.
   /// </summary>
   Uniform,

   /// <summary>
   /// Specifies the Frequency(Monobit) Test.
   /// </summary>
   Monobit,

   /// <summary>
   /// Specifies the Frequency Test within a Block.
   /// </summary>
   FrequencyBlock,

   /// <summary>
   /// Specifies the Runs test (Section 2.3 of Ref. A)
   /// </summary>
   Runs,
}