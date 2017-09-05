using System;
using System.Threading.Tasks;
using System.Web.Http;
using TwatsAppCore.Models.Binding;
using TwatsAppCore.Services;

namespace TwatsAppServer.Controllers
{
    [Authorize]
    public class UsersController : ApiController
    {
        [HttpPost]
        [AllowAnonymous]
        [Route("register")]
        public async Task<IHttpActionResult> Register([FromBody] UserRegistrationBindingModel user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                using (var UserService = new UserService())
                {
                    var found = await UserService.FindByCredentials(user.UserName, user.Password);
                    if (found != null)
                    {
                        return BadRequest("Username is allready taken");
                    }
                    await UserService.RegisterUser(user);
                    return Ok();
                }
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }




    }
}
