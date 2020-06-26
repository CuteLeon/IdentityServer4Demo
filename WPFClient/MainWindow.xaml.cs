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
using Client.Utiliy;
using Newtonsoft.Json.Linq;

namespace WPFClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public TokenResponse Token { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void GetTokenButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var userName = this.UserNameInput.Text;
                var password = this.PasswordInput.Password;
                var client = new HttpClient();

                // Request discory document
                PrintOutput(this.TokenOutput, "Get dicovery document of Identity provider site ...");
                var discovery = await client.GetDiscoveryDocumentAsync("https://localhost:5000/");
                if (discovery.IsError)
                {
                    PrintOutput(this.TokenOutput, $"Failed: {discovery.Error}");
                    return;
                }
                PrintOutput(this.TokenOutput, $"Get discovery document successfully:\n\t{discovery.Issuer}\n\t{discovery.TokenEndpoint}");

                // Request client token, using TokenEndpoint、ClientID+ClientSecret(writed in Identity Provider Config)、UserName+Password(writed in Identity Provider TestUsers)
                PrintOutput(this.TokenOutput, $"Get User token ...");
                this.Token = await client.RequestPasswordTokenAsync(new PasswordTokenRequest()
                {
                    Address = discovery.TokenEndpoint,
                    ClientId = "WPFClient",
                    ClientSecret = "84C0358B-B7A8-427A-933E-9F8FA080F3C5",
                    Scope = "api1",
                    UserName = userName,
                    Password = password,
                });
                if (this.Token.IsError)
                {
                    PrintOutput(this.TokenOutput, $"Failed: {this.Token.ErrorDescription}");
                    return;
                }
                PrintOutput(this.TokenOutput, $"Get User token successfully:\n\t{this.Token.AccessToken.Left()}");
            }
            catch (Exception ex)
            {
                PrintOutput(this.TokenOutput, $"Failed: {ex.Message}");
            }
        }

        private async void GetWebAPIButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Call api
                PrintOutput(this.WebAPIOutput, $"Get Web API ...");
                var apiClient = new HttpClient();
                apiClient.SetBearerToken(this.Token.AccessToken);
                var apiResponse = await apiClient.GetAsync("https://localhost:5001/WeatherForecast");
                if (!apiResponse.IsSuccessStatusCode)
                {
                    PrintOutput(this.WebAPIOutput, $"Failed: {apiResponse.StatusCode}");
                    return;
                }

                var content = await apiResponse.Content.ReadAsStringAsync();
                PrintOutput(this.WebAPIOutput, $"Get Web API successfully: {JArray.Parse(content)}");
                var userClaims = apiResponse.Headers.GetValues("UserClaims") as IEnumerable<string>;
                PrintOutput(this.WebAPIOutput, $"User Claims:\n\t{string.Join("\n\t", userClaims)}");
            }
            catch (Exception ex)
            {
                PrintOutput(this.WebAPIOutput, $"Failed: {ex.Message}");
            }
        }

        private void PrintOutput(TextBox output, string message)
        {
            output.AppendText($"{message}\n");
            output.ScrollToEnd();
        }
    }
}
