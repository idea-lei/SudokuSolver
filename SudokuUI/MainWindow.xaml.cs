using Microsoft.Win32;
using SudokuSolver.DataType;
using SudokuSolver.Solver;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SudokuUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private VisualGame VisualGame = new();
        private List<Game> Answers = new();
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

        private void InitEvents()
        {
            foreach (var unit in VisualGame)
            {
                if (unit.Unit == null) return;
                unit.Unit.OnCurrentValueChanged += unit.UpdateUnitView;
                unit.Unit.OnPossibleValuesChanged += unit.UpdateUnitView;
            }
        }

        private void InitGame()
        {
            int?[,] mat = new int?[9, 9];
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    if (int.TryParse(VisualGame[i, j].TextBox.Text, out int temp))
                    {
                        if (temp == 0)
                            VisualGame[i, j].TextBox.Text = null;
                        mat[i, j] = temp != 0 ? temp : null;
                    }

            VisualGame.Game.InitBoard(mat);
            InitEvents();
            VisualGame.Game.InitPossibleValues();
        }

        private IEnumerator? Solver;

        private void Btn_Start_Click(object sender, RoutedEventArgs e)
        {
            InitGame();
            Btn_Start.IsEnabled = false;
            Btn_Solve.IsEnabled = true;
            Btn_Reset.IsEnabled = true;
            Btn_Next.IsEnabled = true;
            Solver = VisualGame.Game.Solve(Answers);
        }

        private void Btn_Reset_Click(object sender, RoutedEventArgs e) => Reset();

        private void Btn_Clear_Click(object sender, RoutedEventArgs e)
        {
            foreach (var unit in VisualGame)
                if (unit.Unit != null)
                    unit.Unit.Given = null;
            Reset();
        }

        private void Reset()
        {
            Answers.Clear();
            foreach (var unit in VisualGame)
                unit.Reset();

            Tb_AnswerAmount.Visibility = Visibility.Hidden;
            Tb_AnswerAmount.Text = null;
            currentAnswerIndex = 0;
            Btn_Start.IsEnabled = true;
            Btn_Solve.IsEnabled = false;
            Btn_Reset.IsEnabled = false;
            Btn_Next.IsEnabled = false;
            Btn_Forward.IsEnabled = false;
            Btn_Previous.IsEnabled = false;
            Solver = null;
        }


        private void Btn_Next_Click(object sender, RoutedEventArgs e)
        {
            if (Solver?.MoveNext() != true)
            {
                Btn_Next.IsEnabled = false;
                Btn_Solve.IsEnabled = false;
            }
        }

        private void Btn_Solve_Click(object sender, RoutedEventArgs e)
        {
            Cursor = System.Windows.Input.Cursors.Wait;
            while (Solver?.MoveNext() == true) { }
            Btn_Next.IsEnabled = false;
            Btn_Solve.IsEnabled = false;

            Cursor = System.Windows.Input.Cursors.Arrow;

            if (Answers.Count == 0)
            {
                MessageBox.Show("No solution found!");
                return;
            }

            if (Answers.Count == 1)
            {
                ShowAnswer(Answers[0]);
                foreach (var unit in VisualGame)
                {
                    int? oA = unit.Unit.OptionalAnswer;
                    if (unit.Unit.Answer == null)
                    {
                        unit.Unit.OptionalAnswer = null;
                        unit.Unit.Answer = oA;
                    }
                }
            }

            if (Answers.Count > 1)
            {
                Tb_AnswerAmount.Visibility = Visibility.Visible;
                Btn_Forward.IsEnabled = true;
                Btn_Previous.IsEnabled = true;
            }
        }

        private void Btn_Read_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                var mat = IOHelper.ReadFromFile(openFileDialog.FileName);
                for (int i = 0; i < 9; i++)
                    for (int j = 0; j < 9; j++)
                        VisualGame[i, j].TextBox.Text = mat[i, j]?.ToString();
            }
        }

        private void Btn_Write_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
                VisualGame.WriteToFile(saveFileDialog.FileName);
        }

        int currentAnswerIndex;
        private void Btn_Previous_Click(object sender, RoutedEventArgs e)
        {
            currentAnswerIndex -= 1;
            if (currentAnswerIndex < 0) currentAnswerIndex = Answers.Count - 1;
            Tb_AnswerAmount.Text = $"{currentAnswerIndex} of {Answers.Count}";
            ShowAnswer(Answers[currentAnswerIndex]);
        }

        private void Btn_Forward_Click(object sender, RoutedEventArgs e)
        {
            currentAnswerIndex += 1;
            if (currentAnswerIndex >= Answers.Count) currentAnswerIndex = 0;
            Tb_AnswerAmount.Text = $"{currentAnswerIndex} of {Answers.Count}";
            ShowAnswer(Answers[currentAnswerIndex]);
        }

        private void ShowAnswer(Game game)
        {
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                {
                    var unit = VisualGame[i, j].Unit;
                    if (unit == null) return;
                    unit.Assumption = null;
                    unit.OptionalAnswer = game[i, j]?.CurrentValue;
                }
        }
    }
}
