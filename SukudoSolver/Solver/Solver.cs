using SudokuSolver.DataType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.Solver
{
    public static class Solver
    {
        public static bool HasConflict(this Unit unit)
        {
            return unit.GetPossibleValues().Length == 0 && unit.CurrentValue == null;
        }

        public static void UpdateAnswerWhenOnlyOnePossibleValue(this Unit unit)
        {
            var possibleValues = unit.GetPossibleValues();
            if (possibleValues.Length == 1)
            {
                unit.Answer = possibleValues[0];
                unit.ClearPossibleValues();
            }
        }

        /// <summary>
        /// after updating the current value of the unit, update relevant units  possible values (in row, column and block)
        /// </summary>
        /// <param name="unit"></param>
        public static void UpdatePossiableValuesForRelevantUnits(this Unit unit)
        {
            if (!unit.CurrentValue.HasValue) return;

            // check row
            for (int i = 0; i < 9; i++)
                unit.Game.GameBoard[i, unit.Coordinate.Item2].RemovePossibleValues(unit.CurrentValue.Value);

            // check column
            for (int j = 0; j < 9; j++)
                unit.Game.GameBoard[unit.Coordinate.Item1, j].RemovePossibleValues(unit.CurrentValue.Value);

            // check for block
            for (int i = unit.BlockOrigin.Item1; i < unit.BlockOrigin.Item1 + 3; i++)
                for (int j = unit.BlockOrigin.Item2; j < unit.BlockOrigin.Item2 + 3; j++)
                    unit.Game.GameBoard[i, j].RemovePossibleValues(unit.CurrentValue.Value);
        }
    }
}
