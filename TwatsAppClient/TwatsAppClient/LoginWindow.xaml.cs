using Flurl.Http;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using TwatsAppClient.Helpers;

namespace TwatsAppClient
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private bool isLogin = true;
        public UserBindingModel UserForm { get; set; } = new UserBindingModel();
        public LoginWindow()
        {
            InitializeComponent();
            DataContext = this;
        }


        public static async Task Login(UserBindingModel user,Action<FlurlHttpException> error)
        {
            try
            {
                var response = await API.Account.Login.PostUrlEncodedAsync(new { username = user.UserName, password = user.Password, grant_type = "password" }).ReceiveJson();
                Properties.Settings.Default.Username = user.UserName;
                Properties.Settings.Default.Password = user.Password;
                Properties.Settings.Default.AccessToken = response.access_token;
                Properties.Settings.Default.ExpiresIn = DateTime.Now.AddSeconds(response.expires_in - 60);
                Properties.Settings.Default.UserId = int.Parse(response.userId);
                Properties.Settings.Default.FullName = $"{response.firstName} {response.lastName}";
                Properties.Settings.Default.Save();
            }
            catch (FlurlHttpTimeoutException)
            {
                throw new TimeoutException("The request has timed out.");
            }
            catch (FlurlHttpException ex)
            {
                error(ex);
            }
            catch (Exception)
            {
                throw new Exception("Something weird happend.");
            }
        }


        private async Task Register(UserBindingModel user, Action<FlurlHttpException> error)
        {
            try
            {
                var response = await API.Account.Register.PostJsonAsync(new
                {
                    username = user.UserName,
                    password = user.Password,
                    confirmPassword = user.ConfirmPassword,
                    firstName = user.FirstName,
                    lastName = user.LastName
                });
            }
            catch (FlurlHttpTimeoutException)
            {
                throw new TimeoutException("The request has timed out.");
            }
            catch (FlurlHttpException ex)
            {
                error(ex);
            }
            catch (Exception)
            {
                throw new Exception("Something weird happend.");
            }
        }
        
        private void HandleRegisterError(FlurlHttpException ex)
        {
            switch (ex.Call.HttpStatus)
            {
                case HttpStatusCode.BadRequest:
                    var response = ex.GetResponseJson();
                    AlertContent.Text = string.Empty;
                    if (response.message == API.ModelStateErrorMessage)
                    {
                        foreach (var error in response.modelState)
                        {
                            foreach (string msg in error.Value)
                            {
                                AlertContent.Text += $"{msg}\n";
                            }
                        }
                    }
                    else
                    {
                        AlertContent.Text = response.message;
                    }
                    break;
                case HttpStatusCode.InternalServerError:
                    AlertContent.Text = API.GenericInternalServerError;
                    break;
                default:
                    AlertContent.Text = API.CrazyShitError;
                    break;
            }
            AlertLabel.Visibility = Visibility.Visible;

        }

        private void HandleLoginError(FlurlHttpException ex)
        {
            switch (ex.Call.HttpStatus)
            {
                case HttpStatusCode.BadRequest:
                    var response = ex.GetResponseJson();
                    AlertContent.Text = response.error_description;
                    break;
                case HttpStatusCode.InternalServerError:
                    AlertContent.Text = API.GenericInternalServerError;
                    break;
                default:
                    AlertContent.Text = API.CrazyShitError;
                    break;
            }
            AlertLabel.Visibility = Visibility.Visible;
            throw new LoginException();
        }

        private void SwitchLogin(object sender, RoutedEventArgs e)
        {
            if (isLogin)
            {
                RegisterFields.Visibility = Visibility.Visible;
                LoginBtn.Content = "Register";
                LoginSwitch.Content = "I allready have an account";
            }
            else
            {
                RegisterFields.Visibility = Visibility.Collapsed;
                LoginBtn.Content = "Login";
                LoginSwitch.Content = "I don't have an account";
            }
            AlertLabel.Visibility = Visibility.Hidden;
            AlertContent.Text = string.Empty;
            isLogin = !isLogin;
        }

        private async void LoginBtn_ClickAsync(object sender, RoutedEventArgs e)
        {
            try
            {
                Spinner.Work();
                UserForm.Password = Password.Password;
                if (!isLogin)
                {
                    UserForm.ConfirmPassword = ConfirmPassword.Password;
                    await Register(UserForm, HandleRegisterError);
                }
                await Login(UserForm, HandleLoginError);
                Close();
            }
            catch (LoginException)
            {

            }
            catch (Exception ex)
            {
                AlertLabel.Visibility = Visibility.Visible;
                AlertContent.Text = ex.Message;
            }
            finally
            {
                Spinner.Stop();
            }
        }

    }
}
