using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TestControlCenter.Infrastructure;
using TestControlCenter.Models;
using TestControlCenter.Properties;
using TestControlCenter.Services;
using TestControlCenter.Tools;

namespace TestControlCenter.Windows
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public bool IsLoggedIn { get; set; } = false;

        private string Username { get; set; }

        private string Password { get; set; }

        public LoginWindow()
        {
            InitializeComponent();

            //if (!string.IsNullOrEmpty(Settings.Default.Username))
            //{
            //    Username = StringProtector.Unprotect(Settings.Default.Username);
            //}

            //if (!string.IsNullOrEmpty(Settings.Default.Password))
            //{
            //    Password = StringProtector.Unprotect(Settings.Default.Password);
            //    SaveDataCheckBox.IsChecked = true;
            //}

            //UsernameTextBox.Text = Username;
            //PasswordTextBox.Password = Password;

            UsernameStudentTextBox.Focus();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            await OperatorLogin();
        }

        private async Task OperatorLogin()
        {
            loginButton.IsEnabled = false;

            var temp = loginButton.Content;

            loginButton.Content = "لطفا منتظر باشید...";

            await Login();

            loginButton.Content = temp;

            loginButton.IsEnabled = true;
        }

        private async Task Login()
        {
            //if (SaveDataCheckBox.IsChecked == true)
            //{
            //    var username = StringProtector.Protect(UsernameTextBox.Text);
            //    var password = StringProtector.Protect(PasswordTextBox.Password);

            //    Settings.Default.Username = username;
            //    Settings.Default.Password = password;
            //}
            //else
            //{
            //    Settings.Default.Username = string.Empty;
            //    Settings.Default.Password = string.Empty;
            //}

            //Settings.Default.Save();
            //Settings.Default.Reload();

            var result = await CommunicationService.Login(UsernameTextBox.Text, PasswordTextBox.Password);

            if (result.Type != LoginResultType.Success)
            {
                if (result.Type == LoginResultType.CommunicationProblem)
                {
                    if(result.Type == LoginResultType.Failed)
                    {
                        NotificationsHelper.Information("نام کاربری یا رمز عبور اشتباه است.", "خطا");
                    }
                    else
                    {
                        NotificationsHelper.Error("در برقراری ارتباط با سرور خطایی به وجود آمد.", "خطای ارتباطی");
                    }
                }
                else
                {
                    NotificationsHelper.Information("نام کاربری یا رمز عبور اشتباه است.", "خطا");
                }

                return;
            }

            if (Authorize(result.Response) == null)
            {
                NotificationsHelper.Error("در برقراری ارتباط با سرور خطایی به وجود آمد.", "خطای ارتباطی");
                return;
            }

            StaticValues.LoggedInUserType = UserType.Operator;

            Close();
        }

        private AuthenticationItem Authorize(string response)
        {
            try
            {
                var data = JsonConvert.DeserializeObject<AuthenticationItem>(response);

                ServerClient.AuthenticationData = data;

                IsLoggedIn = true;

                return data;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void ForgetPasswordLink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private async void StudentLoginButton_Click(object sender, RoutedEventArgs e)
        {
            await StudentLoginExecute();
        }

        private async Task StudentLoginExecute()
        {
            loginStudentButton.IsEnabled = false;

            var temp = loginStudentButton.Content;

            loginStudentButton.Content = "لطفا منتظر باشید...";

            await StudentLogin();

            loginStudentButton.Content = temp;

            loginStudentButton.IsEnabled = true;
        }

        private async Task StudentLogin()
        {
            var result = await CommunicationService.StudentLogin(UsernameStudentTextBox.Text, PasswordStudentTextBox.Password);

            if (result.Type != LoginResultType.Success)
            {
                if (result.Type == LoginResultType.CommunicationProblem)
                {
                    if (result.Type == LoginResultType.Failed)
                    {
                        NotificationsHelper.Information("نام کاربری یا توکن اشتباه است.", "خطا");
                    }
                    else
                    {
                        NotificationsHelper.Error("در برقراری ارتباط با سرور خطایی به وجود آمد.", "خطای ارتباطی");
                    }
                }
                else
                {
                    NotificationsHelper.Information("نام کاربری یا توکن اشتباه است.", "خطا");
                }

                return;
            }

            if (Authorize(result.Response) == null)
            {
                NotificationsHelper.Error("در برقراری ارتباط با سرور خطایی به وجود آمد.", "خطای ارتباطی");
                return;
            }

            StaticValues.LoggedInUserType = UserType.Student;

            Close();
        }

        private async void StudentText_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == System.Windows.Input.Key.Enter)
            {
                await StudentLoginExecute();
            }
        }

        private async void OperatorText_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                await OperatorLogin();
            }
        }
    }
}
