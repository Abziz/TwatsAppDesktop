using Flurl.Http;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using TwatsAppClient.extensions;

namespace TwatsAppClient
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private bool isLogin = true;
        public LoginWindow()
        {
            InitializeComponent();
        }


        private async Task Login()
        {
            try
            {
                var response = await API.Account.Login.PostJsonAsync(new { username = Username.Text, password = Password.Password, grant_type = "password" });
            }
            catch (FlurlHttpTimeoutException)
            {
                //timeout
            }
            catch (FlurlHttpException ex)
            {
                if (ex.Call.Response != null)
                {
                    switch (ex.Call.HttpStatus)
                    {
                        case HttpStatusCode.NotFound:
                            break;
                        case HttpStatusCode.BadRequest:
                            break;
                        case HttpStatusCode.InternalServerError:
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    //wtf happened?
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task Register()
        {
            try
            {
                var response = await API.Account.Register.PostJsonAsync(new
                {
                    username = Username.Text,
                    password = Password.Password,
                    ConfirmPassword = ConfirmPassword.Password,
                    firstName = FirstName.Text,
                    lastName = LastName.Text
                });
            }
            catch (FlurlHttpTimeoutException)
            {
                //timeout
            }
            catch (FlurlHttpException ex)
            {
                if (ex.Call.Response != null)
                {
                    switch (ex.Call.HttpStatus)
                    {
                        case HttpStatusCode.NotFound:
                            break;
                        case HttpStatusCode.BadRequest:
                            break;
                        case HttpStatusCode.InternalServerError:
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    //problem with server
                }
            }
        }

        private void SwitchLogin(object sender, RoutedEventArgs e)
        {
            if (isLogin)
            {
                RegisterFields.Visibility = Visibility.Visible;
                LoginBtn.Content = "Register";
                LoginSwitch.Content = "I allready have an account";
                isLogin = false;
            }
            else
            {
                RegisterFields.Visibility = Visibility.Collapsed;
                LoginBtn.Content = "Login";
                LoginSwitch.Content = "I don't have an account";
                isLogin = true;
            }
        }

        private async void LoginBtn_ClickAsync(object sender, RoutedEventArgs e)
        {
            try
            {
                Spinner.Work();

                if (!isLogin)
                {
                    await Register();
                }
                await Login();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                Spinner.Stop();
            }
        }
    }
}
