using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuUI
{
    internal static class IOHelper
    {
        internal static void WriteToFile(this VisualGame game, string fileName)
        {
            StringBuilder sb = new();
            for (int i = 0; i < 9; i++)
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
    }
}
