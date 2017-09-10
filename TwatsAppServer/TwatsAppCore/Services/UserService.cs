using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using static TwatsAppCore.Helpers.Utils;
using TwatsAppCore.Models;
using System;
using TwatsAppCore.Models.Binding;

namespace TwatsAppCore.Services
{

    public class UserService : BaseService
    {
        /// <summary>
        /// Finds a  user by given id
        /// </summary>
        /// <param name="id">The user's id</param>
        /// <returns>The user</returns>
        public async Task<TwatsAppUser> FindById(int id)
        {
            return await db.Users.Where(u => u.Id.Equals(id)).FirstOrDefaultAsync();
        }
        /// <summary>
        /// Register a user to the server
        /// </summary>
        /// <param name="userModel">The user's information</param>
        /// <returns>The newly registered user</returns>
        public async Task<TwatsAppUser> RegisterUser(UserRegistrationBindingModel userModel)
        {
            TwatsAppUser user = new TwatsAppUser
            {
                UserName = userModel.UserName,
                FirstName = userModel.FirstName,
                LastName = userModel.LastName,
                PasswordHash = GenerateSHA256String(userModel.Password)
            };
            user = db.Users.Add(user);
            await db.SaveChangesAsync();
            return user;
        }
        /// <summary>
        /// Find a user by username and password
        /// </summary>
        /// <param name="userName">the user's username</param>
        /// <param name="password">the user's password</param>
        /// <returns>The user</returns>
        public async Task<TwatsAppUser> FindByCredentials(string userName, string password)
        {
            string passwordHash = GenerateSHA256String(password);
            return await db.Users.Where(u => u.UserName.Equals(userName) && u.PasswordHash.Equals(passwordHash)).FirstOrDefaultAsync();
        }
      
    }
}

