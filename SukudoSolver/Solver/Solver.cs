using SukudoSolver.DataType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SukudoSolver.Solver
{
    public static class Solver
    {
        /// <summary>
        /// to update possible values of a *unknown* unit
        /// </summary>
        /// <param name="unit"></param>
        public static void FindPossibleValuesForUnit(this Unit unit)
        {
            if (unit.CurrentValue != null) return;
            HashSet<int> apprentValues = new();

            // check row
            for (int i = 0; i < 9; i++)
            {
                int? checkedUnitValue = unit.Game.GameBoard[i, unit.Coordinate.Item2].CurrentValue;
                if (checkedUnitValue.HasValue)
                    apprentValues.Add(checkedUnitValue.Value);
            }

            // check column
            for (int j = 0; j < 9; j++)
            {
                int? checkedUnitValue = unit.Game.GameBoard[unit.Coordinate.Item1, j].CurrentValue;
                if (checkedUnitValue.HasValue)
                    apprentValues.Add(checkedUnitValue.Value);
            }

            // check for block
            for (int i = unit.BlockOrigin.Item1; i < unit.BlockOrigin.Item1 + 3; i++)
                for (int j = unit.BlockOrigin.Item2; j < unit.BlockOrigin.Item2 + 3; j++)
                {
                    int? checkedUnitValue = unit.Game.GameBoard[i, j].CurrentValue;
                    if (checkedUnitValue.HasValue)
                        apprentValues.Add(checkedUnitValue.Value);
                }

            unit.PossibleValues.Clear();
            foreach (int v in Enumerable.Range(1, 9).Except(apprentValues))
                unit.PossibleValues.Add(v);
        }

        public static bool HasConflict(this Unit unit)
        {
            return unit.PossibleValues.Count == 0;
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
                unit.Game.GameBoard[i, unit.Coordinate.Item2].PossibleValues.Remove(unit.CurrentValue.Value);

            // check column
            for (int j = 0; j < 9; j++)
                unit.Game.GameBoard[unit.Coordinate.Item1, j].PossibleValues.Remove(unit.CurrentValue.Value);

            // check for block
            for (int i = unit.BlockOrigin.Item1; i < unit.BlockOrigin.Item1 + 3; i++)
                for (int j = unit.BlockOrigin.Item2; j < unit.BlockOrigin.Item2 + 3; j++)
                    unit.Game.GameBoard[i, j].PossibleValues.Remove(unit.CurrentValue.Value);
        }
    }
}
