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
            CreateShade();
        }

        private void CreateShade()
        {
            //TextBlock textBlockShade = new TextBlock();
            //textBlockShade = textBlock1;
            //Thickness m = textBlockShade.Margin;
            //m.Top += 5;
            //m.Left += 5;
            //textBlockShade.Margin = m;
            //textBlockShade.Foreground = Brushes.Black;
            //Canvas.SetZIndex(textBlockShade, (int)1);
        }


        #region Methods
        private void InitColors()
        {
            colors = new List<Color>();
            colors.Add(Colors.Blue);
            colors.Add(Colors.Brown);
            colors.Add(Colors.Red);
            colors.Add(Colors.Purple);
            colors.Add(Colors.Gray);
            colors.Add(Colors.Yellow);
            colors.Add(Colors.Black);
            colors.Add(Colors.Green);
        }

        private void InitNames()
        {
            colorNames = new List<string>();
            colorNames.Add("Blue");
            colorNames.Add("Brown");
            colorNames.Add("Red");
            colorNames.Add("Purple");
            colorNames.Add("Gray");
            colorNames.Add("Yellow");
            colorNames.Add("Black");
            colorNames.Add("Green");
        }
        #endregion

        private void textBlock1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            NextColor();
            CreateShade();
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
            } while (j == indexC && j == index);
            indexC = j;

            textBlock1.Foreground = new SolidColorBrush(colors[indexC]);

        }
    }
}
