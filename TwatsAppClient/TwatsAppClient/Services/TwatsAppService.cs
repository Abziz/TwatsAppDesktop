using Flurl.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwatsAppClient.Helpers;

namespace TwatsAppClient.Services
{
    /// <summary>
    /// Simple static class that holds the url strings for http calls
    /// also it has an API for the calls allowing to pass success and error function delegates
    /// </summary>
    public class TwatsAppService
    {
        private TwatsAppService() { }

        public class API
        {
            public const string host = "http://localhost:50504/";
            public const string ModelStateErrorMessage = "The request is invalid.";
            public const string GenericInternalServerErrorMessage = "Something went wrong on our servers.";
            public const string GenericTimeoutErrorMessage = "Request has timed out.";
            public const string GenericWeirdErrorMessage = "Something weird happend.";
            public const string CrazyShitErrorMessage = "Someone fucked up.";
            public const string Register = host + "register";
            public const string Login = host + "token";
            public const string SendMessages = host + "messages";
            public const string GetContacts = host + "contacts";
            public const string GetUpdates = host + "update";
        }


        /// <summary>
        /// Register at the server, will log in when done
        /// </summary>
        /// <param name="user"> the user to register</param>
        /// <param name="error">error callback</param>
        /// <param name="success">success callback</param>
        /// <returns></returns>
        public static async Task Register(AccountBindingModel user, Action<FlurlHttpException> error = null, Action<dynamic> success = null)
        {
            try
            {
                var response = await API.Register.PostJsonAsync(user).ReceiveJson();
                success?.Invoke(response);
            }
            catch (FlurlHttpTimeoutException ex)
            {
                throw new Exception(API.GenericTimeoutErrorMessage, ex);
            }
            catch (FlurlHttpException ex)
            {
                error?.Invoke(ex);
            }
            catch (Exception ex)
            {
                throw new Exception(API.GenericWeirdErrorMessage, ex);
            }
        }

        /// <summary>
        /// Login to the server , basically receive a token that will be saved for next http requests
        /// </summary>
        /// <param name="user"> the user to login</param>
        /// <param name="error">error callback</param>
        /// <param name="success">success callback</param>
        /// <returns></returns>
        public static async Task Login(AccountBindingModel user, Action<FlurlHttpException> error = null, Action<dynamic> success = null)
        {
            try
            {
                var response = await API.Login.PostUrlEncodedAsync(new { username = user.UserName, password = user.Password, grant_type = "password" }).ReceiveJson();
                Properties.Settings.Default.Username = user.UserName;
                Properties.Settings.Default.Password = user.Password;
                Properties.Settings.Default.AccessToken = response.access_token;
                Properties.Settings.Default.ExpiresIn = DateTime.Now.AddSeconds(response.expires_in - 60);
                Properties.Settings.Default.UserId = int.Parse(response.userId);
                Properties.Settings.Default.FullName = $"{response.firstName} {response.lastName}";
                Properties.Settings.Default.Save();
                success?.Invoke(response);
            }
            catch (FlurlHttpTimeoutException ex)
            {
                throw new Exception(API.GenericTimeoutErrorMessage, ex);
            }
            catch (FlurlHttpException ex)
            {
                error?.Invoke(ex);
            }
            catch (Exception ex)
            {
                throw new Exception(API.GenericWeirdErrorMessage, ex);
            }
        }

        /// <summary>
        /// Receives the Contacts information and their messages.
        /// </summary>
        /// <param name="error">error callback</param>
        /// <param name="success">success callback</param>
        /// <returns></returns>
        public static async Task GetContactsAndMessages(Action<FlurlHttpException> error = null, Action<List<ContactDto>> success = null)
        {
            try
            {
                var response = await API.GetContacts.WithOAuthBearerToken(Properties.Settings.Default.AccessToken).GetJsonAsync<List<ContactDto>>();
                Properties.Settings.Default.LastRefresh = DateTimeOffset.Now;
                success?.Invoke(response);
            }
            catch (FlurlHttpTimeoutException ex)
            {
                throw new Exception(API.GenericTimeoutErrorMessage, ex);
            }
            catch (FlurlHttpException ex)
            {
                error.Invoke(ex);
            }
            catch (Exception ex)
            {
                throw new Exception(API.GenericWeirdErrorMessage, ex);
            }
        }

        /// <summary>
        /// Dispatches the messages you sent to other contacts
        /// </summary>
        /// <param name="messages">A list of messages to dispatch</param>
        /// <param name="error">error callback</param>
        /// <param name="success">success callback</param>
        /// <returns></returns>
        public static async Task SendMessages(List<SendMessageBindingModel> messages, Action<FlurlHttpException> error = null, Action<List<ContactDto>> success = null)
        {
            try
            {
                var response = await API.SendMessages.WithOAuthBearerToken(Properties.Settings.Default.AccessToken).PostJsonAsync(messages).ReceiveJson();
                success?.Invoke(response);
            }
            catch (FlurlHttpTimeoutException ex)
            {
                throw new Exception(API.GenericTimeoutErrorMessage, ex);
            }
            catch (FlurlHttpException ex)
            {
                error.Invoke(ex);
            }
            catch (Exception ex)
            {
                throw new Exception(API.GenericWeirdErrorMessage, ex);
            }
        }

        /// <summary>
        /// Check if there are new contacts and new messages
        /// </summary>
        /// <param name="error">error callback</param>
        /// <param name="success">success callback</param>
        /// <returns></returns>
        public static async Task GetUpdates(Action<FlurlHttpException> error = null, Action<List<ContactDto>> success = null)
        {
            try
            {
                var response = await API.GetUpdates.WithOAuthBearerToken(Properties.Settings.Default.AccessToken).PostJsonAsync(Properties.Settings.Default.LastRefresh).ReceiveJson<List<ContactDto>>();
                Properties.Settings.Default.LastRefresh = DateTimeOffset.Now;
                success?.Invoke(response);
            }
            catch (FlurlHttpTimeoutException ex)
            {
                throw new Exception(API.GenericTimeoutErrorMessage, ex);
            }
            catch (FlurlHttpException ex)
            {
                error.Invoke(ex);
            }
            catch (Exception ex)
            {
                throw new Exception(API.GenericWeirdErrorMessage, ex);
            }
        }

    }
}
