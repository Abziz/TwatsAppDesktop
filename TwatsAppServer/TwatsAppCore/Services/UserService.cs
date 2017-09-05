using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using static TwatsAppCore.Helpers.Utils;
using TwatsAppCore.Models;
using System;
using System.Collections;
using TwatsAppCore.Models.Binding;

namespace TwatsAppCore.Services
{

    public class UserService : BaseService
    {
        public async Task<TwatsAppUser> FindById(int id)
        {
            return await db.Users.Where(u => u.Id.Equals(id)).FirstOrDefaultAsync();
        }
        public async Task<TwatsAppUser> FindById(string id)
        {
            return await db.Users.Where(u => u.Id.ToString().Equals(id)).FirstOrDefaultAsync();
        }
        public async Task<List<TwatsAppUser>> GetAllUsers()
        {
            return await db.Users.ToListAsync();
        }
        public async Task<List<TwatsAppUser>> GetAllNewUsers(DateTimeOffset LastChecked)
        {
            return await db.Users.Where(u => u.TimeCreated > LastChecked).ToListAsync();
        }
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
        public async Task<TwatsAppUser> FindByCredentials(string userName, string password)
        {
            string passwordHash = GenerateSHA256String(password);
            return await db.Users.Where(u => u.UserName.Equals(userName) && u.PasswordHash.Equals(passwordHash)).FirstOrDefaultAsync();
        }
      
    }
}

