using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.DataType
{
    /// <summary>
    /// the node to start an assumption
    /// !!! It's not a tree since there is no need to traverse it, only need the last one
    /// </summary>
    public class AssumptionNode
    {
        public bool Initialized { get; init; } = false;
        public (int, int) Position { get; init; }
        public Game GameBeforeAssumption { get; init; }
        public Assumption[] Assumptions { get; init; }
        
        public AssumptionNode(Game game)
        {
            GameBeforeAssumption = game;
            if(!GameBeforeAssumption.InitPossibleValues()) return;
            
            // the first unit with min possible value count
            Unit? mU = null;
            foreach (var u in game)
            {
                if (u?.CurrentValue != null) continue;
                if (mU == null ||
                    mU?.GetPossibleValues().Length > u?.GetPossibleValues().Length)
                {
                    mU = u;
                    continue;
                }
            }
            if (mU == null) 
                throw new Exception("No unit with min possible value count");
            
            Position = mU.Position;
            int[] assumptionValues = mU.GetPossibleValues();
            Assumptions = new Assumption[assumptionValues.Length];
            for (int i = 0; i < Assumptions.Length; i++)
            {
                var board = game.GetCurrentBoard();
                board[Position.Item1, Position.Item2] = assumptionValues[i];
                Assumptions[i] = new Assumption(assumptionValues[i], board);
            }

            Initialized = true;
        }
    }

    public class Assumption
    {
        public int Value { get; init; }
        public Game GameAfterAssumption { get; init; }

        public Assumption(int value, int?[,] board)
        {
            Value = value;
            GameAfterAssumption = new Game();
            GameAfterAssumption.InitBoard(board);
        }
    }
}
