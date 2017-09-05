using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwatsAppCore.Models.Dtos
{
    public class MessageDto
    {
        public UserDto From { get; set; }
        public UserDto To { get; set; }
        public string Content { get; set; }
        public DateTimeOffset DispatchedAt { get; set; }
        public bool Seen { get; set; }

        public MessageDto(Message msg)
        {
            From = new UserDto(msg.From);
            To = new UserDto(msg.To);
            Content = msg.Content;
            DispatchedAt = msg.DispatchedAt;
            Seen = msg.Seen;
        }
    }

    public class ContactDto
    {
        public UserDto User { get; set; }
        public MessageDto LastMessage { get; set; }
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        DateTimeOffset Joined { get; set; }
        public UserDto() { }
        public UserDto(TwatsAppUser user)
        {
            Id = user.Id;
            FullName = user.FirstName + " " + user.LastName;
            Joined = user.TimeCreated;
        }
    }
}
