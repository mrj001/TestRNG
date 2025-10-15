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
using System.Collections;
using System.Diagnostics;
using System.Linq;

namespace TestRNG.Utility;

/// <summary>
/// A square Matrix modulo 2, which is implemented by storing one bit per entry
/// of the Matrix.
/// </summary>
public class Matrix
{
   private readonly int _size;
   private readonly BitArray[] _rows;

   /// <summary>
   /// Constructs a new Matrix object, with all entries set to zero.
   /// </summary>
   /// <param name="sze">The number of rows and columns in the new Matrix.</param>
   /// <remarks>
   /// <para>
   /// All bits are initially set to zero (false).
   /// </para>
   /// </remarks>
   public Matrix(int size)
   {
      _size = size;
      _rows = new BitArray[size];
      for (int row = 0; row < size; row++)
         _rows[row] = new BitArray(size);
   }

   [Conditional("DEBUG")]
   private void ValidateRow(int rowIndex)
   {
      if (rowIndex < 0 || rowIndex >= _size)
         throw new ArgumentOutOfRangeException($"{nameof(rowIndex)} = {rowIndex} is out of bounds [0, {_size})");
   }

   [Conditional("DEBUG")]
   private void ValidateColumn(int columnIndex)
   {
      if (columnIndex < 0 || columnIndex >= _size)
         throw new ArgumentOutOfRangeException($"{nameof(columnIndex)} = {columnIndex} is out of bounds [0, {_size}).");
   }

   /// <inheritdoc />
   public bool this[int rowIndex, int columnIndex]
   {
      get
      {
         ValidateRow(rowIndex);
         ValidateColumn(columnIndex);
         return _rows[rowIndex][columnIndex];
      }
      set
      {
         ValidateRow(rowIndex);
         ValidateColumn(columnIndex);
         _rows[rowIndex][columnIndex] = value;
      }
   }

   public int GetRank()
   {
      ReduceForward();

      // Check for zero rows at the bottom.
      int row = _size;
      int popCount;
      do
      {
         row--;
         popCount = _rows[row].Cast<bool>().Where(x => x).Count();
      } while (row > 0 && popCount == 0);

      // For small matrices, there is a realisitic chance that the entire
      // matrix could be zeroes.  In this case, popCount and row will both
      // be zero, and the rank of the matrix will correctly be zero!
      if (popCount > 0) row++;

      return row;
   }

   /// <summary>
   /// Reduces the matrix to Row Echelon Form
   /// </summary>
   private void ReduceForward()
   {
      for (int col = 0, curRow = 0; col < _size && curRow < _size; col++)
      {
         if (!this[curRow, col])
         {
            // find a row below to swap with.
            int rw = curRow + 1;
            while (rw < _size && !this[rw, col])
               rw++;
            if (rw < _size)
            {
               BitArray t = _rows[curRow];
               _rows[curRow] = _rows[rw];
               _rows[rw] = t;
            }
            else
            {
               continue;
            }
         }

         int k = curRow + 1;
         while (k < _size)
         {
            while (k < _size && !this[k, col])
               k++;

            if (k < _size)
            {
               _rows[k].Xor(_rows[curRow]);
               k++;
            }
         }

         curRow++;
      }
   }
}
