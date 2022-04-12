using SudokuSolver.DataType;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SudokuUI
{
    /// <summary>
    /// Helper class to manipulate visual elements.
    /// </summary>
    internal static class VisualHelper
    {
        public static IEnumerable<T> FindVisualChilds<T>(this DependencyObject depObj) where T : DependencyObject
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

        // write to file
        public static void WriteToFile(this VisualGame game, string fileName)
        {
            StringBuilder sb = new StringBuilder();
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

        // read from file
        public static int?[,] ReadFromFile(string fileName)
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
    }
}
