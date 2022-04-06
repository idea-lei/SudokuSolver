using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SukudoSolver.DataType
{
    internal class Game
    {
        public bool Initalized { get; init; }

        public Unit[,] GameBoard { get; init; }

        public Game(int?[,] givenBoard)
        {
            if (givenBoard.GetLength(0) != 9 || givenBoard.GetLength(1) != 9)
            {
                Initalized = false;
                return;
            }

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
