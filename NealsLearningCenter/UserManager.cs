using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NealsLearningCenter
{
    public interface IUserManager
    {
        User LogIn(string email, string password);
        User Register(string email, string password);
    }

    public class UserManager
    {
        public User Login(string email, string password)
        {
            var user = DatabaseAccessor.Instance.Users.FirstOrDefault(t => t.UserEmail.ToLower() == email.ToLower()
                                      && t.UserPassword == password);
            if (user == null)
            {
                return null;
            }

            return user;
        }
        public User Register(string email, string password)
        {
            var user = DatabaseAccessor.Instance.Users
        .Add(new User { UserEmail = email, UserPassword = password });

            DatabaseAccessor.Instance.SaveChanges();

            return user;
        }
    }
}