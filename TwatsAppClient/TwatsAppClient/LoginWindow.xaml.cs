using Flurl.Http;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using TwatsAppClient.Helpers;
using TwatsAppClient.Services;

namespace TwatsAppClient
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private bool isLogin = true;
        public AccountBindingModel UserForm { get; set; } = new AccountBindingModel();

        public LoginWindow()
        {
            InitializeComponent();
        }

        /* error and success callbacks */
        private void HandleRegisterError(FlurlHttpException ex)
        {
            switch (ex.Call.HttpStatus)
            {
                case HttpStatusCode.BadRequest:
                    var response = ex.GetResponseJson();
                    AlertContent.Text = string.Empty;
                    if (response.message == TwatsAppService.API.ModelStateErrorMessage)
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
                    AlertContent.Text = TwatsAppService.API.GenericInternalServerErrorMessage;
                    break;
                default:
                    AlertContent.Text = TwatsAppService.API.CrazyShitErrorMessage;
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
                    AlertContent.Text = TwatsAppService.API.GenericInternalServerErrorMessage;
                    break;
                default:
                    AlertContent.Text = TwatsAppService.API.CrazyShitErrorMessage;
                    break;
            }
            AlertLabel.Visibility = Visibility.Visible;
            throw new LoginException();
        }

        /* For Event Handlers */
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
        private async void LoginBtnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Spinner.Work();
                UserForm.Password = Password.Password;
                if (!isLogin)
                {
                    UserForm.ConfirmPassword = ConfirmPassword.Password;
                    await TwatsAppService.Register(UserForm, HandleRegisterError);
                }
                await TwatsAppService.Login(UserForm, HandleLoginError);
                DialogResult = true;
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
