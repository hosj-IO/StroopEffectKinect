using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPF_Stroop_Effect
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<string> colorNames;
        private List<Color> colors;
        private int index = 0, indexC = 0;

        public MainWindow()
        {
            InitializeComponent();
            InitColors();
            InitNames();
            NextColor();
        }


        #region Methods
        private void InitColors()
        {
            colors = new List<Color>();
            colors.Add(Colors.Red);
            colors.Add(Colors.Blue);
            colors.Add(Colors.Green);
        }

        private void InitNames()
        {
            colorNames = new List<string>();
            colorNames.Add("Red");
            colorNames.Add("Blue");
            colorNames.Add("Green");
        }
        #endregion

        private void textBlock1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            NextColor();
        }

        private void NextColor()
        {
            Random rdm = new Random();
            int i;
            do
            {
                i = rdm.Next(0, colorNames.Count);
            } while (i == index);

            index = i;

            textBlock1.Text = colorNames[index];

            int j;
            do
            {
                j = rdm.Next(0, colors.Count);
            } while (j == indexC);
            indexC = j;

            textBlock1.Foreground = new SolidColorBrush(colors[indexC]);

        }
    }
}
