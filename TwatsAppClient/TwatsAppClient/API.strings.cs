using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwatsAppClient
{
    static public class API
    {
        private const string host = "http://localhost:50504/";
		public static class Account
        {
            public const string Register = host + "register";
            public const string Login = host + "token";
        }

        public static class Chat
        {
            public const string SendMessage = host + "message";
        }
    }
}
