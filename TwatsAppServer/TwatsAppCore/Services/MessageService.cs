using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using static TwatsAppCore.Helpers.Utils;
using System.Threading.Tasks;
using TwatsAppCore.Models;

namespace TwatsAppCore.Services
{
    public class MessageService : BaseService
    {
        public async Task<Message> FindMessageById(int id)
        {
            return await db.Messages.Where(m => m.Id.Equals(id)).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Message>> GetAllMessages()
        {
            return await db.Messages.ToListAsync();
        }

        public async Task<Conversation> FindConversationById(int id)
        {
            return await db.Conversations.Where(c => c.Id.Equals(id)).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Conversation>> FindUserConversations(int userId)
        {
            return await db.Conversations
                .Where(c => c.LastMessage.Sender.Id == userId || c.LastMessage.Receiver.Id.Equals(userId))
                .OrderByDescending(c => c.LastMessage.TimeSent)
                .ToListAsync();
        }

        public async Task<Conversation> FindConversationByTwoUserIds(int firstId, int secondId)
        {
            if (firstId == secondId)
            {
                throw new Exception();
            }
            return await db.Conversations
                .Where(c => (c.LastMessage.Sender.Id == firstId && c.LastMessage.Receiver.Id == secondId) || (c.LastMessage.Sender.Id == secondId && c.LastMessage.Receiver.Id == firstId))
                .FirstOrDefaultAsync();
        }
        
        
        public async Task SendMessage(Message message)
        {
            var conversation = await FindConversationByTwoUserIds(message.Sender.Id, message.Receiver.Id);
            if (conversation == null)
            {
                conversation = db.Conversations.Add(new Conversation());
                await db.SaveChangesAsync();
            }
            
            db.Users.Attach(message.Sender);
            db.Users.Attach(message.Receiver);
            conversation.Messages.Add(message);
            conversation.LastMessage = message;
            await db.SaveChangesAsync();
        }
    }

}
