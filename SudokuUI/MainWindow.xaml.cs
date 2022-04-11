using SudokuSolver.DataType;
using SudokuSolver.Solver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SudokuUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private VisualGame VisualGame = new();
        public MainWindow()
        {
            InitializeComponent();
            InitUI();
        }

        private void InitUI()
        {
            // because of the splitter, using i += 2
            // these two for-loops are for the G_Game
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    Grid g = new Grid();
                    for (int a = 0; a < 3; a++)
                    {
                        g.ColumnDefinitions.Add(new ColumnDefinition());
                        g.RowDefinitions.Add(new RowDefinition());
                    }

                    // these two for-loops are for the 'g' above
                    for (int x = 0; x < 3; x++)
                        for (int y = 0; y < 3; y++)
                        {
                            // for current value
                            TextBox tbox = new();
                            Grid.SetColumn(tbox, y);
                            Grid.SetRow(tbox, x);
                            g.Children.Add(tbox);
                            VisualGame.TextBox[i * 3 + x, j * 3 + y] = tbox;

                            // for assumption
                            TextBlock tblock = new()
                            {
                                Visibility = Visibility.Hidden,
                                VerticalAlignment = VerticalAlignment.Center,
                                HorizontalAlignment = HorizontalAlignment.Center
                            };
                            Grid.SetColumn(tblock, y);
                            Grid.SetRow(tblock, x);
                            g.Children.Add(tblock);

                            VisualGame.TextBlock[i * 3 + x, j * 3 + y] = tblock;
                        }
                    Grid.SetColumn(g, j * 2);
                    Grid.SetRow(g, i * 2);
                    G_Game.Children.Add(g);
                }
        }

        private void InitGame()
        {
            int?[,] mat = new int?[9, 9];
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    if (int.TryParse(VisualGame[i, j].TextBox.Text, out int temp))
                        mat[i, j] = temp;

            VisualGame.Game.InitBoard(mat);
            foreach (var unit in VisualGame)
            {
                if (unit.Unit == null) return;
                unit.Unit.OnCurrentValueChanged += () =>
                {
                    UpdateUnitView(unit);
                    unit.Unit.UpdatePossiableValuesForRelevantUnits();
                };
                unit.Unit.OnPossibleValuesChanged += () =>
                {
                    if (unit.Unit.GetPossibleValues().Length > 1)
                        UpdateUnitView(unit);
                    else
                        unit.Unit.UpdateAnswer_OnlyOnePossibleValue();
                };
            }
        }

        /// <summary>
        /// if the current value of the unit is null, show the possible values, otherwise show the current value
        /// </summary>
        private void UpdateUnitView(VisualUnit unit)
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

        // for testing
        private string MatrixToString(int?[,] matrix)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    sb.Append(matrix[i, j] == null ? "0" : matrix[i, j].ToString());
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        private void InitPossibleValues()
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



        private void Solve()
        {
            InitPossibleValues();
            bool updated = true;
            while (updated)
            {
                updated = false;
                updated |= VisualGame.Game.UpdateUnits_OnlyOnePossibleValue();

                for (int i = 0; i < 9; i++)
                {
                    updated |= VisualGame.Game.UpdateAnswer_OnlyOnePossibleValueInRow(i);
                    //updated |= VisualGame.Game.UpdateAnswer_OnlyOnePossibleValueInColumn(i);
                    //updated |= VisualGame.Game.UpdateAnswer_OnlyOnePossibleValueInBlock(i / 3, i % 3);
                }
            }
        }

        private void Btn_Start_Click(object sender, RoutedEventArgs e)
        {
            InitGame();
            Btn_Start.IsEnabled = false;
            Btn_Reset.IsEnabled = true;
            Solve();
        }

        private void Btn_Reset_Click(object sender, RoutedEventArgs e)
        {
            foreach (var unit in VisualGame)
                unit.Reset();

            Btn_Start.IsEnabled = true;
            Btn_Reset.IsEnabled = false;
        }

        private void Btn_Clear_Click(object sender, RoutedEventArgs e)
        {
            foreach(var unit in VisualGame)
            {
                unit.Unit.Given = null;
                unit.TextBox.BorderBrush = Brushes.Black;
                unit.TextBox.Foreground = Brushes.Black;
                unit.Reset();
            }
            Btn_Start.IsEnabled = true;
            Btn_Reset.IsEnabled = false;
        }
    }
}
