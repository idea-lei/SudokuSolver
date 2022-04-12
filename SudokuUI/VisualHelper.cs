using SudokuSolver.DataType;
using SudokuSolver.Solver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace SudokuUI
{
    /// <summary>
    /// Helper class to manipulate visual elements.
    /// </summary>
    internal static class VisualHelper
    {
        internal static IEnumerable<T> FindVisualChilds<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) yield return (T)Enumerable.Empty<T>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject ithChild = VisualTreeHelper.GetChild(depObj, i);
                if (ithChild == null) continue;
                if (ithChild is T t) yield return t;
                foreach (T childOfChild in FindVisualChilds<T>(ithChild)) yield return childOfChild;
            }
        }

        internal static void WriteToFile(this VisualGame game, string fileName)
        {
            StringBuilder sb = new();
            for(int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    string s = game[i, j].TextBox.Text;
                    s = string.IsNullOrEmpty(s) ? "0" : s;
                    sb.Append(s ?? "0");
                }
                sb.AppendLine();
            }
            File.WriteAllText(fileName, sb.ToString());
        }

        internal static int?[,] ReadFromFile(string fileName)
        {
            string[] content = File.ReadAllLines(fileName);
            if (content.Length != 9)
                throw new Exception("Invalid file format");
            int?[,] map = new int?[9, 9];
            for (int i = 0; i < 9; i++)
            {
                string row = content[i];
                if (row.Length != 9)
                    throw new Exception("Invalid file format");
                for (int j = 0; j < 9; j++)
                    if (int.TryParse(row[j].ToString(), out int value))
                        map[i, j] = value == 0 ? null : value;
            }
            return map;
        }

        /// <summary>
        /// if the current value of the unit is null, show the possible values, otherwise show the current value
        /// </summary>
        internal static void UpdateUnitView(this VisualUnit unit)
        {
            if (unit.Unit == null) return;
            if (unit.Unit.CurrentValue == null)
            {
                unit.TextBlock.Text = null;
                foreach (var v in unit.Unit.GetPossibleValues())
                    unit.TextBlock.Text += v.ToString();
                unit.TextBlock.Visibility = Visibility.Visible;
                unit.TextBox.Visibility = Visibility.Hidden;
            }
            else
            {
                unit.TextBox.Text = unit.Unit.CurrentValue.ToString();
                switch (unit.Unit.UnitValueType)
                {
                    case UnitValueType.Given:
                        unit.TextBox.BorderBrush = Brushes.Black;
                        unit.TextBox.Foreground = Brushes.Black;
                        break;
                    case UnitValueType.Answer:
                        unit.TextBox.BorderBrush = Brushes.Blue;
                        unit.TextBox.Foreground = Brushes.Blue;
                        break;
                    case UnitValueType.Assumption:
                        unit.TextBox.BorderBrush = Brushes.Orange;
                        unit.TextBox.Foreground = Brushes.Orange;
                        break;
                }

                unit.TextBlock.Visibility = Visibility.Hidden;
                unit.TextBox.Visibility = Visibility.Visible;
            }
        }

        internal static void InitPossibleValues(this VisualGame VisualGame)
        {
            foreach (var unit in VisualGame)
            {
                unit.Unit?.InitPossibleValues();
                if (unit.Unit?.HasConflict() == true)
                {
                    MessageBox.Show($"No possible values for unit ({unit.Unit.Coordinate.Item1}, {unit.Unit.Coordinate.Item2})!");
                    break;
                }
            }
        }
    }
}
