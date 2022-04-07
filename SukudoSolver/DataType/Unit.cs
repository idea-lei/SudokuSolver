using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SukudoSolver.DataType
{
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
            get => _answer; set
            {
                _answer = value;
                OnAnswerChanged?.Invoke();
            }
        }

        /// <summary>
        /// assume value for the unit
        /// </summary>
        public int? Assumption { get; set; }

        /// <summary>
        /// possible values for the unit
        /// </summary>
        // todo delete the init values
        public List<int> PossibleValues { get; } = new() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
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
        public event UnitEventHandler OnAssumptionChanged = () => { };
        public event UnitEventHandler OnAnswerChanged = () => { };
    }
}
