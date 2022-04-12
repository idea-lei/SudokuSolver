namespace SudokuSolver.DataType
{
    public enum UnitValueType
    {
        Given,
        Answer,
        OptionalAnswer,
        Assumption,
        Conflict,
        None
    }

    public class Unit
    {
        /// <summary>
        /// coordinate of the unit in the game board
        /// item1 -> x (row)
        /// item2 -> y (column)
        /// </summary>
        public (int, int) Coordinate { get; init; }

        /// <summary>
        /// indicates the origin unit coordinate of the 3x3 block
        /// </summary>
        public (int, int) BlockOrigin => (
            Coordinate.Item1 / 3 * 3,
            Coordinate.Item2 / 3 * 3);

        /// <summary>
        /// init given value, null for empty (would not use 0 for empty, it could cause misunderstanding)
        /// </summary>
        public int? Given { get; set; }

        /// <summary>
        /// one of the possible answer from answer tree
        /// </summary>
        /// <remarks>
        /// check AssumptionNode for more info
        /// </remarks>
        public int? OptionalAnswer { get; set; }

        /// <summary>
        /// The only Answer
        /// </summary>
        private int? _answer;
        public int? Answer
        {
            get => _answer;
            set
            {
                bool euqal = CurrentValue == value;
                _answer = value;
                if (!euqal)
                {
                    ClearPossibleValues();
                    OnCurrentValueChanged?.Invoke();
                }
            }
        }

        /// <summary>
        /// assume value for the unit
        /// </summary>
        private int? _assumption;
        public int? Assumption
        {
            get => _assumption;
            set
            {
                bool euqal = CurrentValue == value;
                _assumption = value;
                if (!euqal)
                    OnCurrentValueChanged?.Invoke();
            }
        }

        public UnitValueType UnitValueType
        {
            get
            {
                if (Given.HasValue) return UnitValueType.Given;
                if (Answer.HasValue) return UnitValueType.Answer;
                if (Assumption.HasValue) return UnitValueType.Assumption;
                if (OptionalAnswer.HasValue) return UnitValueType.OptionalAnswer;
                if (HasConflict()) return UnitValueType.Conflict;
                return UnitValueType.None;
            }
        }

        public int? CurrentValue
        {
            get
            {
                if (Given.HasValue) return Given;
                if (Answer.HasValue) return Answer;
                if (Assumption.HasValue) return Assumption;
                if (OptionalAnswer.HasValue) return OptionalAnswer;
                return null;
            }
        }

        public Game Game { get; init; }

        public event Action? OnCurrentValueChanged;
        public event Action? OnPossibleValuesChanged;

        /// <summary>
        /// possible values for the unit
        /// </summary>
        /// <remarks>
        /// if both current value and possible values are empty, means the layout is unsolvable
        /// </remarks>
        private HashSet<int> _possibleValues = new();

        public int[] GetPossibleValues() => _possibleValues.ToArray();
        public void SetPossibleValues(params int[] values)
        {
            bool changed = false;
            foreach (int v in values)
                if (_possibleValues.Add(v))
                    changed = true;
            if (changed)
                OnPossibleValuesChanged?.Invoke();
        }
        public void RemovePossibleValues(params int[] values)
        {
            bool changed = false;
            foreach (int v in values)
                if (_possibleValues.Remove(v))
                    changed = true;
            if (changed)
                OnPossibleValuesChanged?.Invoke();
        }
        public void ClearPossibleValues()
        {
            if (_possibleValues.Count > 0)
            {
                _possibleValues.Clear();
                OnPossibleValuesChanged?.Invoke();
            }
        }
        public void InitPossibleValues()
        {
            if (CurrentValue != null) return;
            HashSet<int> apprentValues = new();

            // check row
            for (int i = 0; i < 9; i++)
            {
                int? checkedUnitValue = Game.GameBoard[i, Coordinate.Item2].CurrentValue;
                if (checkedUnitValue.HasValue)
                    apprentValues.Add(checkedUnitValue.Value);
            }

            // check column
            for (int j = 0; j < 9; j++)
            {
                int? checkedUnitValue = Game.GameBoard[Coordinate.Item1, j].CurrentValue;
                if (checkedUnitValue.HasValue)
                    apprentValues.Add(checkedUnitValue.Value);
            }

            // check for block
            for (int i = BlockOrigin.Item1; i < BlockOrigin.Item1 + 3; i++)
                for (int j = BlockOrigin.Item2; j < BlockOrigin.Item2 + 3; j++)
                {
                    int? checkedUnitValue = Game.GameBoard[i, j].CurrentValue;
                    if (checkedUnitValue.HasValue)
                        apprentValues.Add(checkedUnitValue.Value);
                }

            _possibleValues.Clear();
            foreach (int v in Enumerable.Range(1, 9).Except(apprentValues))
                _possibleValues.Add(v);
            OnPossibleValuesChanged?.Invoke();
        }

        public bool HasConflict() =>
            GetPossibleValues().Length == 0 && CurrentValue == null;

        public void Reset()
        {
            Assumption = null;
            Answer = null;
            _possibleValues.Clear();

            if (OnCurrentValueChanged != null)
                foreach (Action d in OnCurrentValueChanged.GetInvocationList())
                    OnCurrentValueChanged -= d;

            if (OnPossibleValuesChanged != null)
                foreach (Action d in OnPossibleValuesChanged.GetInvocationList())
                    OnPossibleValuesChanged -= d;
        }
    }
}
