using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;

namespace PonyRescue
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string mazeId = "656ecc24-a1ef-45f7-8387-b2766bf6c3ce";
        private IGameController gameController = null;


        public MainWindow()
        {
            InitializeComponent();

            this.PonyName.ItemsSource = new List<string>() { "Fluttershy", "Rainbow Dash", "Twilight Sparkle", "Pinkie Pie" };
            gameController = new GameController(new PonyChallengeFacade(), new MazeMapFactory(), new PonyNavigator());
            gameController.MoveMadeEvent += MoveMade;
        }

        private async void StartNewGame(object sender, RoutedEventArgs e)
        {
            var snapshot = await gameController.StartNewGame(Int32.Parse(this.WidthTextbox.Text), Int32.Parse(this.HeightTextbox.Text), this.PonyName.Text, Int32.Parse(this.DifficultyTextBox.Text));

            this.MazeSnapshot.Text = snapshot;
        }

        private async void RunMoveSequence(object sender, RoutedEventArgs e)
        {
            var result = await gameController.RescuePony(Int32.Parse(this.MoveDelay.Text));

            MessageBox.Show(result);
        }

        void MoveMade(object sender, EventArgs e)
        {
            if (Thread.CurrentThread == Application.Current.Dispatcher.Thread)
                this.MazeSnapshot.Text = (e as GameEventArgs)?.snapshot ?? "";
            else
            {
                this.MazeSnapshot.Dispatcher.Invoke(() => MoveMade(sender, e));
            }
            
        }
    }

    internal class GameEventArgs :EventArgs
    {
        public string snapshot;
    }
}
