using SudokuSolver.DataType;

namespace SudokuSolver.Solver
{
    public static class Solver
    {
        public static bool HasConflict(this Game game)
        {
            foreach (var unit in game.GameBoard)
                if (unit.HasConflict())
                    return true;
            return false;
        }

        public static bool IsSolved(this Game game)
        {
            foreach (var unit in game.GameBoard)
                if (unit.CurrentValue == null)
                    return false;
            return true;
        }

        /// <summary>
        /// update the unit if only one possible value in it
        /// </summary>
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

        private static bool UpdateAnswer_OnlyOnePossibleValue(Unit[] units)
        {
            HashSet<int> possibleValues = new();
            bool canUpdate = false;

            foreach (var unit in units)
                foreach (var v in unit.GetPossibleValues())
                    possibleValues.Add(v);

            // count the appearance of each value and update if only one
            foreach (var v in possibleValues)
            {
                var count = units.Count(u => u.GetPossibleValues().Contains(v));
                if (count == 1)
                {
                    var unit = units.First(u => u.GetPossibleValues().Contains(v));
                    unit.Answer = v;
                    canUpdate = true;
                }
            }
            return canUpdate;
        }

        /// <summary>
        /// call this method to check if a possiable value only appears once in row
        /// </summary>
        public static bool UpdateAnswer_OnlyOnePossibleValueInRow(this Game game, int row) =>
            UpdateAnswer_OnlyOnePossibleValue(game.GetRow(row));

        /// <summary>
        /// call this method to check if a possiable value only appears once in column
        /// </summary>
        public static bool UpdateAnswer_OnlyOnePossibleValueInColumn(this Game game, int column) =>
            UpdateAnswer_OnlyOnePossibleValue(game.GetColumn(column));

        /// <summary>
        /// call this method to check if a possiable value only appears once in block
        /// </summary>
        public static bool UpdateAnswer_OnlyOnePossibleValueInBlock(this Game game, int row, int column) =>
            UpdateAnswer_OnlyOnePossibleValue(game.GetBlock(row, column).Flatten().ToArray());
    }
}
