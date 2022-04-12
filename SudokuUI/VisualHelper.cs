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

        /// <summary>
        /// if the current value of the unit is null, show the possible values, otherwise show the current value
        /// </summary>
        internal static void UpdateUnitView(this VisualUnit unit)
        {
            if (unit.Unit == null) return;
            if (unit.Unit.CurrentValue == null && !unit.Unit.HasConflict())
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
                
                unit.TextBlock.Visibility = Visibility.Hidden;
                unit.TextBox.Visibility = Visibility.Visible;
            }
            
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
                    unit.TextBox.BorderBrush = Brushes.Green;
                    unit.TextBox.Foreground = Brushes.Green;
                    break;
                case UnitValueType.Conflict:
                    unit.TextBox.BorderBrush = Brushes.Red;
                    unit.TextBox.Foreground = Brushes.Red;
                    break;
            }
        }
    }
}
