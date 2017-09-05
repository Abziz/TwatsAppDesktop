using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwatsAppClient.Helpers
{
    static public class API
    {

        private const string host = "http://localhost:50504/";
        public const string ModelStateErrorMessage = "The request is invalid.";
        public const string GenericInternalServerError = "Something went wrong on our servers.";
        public const string CrazyShitError = "Someone fucked up.";
        public static class Account
        {
            public const string Register = host + "register";
            public const string Login = host + "token";
        }

        public static class Chat
        {
            public const string SendMessage = host + "messages";
            public const string GetContacts = host + "contacts";
        }
    }
}
