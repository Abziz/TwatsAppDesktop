using Flurl.Http;
using MyToolkit.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using TwatsAppClient.Helpers;
using TwatsAppClient.Services;

namespace TwatsAppClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollectionView<ContactDto> Contacts { get; set; }
        private List<SendMessageBindingModel> MessagesQueue { get; set; } = new List<SendMessageBindingModel>();
        private bool NoRefresh = false;

        public MainWindow()
        {
            Contacts = new ObservableCollectionView<ContactDto>()
            {
                Order = c => c.LastMessage?.DispatchedAt,
                Ascending = false
            };
            Initialized += MainWindowLoaded;
            InitializeComponent();
        }

        /// <summary>
        /// Gets the initial data needed for the chat to start
        /// </summary>
        private async Task InitializeApp()
        {

            Spinner.Work();
            Hide();
            bool logged = await Authenticate();
            if (!logged)
            {
                Close();
                return;
            }
            Show();
            await TwatsAppService.GetContactsAndMessages(error: GetContactsError, success: GetContactsSuccess);
            Spinner.Stop();
            NoRefresh = false;
            return;
        }

        /// <summary>
        /// Authenticates the user, if it is his first launch it will take him to login screen
        /// else, it would authenticate  based on Properties.settings.default
        /// </summary>
        /// <returns> a boolean indicating if the user successfully logged in</returns>
        private async Task<bool> Authenticate()
        {
            NoRefresh = true;
            try
            {
                if (HasAllSettings())
                {
                    await TwatsAppService.Login(new AccountBindingModel { UserName = Properties.Settings.Default.Username, Password = Properties.Settings.Default.Password }, ex => { throw new LoginException(); });
                    NoRefresh = false;
                    return true;
                }
                return RequestLoginInNewWindow();
            }
            catch (LoginException)
            {
                return RequestLoginInNewWindow();
            }
        }
        /// <summary>
        /// simple check to validate user's settings
        /// </summary>
        /// <returns> true if all settings exist</returns>
        private bool HasAllSettings()
        {
            return !string.IsNullOrEmpty(Properties.Settings.Default.AccessToken)
                && !string.IsNullOrEmpty(Properties.Settings.Default.Username)
                && !string.IsNullOrEmpty(Properties.Settings.Default.Password)
                && !(Properties.Settings.Default.ExpiresIn == null);
        }
        /// <summary>
        /// Sends the user to login in a different window
        /// </summary>
        /// <returns> boolean indicating the window's state false if close, true if confirmed</returns>
        private bool RequestLoginInNewWindow()
        {
            var result = new LoginWindow().ShowDialog();
            return result ?? false;
        }
        /// <summary>
        /// check wether the authentication token was expired
        /// </summary>
        /// <returns>true if expired</returns>
        private bool TokenExpired()
        {
            return Properties.Settings.Default.ExpiresIn >= DateTime.Now;
        }

        /* error and success callbacks */
        private void GetContactsError(FlurlHttpException response)
        {
            MessageBox.Show("An error occured while trying to get information from our servers.");
            Close();
        }
        private void GetContactsSuccess(List<ContactDto> contacts)
        {
            Contacts.Items.Clear();
            foreach (var contact in contacts)
            {
                if (contact.Messages == null)
                {
                    contact.Messages = new ObservableCollectionView<MessageDto>()
                    {
                        Order = m => m.DispatchedAt
                    };
                }

                Contacts.Items.Add(contact);
            }
        }
        private void GetUpdateSuccess(List<ContactDto> contacts)
        {
            foreach (var contact in contacts)
            {
                if (contact.Messages == null)
                {
                    contact.Messages = new ObservableCollectionView<MessageDto>()
                    {
                        Order = c => c.DispatchedAt,
                        Ascending = false
                    };
                    Contacts.Items.Add(contact);
                    continue;
                }
                else
                {
                    var con = Contacts.Items.Where(c => c.User.Id == contact.User.Id).SingleOrDefault();
                    foreach (var msg in contact.Messages)
                    {
                        con.Messages.Items.Add(msg);
                    }
                    con.LastMessage = contact.Messages.OrderByDescending(m => m.DispatchedAt).FirstOrDefault();
                }
                contact.Messages.Refresh();
                ConversationScrollViewer.ScrollToBottom();
            }
            Contacts.Refresh();
            
        }

        /* For Event handlers */
        private void SendMessage()
        {
            var selected = ContactList.SelectedItem as ContactDto;

            var msg = new MessageDto
            {
                From = new UserDto { Id = Properties.Settings.Default.UserId, FullName = Properties.Settings.Default.FullName },
                To = selected.User,
                Content = MessageInput.Text,
                DispatchedAt = DateTimeOffset.Now
            };
            MessageInput.Clear();
            selected.Messages.Items.Add(msg);
            selected.LastMessage = msg;
            Contacts.Refresh();
            MessagesQueue.Add(new SendMessageBindingModel
            {
                Content = msg.Content,
                From = Properties.Settings.Default.UserId,
                To = selected.User.Id,
                DispatchedAt = msg.DispatchedAt
            });
            ConversationScrollViewer.ScrollToBottom();
        }
        private async void MainWindowLoaded(object sender, EventArgs e)
        {
            await InitializeApp();
            DispatcherTimer RefreshTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(10)
            };
            RefreshTimer.Tick += ApplicationRoutine;
            RefreshTimer.Start();
        }
        private async void Signout(object sender, RoutedEventArgs e)
        {
            Hide();
            Properties.Settings.Default.AccessToken = "";
            Properties.Settings.Default.Save();
            await InitializeApp();
        }
        private async void ApplicationRoutine(object sender, EventArgs e)
        {
            try
            {

                if (NoRefresh)
                {
                    return;
                }
                // Send new messages
                if (MessagesQueue.Count != 0)
                {
                    var temp = new List<SendMessageBindingModel>(MessagesQueue);
                    MessagesQueue.Clear();
                    await TwatsAppService.SendMessages(temp);
                }
                await TwatsAppService.GetUpdates(success: GetUpdateSuccess);
                if (TokenExpired())
                {
                    var logged = await Authenticate();
                    if (!logged)
                    {
                        Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message);
                Close();
            }
        }
        private void ContactSelected(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ConversationViewer.DataContext = (ContactList.SelectedItem as ContactDto);
            ConversationScrollViewer.ScrollToBottom();
        }
        private void CheckCtrlEnter(object sender, KeyEventArgs e)
        {
            if (MessageInput.Text.Length == 0)
            {
                return;
            }
            if (e.Key == Key.Enter)
            {
                if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                {
                    SendMessage();
                }
            }
        }
        private void SendMessageBtnClicked(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

    }
}
