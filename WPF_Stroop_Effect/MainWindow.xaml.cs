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

using Microsoft.Kinect;
using System.Threading;
using Microsoft.Speech.Recognition;
using System.IO;
using Microsoft.Speech.AudioFormat;
using System.Diagnostics;
using System.ComponentModel;

namespace WPF_Stroop_Effect
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        #region Variables
        private List<string> colorNames;
        private List<Color> colors;
        private int index = 0, indexC = 0, score = 0;

        KinectSensor _kinectSensor;
        SpeechRecognitionEngine _sre;
        KinectAudioSource _source;

        #endregion

        #region Constructor
        public MainWindow()
        {
            InitializeComponent();
            textBlockScore.Text = score.ToString();

            InitColors();
            InitNames();
            NextColor();
            CreateShade();
            this.DataContext = this;
            this.Unloaded += delegate
            {
                _kinectSensor.SkeletonStream.Disable();
                _sre.RecognizeAsyncCancel();
                _sre.RecognizeAsyncStop();
                //_source.Dispose();
                _sre.Dispose();
            };
            this.Loaded += delegate
            {
                _kinectSensor = KinectSensor.KinectSensors[0];
                _kinectSensor.SkeletonStream.Enable(new TransformSmoothParameters()
                {
                    Correction = 0.5f
                    ,
                    JitterRadius = 0.05f
                    ,
                    MaxDeviationRadius = 0.04f
                    ,
                    Smoothing = 0.5f
                });
                _kinectSensor.Start();
                StartSpeechRecognition();
            };
        }
        #endregion

        #region Methods
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
            textBlockScore.Text = score.ToString();

        }


        private void WordGuessCorrect()
        {
            score++;
            NextColor();
            CreateShade();
        }


        #endregion

        #region KinectMethods
        private KinectAudioSource CreateAudioSource()
        {
            var source = KinectSensor.KinectSensors[0].AudioSource;
            source.AutomaticGainControlEnabled = false;
            source.EchoCancellationMode = EchoCancellationMode.None;
            return source;
        }

        private void StartSpeechRecognition()
        {
            _source = CreateAudioSource();

            Func<RecognizerInfo, bool> matchingFunc = r =>
            {
                string value;
                r.AdditionalInfo.TryGetValue("Kinect", out value);
                return "True".Equals(value, StringComparison.InvariantCultureIgnoreCase)
                    && "en-US".Equals(r.Culture.Name, StringComparison.InvariantCultureIgnoreCase);
            };
            RecognizerInfo ri = SpeechRecognitionEngine.InstalledRecognizers().Where(matchingFunc).FirstOrDefault();

            _sre = new SpeechRecognitionEngine(ri.Id);
            CreateGrammars(ri);
            _sre.SpeechRecognized += sre_SpeechRecognized;
            _sre.SpeechHypothesized += sre_SpeechHypothesized;
            _sre.SpeechRecognitionRejected += sre_SpeechRecognitionRejected;

            Stream s = _source.Start();
            _sre.SetInputToAudioStream(s,
                                        new SpeechAudioFormatInfo(
                                            EncodingFormat.Pcm, 16000, 16, 1,
                                            32000, 2, null));
            _sre.RecognizeAsync(RecognizeMode.Multiple);
        }

        private void CreateGrammars(RecognizerInfo ri)
        {
            var commands = new Choices();
            foreach (String colorName in colorNames)
            {
                commands.Add(colorName);
            }            

            var gb = new GrammarBuilder();
            gb.Culture = ri.Culture;
            gb.Append(commands);

            var g = new Grammar(gb);
            _sre.LoadGrammar(g);

            var q = new GrammarBuilder();
            q.Append("quit");
            var quit = new Grammar(q);

            _sre.LoadGrammar(quit);
        }

        void sre_SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            HypothesizedText += " Rejected";
            Confidence = Math.Round(e.Result.Confidence, 2).ToString();
        }

        void sre_SpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {
            HypothesizedText = e.Result.Text;
        }

        void sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action<SpeechRecognizedEventArgs>(InterpretCommand), e);
        }

        private void InterpretCommand(SpeechRecognizedEventArgs e)
        {
            var result = e.Result;
            Confidence = Math.Round(result.Confidence, 2).ToString();
            //if (result.Confidence < 95 && result.Words[0].Text == "quit")
            //{
            //    this.Close();
            //}


            if (result.Words[0].Text == colorNames[indexC])
            {
                WordGuessCorrect();   
            }


        }


        private string _confidence;
        public string Confidence
        {
            get { return _confidence; }
            set
            {
                _confidence = value;
                OnPropertyChanged("Confidence");
            }
        }

        private string _hypothesizedText;
        public string HypothesizedText
        {
            get { return _hypothesizedText; }
            set
            {
                _hypothesizedText = value;
                OnPropertyChanged("HypothesizedText");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }

        }
        #endregion
    }
}
