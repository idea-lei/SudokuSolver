using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver.DataType
{
    public class Game
    {
        public bool Initalized { get; private set; }

        public Unit[,] GameBoard { get; private set; }

        public void InitBoard(int?[,] givenBoard)
        {
            Initalized = false;
            if (givenBoard.GetLength(0) != 9 || givenBoard.GetLength(1) != 9) return;

            GameBoard = new Unit[9, 9];

            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                {
                    GameBoard[i, j] = new Unit()
                    {
                        Given = givenBoard[i, j],
                        Coordinate = (i, j),
                        Game = this
                    };
                }

            Initalized = true;
        }
    }
}
