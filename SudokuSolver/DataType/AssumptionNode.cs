using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.DataType
{
    public class AssumptionNode
    {
        public (int, int) Position { get; init; }
        public int[] Assumptions { get; init; }
        public Game[] Games { get; }
        public AssumptionNode(Unit unit, Game game)
        {
            Position = unit.Position;
            Assumptions = unit.GetPossibleValues();
            Games = new Game[Assumptions.Length];
            for (int i = 0; i < Assumptions.Length; i++)
            {
            }
        }
    }
}
