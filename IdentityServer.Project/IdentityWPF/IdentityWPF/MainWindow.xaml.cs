using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
using IdentityModel.Client;

namespace IdentityWPF
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var userName = txtUsername.Text;
            var password = txtPassword.Password;

            //取得發現檔
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5000/");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }

            var tokenRespose = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "wpf.client",
                ClientSecret = "wpf.client",
                Scope = "scope1",

                UserName = userName,
                Password = password
            });
            if (tokenRespose.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }

            //call api
            var apiClient = new HttpClient();
            apiClient.SetBearerToken(tokenRespose.AccessToken);
            var response = await apiClient.GetAsync("http://localhost:5001/WeatherForecast");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }
            else
            {
                var content = response.Content.ReadAsStringAsync();
                //Console.WriteLine(JArray.Parse)
            }


        }
    }
}
