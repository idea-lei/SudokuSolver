using System.Collections;
using System.Text;

namespace SudokuSolver.DataType;

public class Game: IEnumerable<Unit>
{
    public bool Initalized { get; private set; }

    public Unit[,] GameBoard { get; private set; }

    public Unit? this[int x, int y] => GameBoard?[x, y];

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
                    Position = (i, j),
                    Game = this
                };
            }

        Initalized = true;
    }

    /// <returns>true if success, false if has conflict</returns>
    public bool InitPossibleValues()
    {
        foreach (var unit in this)
        {
            unit.InitPossibleValues();
            if (unit.HasConflict() == true)
                return false;
        }
        return true;
    }

    public Unit[] GetRow(int row)
    {
        if (!Initalized)
            throw new Exception("Game not initalized");
        if (row < 0 || row > 8)
            throw new Exception("Row out of range");

        Unit[] rowUnits = new Unit[9];
        for (int i = 0; i < 9; i++)
            rowUnits[i] = GameBoard[row, i];

        return rowUnits;
    }

    public Unit[] GetColumn(int column)
    {
        if (!Initalized)
            throw new Exception("Game not initalized");
        if (column < 0 || column > 8)
            throw new Exception("Column out of range");

        Unit[] columnUnits = new Unit[9];
        for (int i = 0; i < 9; i++)
            columnUnits[i] = GameBoard[i, column];

        return columnUnits;
    }

    /// <param name="row">block row index, not unit row index</param>
    /// <param name="column">block column index, not unit column index</param>
    public Unit[,] GetBlock(int row, int column)
    {
        if (!Initalized)
            throw new Exception("Game not initalized");
        if (row < 0 || row > 3 || column < 0 || column > 3)
            throw new Exception("Invalid block index");

        Unit[,] blockUnits = new Unit[3, 3];
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
                blockUnits[i, j] = GameBoard[row * 3 + i, column * 3 + j];

        return blockUnits;
    }

    public int?[,] GetCurrentBoard() {
        int?[,] board = new int?[9, 9];
        for (int i = 0; i < 9; i++)
            for (int j = 0; j < 9; j++)
                board[i, j] = GameBoard[i, j].CurrentValue;
        return board;
    }

    // only for given value, replace with 0 if not given
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
                sb.Append(GameBoard[i, j].Given?.ToString() ?? "0");
            sb.AppendLine();
        }
        return sb.ToString();
    }

    public IEnumerator<Unit> GetEnumerator()
    {
        for (int x = 0; x < 9; x++)
            for (int y = 0; y < 9; y++)
                yield return this[x, y];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
