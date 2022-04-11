using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Solver
{
    internal static class Helper
    {
        // flatten a 2D Array
        public static IEnumerable<T> Flatten<T>(this T[,] map)
        {
            for (int row = 0; row < map.GetLength(0); row++)
                for (int col = 0; col < map.GetLength(1); col++)
                    yield return map[row, col];
        }
    }
}
