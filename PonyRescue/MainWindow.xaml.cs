using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
using Newtonsoft.Json;

namespace PonyRescue
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string mazeId = "28a37caf-bb56-4f5e-b26a-b419ab2828a7";
        private static readonly HttpClient client = new HttpClient();
        private List<string> shortestPath = new List<string>();

        public MainWindow()
        {
            InitializeComponent();

            this.direction.ItemsSource = new List<string>() { "north", "south", "east", "west" };
            client.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header
        }

        private async void GetMazeState(object sender, RoutedEventArgs e)
        {
            string url = @"https://ponychallenge.trustpilot.com/pony-challenge/maze/" + mazeId;

            string responseBody = await client.GetStringAsync(url);

            MazeState mazeState = JsonConvert.DeserializeObject<MazeState>(responseBody);

            Pathfinder pathfinder = new Pathfinder(mazeState.width, mazeState.height, mazeState.PonyLocation, mazeState.ExitLocation);
            pathfinder.InitializeChambers(mazeState.data);
            this.shortestPath = pathfinder.FindShortestPath();
        }

        private async void PrintMazeSnapshot(object sender, RoutedEventArgs e)
        {
            await PrintMazeSnapshot();
        }

        private async Task PrintMazeSnapshot()
        {
            string url = @"https://ponychallenge.trustpilot.com/pony-challenge/maze/" + mazeId + @"/print";
            string responseBody = await ExecuteGetRequest(url);

            this.MazeSnapshot.Text = String.Format(CultureInfo.InvariantCulture, responseBody);
        }

        private static async Task<string> ExecuteGetRequest(string url)
        {
            try
            {
                return await client.GetStringAsync(url);
            }
            catch (Exception e)
            {
                //implement exception handling here
                Console.WriteLine(e);
                throw;
            }
            
        }

        private async void MovePony(object sender, RoutedEventArgs e)
        {
            string url = @"https://ponychallenge.trustpilot.com/pony-challenge/maze/" + mazeId;

            //move pony
            string direction = this.direction.Text;
            if (direction != string.Empty)
            {
                StringContent content = new StringContent("{\"direction\":\"" + direction + "\"}", Encoding.UTF8, "application/json");
                await ExecutePostRequest(url, content);
            }
        }

        private async Task<string> ExecutePostRequest(string url, StringContent content)
        {
            string responseString = "";
            try
            {
                var response = await client.PostAsync(url, content);
                responseString = await response.Content.ReadAsStringAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return responseString;
        }

        private async void RunMoveSequence(object sender, RoutedEventArgs e)
        {
            string url = @"https://ponychallenge.trustpilot.com/pony-challenge/maze/" + mazeId;

            foreach (var direction in this.shortestPath) 
            {
                //TODO-add ruaway feature
                StringContent content = new StringContent("{\"direction\":\"" + direction + "\"}", Encoding.UTF8, "application/json");
                await ExecutePostRequest(url, content);

                await Task.Delay(1000);
                await PrintMazeSnapshot();
            }
        }
    }
}
