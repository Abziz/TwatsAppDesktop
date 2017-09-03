
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using TwatsAppServer.Extensions;
using System.Web.Http;
using TwatsAppCore.Models;
using TwatsAppCore.Services;
using System;

namespace TwatsAppServer.Controllers
{
    [Authorize]
    public class MessagesController : ApiController
    {        
        [HttpPost]
        [Route("message")]
        public async Task<IHttpActionResult> SendMessage(MessageBindingModel message)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (message.ReceiverId == message.SenderId)
                {
                    return BadRequest("Can not send message to yourself");
                }
                if (!HttpContext.Current.GetOwinContext().GetUserId().Equals(message.SenderId.ToString()))
                {
                    return BadRequest("You must be the one sending the message");
                }
                Message msg;
                using (var UserService = new UserService())
                {
                    msg = new Message()
                    {
                        Sender = await UserService.FindById(message.SenderId),
                        Receiver = await UserService.FindById(message.ReceiverId),
                        Content = message.Content,
                    };
                }
                using(var MessageService = new MessageService())
                {
                    await MessageService.SendMessage(msg);
                }
                return Ok();
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }

        }
    }
}
