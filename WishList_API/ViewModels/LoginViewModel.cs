using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WishList_API.ViewModels
{
    public class LoginViewModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string DeviceId { get; set; }
    }
}