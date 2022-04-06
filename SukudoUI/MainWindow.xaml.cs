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

namespace SukudoUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Grid[,] Grids = new Grid[3,3];
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
                    for (int x = 0;x<3; x++)
                        for (int y = 0; y < 3; y++)
                        {
                            TextBox t = new();
                            Grid.SetColumn(t, x);
                            Grid.SetRow(t, y);
                            g.Children.Add(t);
                        }
                    Grid.SetColumn(g, i * 2);
                    Grid.SetRow(g, j * 2);
                    G_Game.Children.Add(g);
                    Grids[i,j] = g;
                }
        }
    }
}
