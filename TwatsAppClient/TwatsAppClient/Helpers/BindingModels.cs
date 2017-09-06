using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwatsAppClient.Helpers
{
    public class UserBindingModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class MessageBindingModel
    {
        public string Content { get; set; }
        public DateTimeOffset DispatchedAt { get; set; }
        public bool NotRead { get; set; }

    }

    public class ContactBindingModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public ObservableCollection<MessageBindingModel> Messages { get; set; } = new ObservableCollection<MessageBindingModel>();
        public MessageBindingModel LastMessage { get; set; }
    }

    public class MessageDto
    {
        public UserDto From { get; set; }
        public UserDto To { get; set; }
        public string Content { get; set; }
        public DateTimeOffset DispatchedAt { get; set; }
        public bool NotRead { get; set; }
    }

    public class ContactDto
    {
        public UserDto User { get; set; }
        public MessageDto LastMessage { get; set; }
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string FullName { get; set;}
        DateTimeOffset Joined { get; set; }
    }

}
