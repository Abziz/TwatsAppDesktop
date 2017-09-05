using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using static TwatsAppCore.Helpers.Utils;
using System.Threading.Tasks;
using TwatsAppCore.Models;
using TwatsAppCore.Models.Dtos;

namespace TwatsAppCore.Services
{
    public class MessageService : BaseService
    {
        public async Task<Message> FindMessageById(int id)
        {
            return await db.Messages.Where(m => m.Id.Equals(id)).FirstOrDefaultAsync();
        }

        public async Task<List<Message>> GetAllMessages()
        {
            return await db.Messages.ToListAsync();
        }

        public async Task<List<ContactDto>> GetAllContactsForUser(int id)
        {
            var contacts = new List<ContactDto>();
            var conversations = await FindUserConversations(id);
            var allreadyHaveConversations = new List<int> { id };
            foreach (var conversation in conversations)
            {
                var last = conversation.LastMessage;
                var user = last.From.Id == id ? last.To : last.From;
                allreadyHaveConversations.Add(user.Id);
                contacts.Add(new ContactDto { LastMessage = new MessageDto(last), User = new UserDto(user) });
            }
            var contactsWithNoConverstations = await db.Users
                .Where(u => !allreadyHaveConversations.Contains(u.Id))
                .Select(u => new ContactDto { User = new UserDto{ Id = u.Id, FullName = u.FirstName + " " + u.LastName} })
                .ToListAsync();
            contacts.AddRange(contactsWithNoConverstations);
            return contacts;
        }

        public async Task<Conversation> FindConversationById(int id)
        {
            return await db.Conversations.Where(c => c.Id.Equals(id)).FirstOrDefaultAsync();
        }

        public async Task<List<Conversation>> FindUserConversations(int userId)
        {
            var list = await db.Conversations
                .Include(x => x.LastMessage)
                .Include(x => x.LastMessage.From)
                .Include(x => x.LastMessage.To)
                .Where(c => c.LastMessage.From.Id == userId || c.LastMessage.To.Id == (userId))
                .OrderByDescending(c => c.LastMessage.DispatchedAt)
                .ToListAsync();
            return list;
        }

        public async Task<Conversation> FindConversationByTwoUserIds(int firstId, int secondId)
        {
            if (firstId == secondId)
            {
                throw new Exception();
            }
            return await db.Conversations
                .Where(c => (c.LastMessage.From.Id == firstId && c.LastMessage.To.Id == secondId) || (c.LastMessage.From.Id == secondId && c.LastMessage.To.Id == firstId))
                .FirstOrDefaultAsync();
        }


        public async Task SendMessage(Message message)
        {
            var conversation = await FindConversationByTwoUserIds(message.From.Id, message.To.Id);
            if (conversation == null)
            {
                conversation = db.Conversations.Add(new Conversation());
                await db.SaveChangesAsync();
            }

            db.Users.Attach(message.From);
            db.Users.Attach(message.To);
            conversation.Messages.Add(message);
            conversation.LastMessage = message;
            await db.SaveChangesAsync();
        }
    }

}
