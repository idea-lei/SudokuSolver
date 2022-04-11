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

        public static bool UpdateAnswer_OnlyOnePossibleValue(this Unit unit)
        {
            bool canUpdate = false;
            var possibleValues = unit.GetPossibleValues();
            if (possibleValues.Length == 1)
            {
                unit.Answer = possibleValues[0];
                unit.ClearPossibleValues();
                canUpdate = true;
            }
            return canUpdate;
        }

        public static bool UpdateUnits_OnlyOnePossibleValue(this Game game)
        {
            bool canUpdate = false;
            foreach (var unit in game.GameBoard)
                canUpdate |= unit?.UpdateAnswer_OnlyOnePossibleValue() == true;
            return canUpdate;
        }        

        /// <summary>
        /// call this method to check if a possiable value only appears once in row,
        /// there is no other option for the corresponding unit.
        /// </summary>
        public static bool UpdateAnswer_OnlyOnePossibleValueInRow(this Game game, int row)
        {
            bool canUpdate = false;
            var rowUnits = game.GetRow(row);
            HashSet<int> possibleValues = new();

            // gather all possible values in row
            foreach (var unit in rowUnits)
                foreach (var v in unit.GetPossibleValues())
                    possibleValues.Add(v);

            // count the appearance of each value and update if only one
            foreach (var v in possibleValues)
            {
                var count = rowUnits.Count(u => u.GetPossibleValues().Contains(v));
                if (count == 1)
                {
                    var unit = rowUnits.First(u => u.GetPossibleValues().Contains(v));
                    unit.Answer = v;
                    canUpdate = true;
                }
            }
            return canUpdate;
        }

        /// <summary>
        /// call this method to check if a possiable value only appears once in column,
        /// there is no other option for the corresponding unit.
        /// </summary>
        public static bool UpdateAnswer_OnlyOnePossibleValueInColumn(this Game game, int column)
        {
            bool canUpdate = false;
            var columnUnits = game.GetColumn(column);
            HashSet<int> possibleValues = new();

            // gather all possible values in column
            foreach (var unit in columnUnits)
                foreach (var v in unit.GetPossibleValues())
                    possibleValues.Add(v);

            // count the appearance of each value and update if only one
            foreach (var v in possibleValues)
            {
                var count = columnUnits.Count(u => u.GetPossibleValues().Contains(v));
                if (count == 1)
                {
                    var unit = columnUnits.First(u => u.GetPossibleValues().Contains(v));
                    unit.Answer = v;
                    canUpdate = true;
                }
            }
            return canUpdate;
        }

        public static bool UpdateAnswer_OnlyOnePossibleValueInBlock(this Game game, int row, int column)
        {
            bool canUpdate = false;
            var blockUnits = game.GetBlock(row, column).Flatten();
            HashSet<int> possibleValues = new();

            // gather all possible values in block
            foreach (var unit in blockUnits)
                foreach (var v in unit.GetPossibleValues())
                    possibleValues.Add(v);

            // count the appearance of each value and update if only one
            foreach (var v in possibleValues)
            {
                var count = blockUnits.Count(u => u.GetPossibleValues().Contains(v));
                if (count == 1)
                {
                    var unit = blockUnits.First(u => u.GetPossibleValues().Contains(v));
                    unit.Answer = v;
                    canUpdate = true;
                }
            }
            return canUpdate;
        }

        /// <summary>
        /// after updating the current value of the unit, update relevant units possible values (in row, column and block)
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
