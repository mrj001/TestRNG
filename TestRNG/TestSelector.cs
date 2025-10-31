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

   /// <summary>
   /// Specifies the Longest Run of Ones in a Block test (Section 2.4 of Ref. A)
   /// </summary>
   LongestRun,

   /// <summary>
   /// Specifies the Binary Matrix Rank test (Section 2.5 of Ref. A)
   /// </summary>
   MatrixRank,

   /// <summary>
   /// Specifies the Discrete Fourier Transform (Spectral) Test
   /// (Section 2.6 of Ref. A)
   /// </summary>
   Spectral,

   /// <summary>
   /// Specifies the Non-overlapping Template Test (Section 2.7 of Ref. A)
   /// </summary>
   NonOverlapping,

   /// <summary>
   /// Specifies the Overlapping Template Matching Test (Section 2.8 of Ref. A)
   /// </summary>
   Overlapping,

   /// <summary>
   /// Specifies Maurer's Universal Statistical Test (Section 2.9 of Ref. A)
   /// </summary>
   Maurer,

   /// <summary>
   /// Specifies the Linear Complexity Test (Section 2.10 of Ref. A)
   /// </summary>
   Linear,

   /// <summary>
   /// Specifies the Serial Test (Section 2.11 of Ref. A)
   /// </summary>
   Serial,

   /// <summary>
   /// Specifies the Approximate Entropy Test (Section 2.12 of Ref. A)
   /// </summary>
   Entropy,
}