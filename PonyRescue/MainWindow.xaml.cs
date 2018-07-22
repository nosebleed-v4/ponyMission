using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
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
        private string mazeId = "875c1ba0-5a00-4210-a6e7-cb932e54d821";
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string url = @"https://ponychallenge.trustpilot.com/pony-challenge/maze/" + mazeId;

            string responseBody = ExecuteGetRequest(url);

            MazeState mazeState = JsonConvert.DeserializeObject<MazeState>(responseBody);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string url = @"https://ponychallenge.trustpilot.com/pony-challenge/maze/" + mazeId + @"/print";

            string responseBody = ExecuteGetRequest(url);

            this.MazeSnapshot.Text = String.Format(CultureInfo.InvariantCulture, responseBody);
        }

        private static string ExecuteGetRequest(string url)
        {
            string responseBody;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                responseBody = reader.ReadToEnd();
            }

            return responseBody;
        }
    }
}
