using Flurl.Http;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TwatsAppClient.Helpers;

namespace TwatsAppClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<ContactDto> Contacts { get; set; } = new ObservableCollection<ContactDto>();
        //public ObservableCollection Messages { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

           // Contacts.Add(new ContactBindingModel { FullName = "Tal Abziz", NotRead=true, LastMessageDate = DateTimeOffset.Now ,LastMessageText="Wtf arreea asslkh gdjks agkjhgsdakghd sgdas "});
           // Contacts.Add(new ContactBindingModel { FullName = "AssHole Shay", NotRead = false,LastMessageDate = DateTimeOffset.Now ,LastMessageText= "Wtf arreea asslkh gdjks agkjhgsdakghd sgdassafff cafsasfasf " });
        }

        protected override async void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            await ValidateLogin();
            await InitializeApp();
        }

        private async Task ValidateLogin()
        {
            if (!HasAllSettings())
            {
                ShowLoginWindow();
            }
            else
            {
                try
                {
                    var user = new UserBindingModel { UserName = Properties.Settings.Default.Username, Password = Properties.Settings.Default.Password };
                    await LoginWindow.Login(user, ex => { throw new LoginException(); });
                    return;
                }
                catch (LoginException)
                {
                    ShowLoginWindow();
                }
            }
        }

        private bool HasAllSettings()
        {
            return !string.IsNullOrEmpty(Properties.Settings.Default.AccessToken)
                && !string.IsNullOrEmpty(Properties.Settings.Default.Username)
                && !string.IsNullOrEmpty(Properties.Settings.Default.Password)
                && !(Properties.Settings.Default.ExpiresIn == null);
        }

        private void HandleLoginError(FlurlHttpException ex)
        {

        }

        private bool TokenExpired()
        {
            return Properties.Settings.Default.ExpiresIn >= DateTime.Now; 
        }

        private void Signout(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.AccessToken ="";
            Properties.Settings.Default.Save();
            ShowLoginWindow();
        }

        private async void ShowLoginWindow()
        {
            Hide();
            new LoginWindow().ShowDialog();
            if(string.IsNullOrEmpty(Properties.Settings.Default.AccessToken))
            {
                Close();
            }
            Show();
            await InitializeApp();
        }

        private async Task InitializeApp()
        {
             
            await GetContacts(x => { });
        }


        public async Task GetContacts(Action<FlurlHttpException> error)
        {
            if( Contacts.Count != 0)
            {
                Contacts.Clear();
            }
            try
            {
                var contacts = await API.Chat.GetContacts.WithOAuthBearerToken(Properties.Settings.Default.AccessToken).GetJsonAsync<List<ContactDto>>();
                contacts.OrderByDescending(c => c.LastMessage?.DispatchedAt);
                foreach(var contact in contacts)
                {
                    Contacts.Add(contact);
                }
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
    }
}
