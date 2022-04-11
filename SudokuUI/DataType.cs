using SudokuSolver.DataType;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SudokuUI
{
    internal class VisualGame : IEnumerable<VisualUnit>
    {
        public Game Game { get; set; } = new();
        public TextBox[,] TextBox { get; } = new TextBox[9, 9];
        public TextBlock[,] TextBlock { get; } = new TextBlock[9, 9];

        public VisualUnit this[int x, int y]
        {
            get => new()
            {
                Unit = Game?.GameBoard?[x, y],
                TextBox = TextBox[x, y],
                TextBlock = TextBlock[x, y]
            };
        }

        public VisualUnit[] GetRow(int row)
        {
            if (row < 0 || row > 8)
                throw new ArgumentOutOfRangeException(nameof(row));

            var rowUnits = new VisualUnit[9];
            for (int i = 0; i < 9; i++)
                rowUnits[i] = this[row, i];
            return rowUnits;
        }

        public VisualUnit[] GetColumn(int column)
        {
            if (column < 0 || column > 8)
                throw new ArgumentOutOfRangeException(nameof(column));

            var columnUnits = new VisualUnit[9];
            for (int i = 0; i < 9; i++)
                columnUnits[i] = this[i, column];
            return columnUnits;
        }

        public VisualUnit[,] GetBlock(int row, int column)
        {
            if (row < 0 || row > 3)
                throw new ArgumentOutOfRangeException(nameof(row));
            if (column < 0 || column > 3)
                throw new ArgumentOutOfRangeException(nameof(column));

            var blockUnits = new VisualUnit[3, 3];
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    blockUnits[i, j] = this[row + i, column + j];
            return blockUnits;
        }

        public IEnumerator<VisualUnit> GetEnumerator()
        {
            for (int x = 0; x < 9; x++)
                for (int y = 0; y < 9; y++)
                    yield return this[x, y];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    internal class VisualUnit
    {
        public Unit? Unit { get; set; }
        public TextBox TextBox { get; set; }
        public TextBlock TextBlock { get; set; }

        public void Reset()
        {
            if (Unit != null) Unit.Reset();
            TextBox.Text = Unit.CurrentValue.ToString();
            TextBox.BorderBrush = Brushes.Black;
            TextBlock.Visibility = Visibility.Hidden;
            TextBox.Visibility = Visibility.Visible;
        }
    }
}
