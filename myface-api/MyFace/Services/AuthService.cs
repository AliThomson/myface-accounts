using MyFace.Repositories;
using MyFace.Models.Database;
using System;
using System.Text;
using MyFace.Helpers;


namespace MyFace.Services
{
    public interface IAuthService
    {
        bool IsValidUsernameAndPassword(string username, string password);
    }

    public class AuthService : IAuthService
    {
        private readonly IUsersRepo _users;

        public AuthService(IUsersRepo users)
        {
            _users = users;
        }

        public bool IsValidUsernameAndPassword(string username, string password)
        {
            User user;
            try
            {
                user = _users.GetByUsername(username);
            }
            catch (InvalidOperationException)
            {
                return false;
            }

            // hash user's password and check it
            var helper = new PasswordHelper();
            string saltString = user.Salt;
            byte[] salt = Convert.FromBase64String(saltString);
            
            string hashed = helper.GetHashedPassword(password, salt);
            //Console.WriteLine("Password = " + password + ", LoggedIn = " + hashed + ", dbPassword = " + user.HashedPassword);
            string saltAfterConversion = Convert.ToBase64String(salt);
            //Console.WriteLine("Salt = " + saltString + ", after conversion = "+ saltAfterConversion);     
            
            if (hashed != user.HashedPassword)
            {
                return false;
            }

            return true;
        }
    }
}