
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using TwatsAppServer.Extensions;
using System.Web.Http;
using TwatsAppCore.Models;
using TwatsAppCore.Services;
using System;
using TwatsAppCore.Models.Binding;
using System.Collections.Generic;
using System.Linq;

namespace TwatsAppServer.Controllers
{
    [Authorize]
    public class MessagesController : ApiController
    {
        private int CurrentUserId => int.Parse(HttpContext.Current.GetOwinContext().GetUserId());

        
        [HttpGet]
        [Route("contacts")]
        public async Task<IHttpActionResult> GetContacts() {

            try
            {
                using (var MessageService = new MessageService())
                {
                    var contacts = await MessageService.GetAllContactsForUser(CurrentUserId);
                    return Ok(contacts);
                }
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        [HttpPost]
        [Route("messages")]
        public async Task<IHttpActionResult> SendMessages([FromBody]List<SendMessageBindingModel> messages)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if(messages.Exists( m=> m.From == m.To))
                {
                    return BadRequest("Can not send message to yourself");
                }
                if( messages.Exists( m=> m.From != CurrentUserId))
                {
                    return BadRequest("You must be the one sending the message");
                }
                var messageDictionary  = messages.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.Select( m => new Message() {Content = m.Content,DispatchedAt =m.DispatchedAt,SeenBySender=true}).ToList());
                using (var UserService = new UserService())
                {
                    var from = await UserService.FindById(CurrentUserId);
                    foreach( var group in messageDictionary)
                    {
                        var to = await UserService.FindById(group.Key);
                        foreach(var msg in group.Value)
                        {
                            msg.From = from;
                            msg.To = to;
                        }
                    }
                }
                using(var MessageService = new MessageService())
                {
                    await MessageService.SendManyMessages(messageDictionary.SelectMany(d => d.Value).ToList(),CurrentUserId);
                }
                return Ok();
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        [HttpPost]
        [Route("update")]
        public async Task<IHttpActionResult> CheckUpdates([FromBody]DateTimeOffset LastRefresh)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if( LastRefresh > DateTimeOffset.Now)
                {
                    return Ok();//what else?
                }
                using( var MessageService = new MessageService())
                {
                    return Ok(await MessageService.CheckForUpdate(CurrentUserId,LastRefresh));
                }
            }
            catch(Exception e)
            {
                return InternalServerError(e);
            }
            
        }
     
    }
}
