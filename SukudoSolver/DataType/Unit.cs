using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SukudoSolver.DataType
{
    public enum UnitValueType
    {
        Given,
        Answer,
        Assumption,
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
        public int? Given { get; init; }

        /// <summary>
        /// confirmed answer, could be more than one possibility
        /// the sequence should be identical to other unit
        /// </summary>
        public List<int> Answers { get; } = new();

        /// <summary>
        /// Current Answer
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
                    OnCurrentValueChanged?.Invoke();
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
                return UnitValueType.None;
            }
        }

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

        public int? CurrentValue
        {
            get
            {
                if (Given != null) return Given;
                if (Answer != null) return Answer;
                return Assumption;
            }
        }

        public Game Game { get; init; }

        public delegate void UnitEventHandler();
        public event UnitEventHandler? OnCurrentValueChanged;
        public event UnitEventHandler? OnPossibleValuesChanged;

        public void Reset()
        {
            Assumption = null;
            Answers.Clear();
            Answer = null;
            _possibleValues.Clear();

            if (OnCurrentValueChanged != null)
                foreach (UnitEventHandler d in OnCurrentValueChanged.GetInvocationList())
                    OnCurrentValueChanged -= d;

            if (OnPossibleValuesChanged != null)
                foreach (UnitEventHandler d in OnPossibleValuesChanged.GetInvocationList())
                    OnPossibleValuesChanged -= d;
        }
    }
}
