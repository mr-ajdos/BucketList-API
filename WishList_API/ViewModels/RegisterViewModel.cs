using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WishList_API.Helpers;
using WishList_API.Models;

namespace WishList_API.ViewModels
{
    public class RegisterViewModel
    {
        public string Username { get; set; }
        public string Password { get; set; }


        public static implicit operator Users(RegisterViewModel model)
        {
            var salt = Cryptography.GenerateSalt();

            return new Users
            {
                Username = model.Username,
                PasswordSalt = salt,
                PasswordHash = Cryptography.GenerateHash(salt, model.Password),
                IsDeleted = false
            };
        }
    }
}