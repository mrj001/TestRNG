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
using System.Text;

namespace TestRNG.Utility;

public static class UtilityMethods
{
   /// <summary>
   /// 
   /// </summary>
   /// <param name="formattedValues">First index is the row; second index is the column.  The first row contains the headers.</param>
   public static void PrintTable(string[,] formattedValues)
   {
      string colSeparator = " | ";

      // Determine column widths
      int[] columnWidths = new int[formattedValues.GetLength(1)];
      for (int col = 0; col < formattedValues.GetLength(1); col++)
         for (int rw = 0; rw < formattedValues.GetLength(0); rw++)
            columnWidths[col] = Math.Max(columnWidths[col], formattedValues[rw, col].Length);

      // Print header row
      for (int col = 0; col < formattedValues.GetLength(1); col++)
      {
         if (col > 0)
            Console.Write(colSeparator);
         Console.Write(formattedValues[0, col].PadLeft(columnWidths[col]));
      }
      Console.WriteLine();

      // Print header row separator
      StringBuilder sb = new();
      for (int col = 0; col < formattedValues.GetLength(1); col++)
      {
         if (col > 0)
            sb.Append("-+-");
         for (int j = 0; j < columnWidths[col]; j++)
            sb.Append('-');
      }
      Console.WriteLine(sb);

      // Print table Content
      for (int rw = 1; rw < formattedValues.GetLength(0); rw++)
      {
         for (int col = 0; col < formattedValues.GetLength(1); col++)
         {
            if (col > 0)
               Console.Write(colSeparator);
            Console.Write(formattedValues[rw, col].PadLeft(columnWidths[col]));
         }
         Console.WriteLine();
      }
   }
}