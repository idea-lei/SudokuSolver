using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SukudoSolver.DataType
{
    internal class Unit
    {
        /// <summary>
        /// coordinate of the unit in the game board
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

        public int? CurrentAnswer { get; set; }

        /// <summary>
        /// assume value for the unit
        /// </summary>
        public int? Assumption { get; set; }

        /// <summary>
        /// possible values for the unit
        /// </summary>
        public List<int> PossibleValues { get; } = new();

        public int? CurrentValue
        {
            get
            {
                if (Given != null) return Given;
                if (CurrentAnswer != null) return CurrentAnswer;
                return Assumption;
            }
        }

        public Game Game { get; init; }
    }
}
