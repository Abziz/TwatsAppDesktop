using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using TwatsAppCore.Models;
using TwatsAppCore.Models.Dtos;

namespace TwatsAppCore.Services
{
    public class MessageService : BaseService
    {

        /// <summary>
        /// Returns all the Contacts information for a user
        /// </summary>
        /// <param name="id">the user's id</param>
        /// <returns>A list of contacts</returns>
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
                contacts.Add(new ContactDto { LastMessage = new MessageDto(last), User = new UserDto(user), Messages = conversation.Messages.Select(msg => new MessageDto(msg)).ToList()});
            }
            var contactsWithNoConverstations = await db.Users
                .Where(u => !allreadyHaveConversations.Contains(u.Id))
                .Select(u => new ContactDto { User = new UserDto{ Id = u.Id, FullName = u.FirstName + " " + u.LastName,Joined=u.TimeCreated} })
                .ToListAsync();
            contacts.AddRange(contactsWithNoConverstations);
            await db.Messages.Where(x => x.To.Id == id).ForEachAsync(x => x.SeenByReceiver = true);
            await db.Messages.Where(x => x.From.Id == id).ForEachAsync(x => x.SeenBySender = true);
            return contacts;
        }

        /// <summary>
        /// Finds all conversations of a user
        /// </summary>
        /// <param name="id"> the user's id</param>
        /// <returns>A list of conversations</returns>
        public async Task<List<Conversation>> FindUserConversations(int id)
        {
            var list = await db.Conversations
                .Include(x => x.LastMessage)
                .Include(x => x.LastMessage.From)
                .Include(x => x.LastMessage.To)
                .Include(x => x.Messages)
                .Where(c => c.LastMessage.From.Id == id || c.LastMessage.To.Id == (id))
                .OrderByDescending(c => c.LastMessage.DispatchedAt)
                .ToListAsync();
            return list;
        }
        /// <summary>
        /// Finds the conversation between two users
        /// </summary>
        /// <param name="firstId">first user's id</param>
        /// <param name="secondId">second user's id</param>
        /// <returns>The conversation between the users</returns>
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
        /// <summary>
        /// Sends messages from a user
        /// </summary>
        /// <param name="messages">The messages to send</param>
        /// <param name="from">The user's id</param>
        /// <returns> task </returns>
        public async Task SendManyMessages(List<Message> messages,int from)
        {
            var map = messages.GroupBy(x => x.To.Id).ToDictionary(x => x.Key, x => x.ToList());

            foreach( var pair in map)
            {
                var conversation = await FindConversationByTwoUserIds(from, pair.Key);
                if (conversation == null)
                {
                    conversation = db.Conversations.Add(new Conversation());
                    db.SaveChanges();
                }
                foreach( var msg in pair.Value)
                {
                    db.Users.Attach(msg.From);
                    db.Users.Attach(msg.To);
                    conversation.Messages.Add(msg);
                    conversation.LastMessage = msg;
                }
            }
            await db.SaveChangesAsync();
        }

        /// <summary>
        /// Check if there are new messages for a user, and if there are new contacts since a given date
        /// </summary>
        /// <param name="id">The user's id</param>
        /// <param name="lastRefresh">Last time checked for updates</param>
        /// <returns>A list of contacts with their new messages if there are</returns>
        public async Task<List<ContactDto>> CheckForUpdate(int id,DateTimeOffset lastRefresh)
        {
            //find new users - the ones that were created after the last refresh
            List<UserDto> newUsers = await db.Users
                .Where(u => u.TimeCreated > lastRefresh)
                .Select(x => new UserDto { Id = x.Id, FullName = x.FirstName + " " + x.LastName, Joined = x.TimeCreated })
                .ToListAsync();

            //find the current user
            var from = db.Users.Find(id);

            //find not read messages ones that were dispatched after last refresh and are sent to the user
            var notReadQuery = db.Messages
                .Where(m=>m.DispatchedAt >= lastRefresh)
                .Where(m => m.To.Id == id && !m.SeenByReceiver);
            
            //find any conversation that has one of the messages above
            var conversations = await  db.Conversations
                .Include(x => x.LastMessage)
                .Include(x => x.LastMessage.From)
                .Include(x => x.LastMessage.To)
                .Where(c => c.Messages.Any(m => notReadQuery.Contains(m)))
                .ToListAsync();

            var updateList = new List<ContactDto>();

            // create a contactDTO from each conversation and add to the update list
            foreach(var conversation in conversations)
            {
                var last = conversation.LastMessage;
                var user = last.From.Id == id ? last.To : last.From;
                updateList.Add(new ContactDto {
                    LastMessage = new MessageDto(last),
                    User = new UserDto(user),
                    Messages = conversation.Messages.Select(msg => new MessageDto(msg)).ToList()
                });
            }

            //add the new users to the update list as contactDTO
            updateList.AddRange(newUsers.Select(x => new ContactDto { User = x }));

            //set the unread messages to read
            await notReadQuery.ForEachAsync(x => x.SeenByReceiver = true);
            //save everything
            await db.SaveChangesAsync();
            return updateList;
        }
    }

}
