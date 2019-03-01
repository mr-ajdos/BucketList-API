using System;
using System.Linq;
using System.Web.Http;

using WishList_API.Models;
using WishList_API.Helpers;
using WishList_API.ViewModels;

namespace WishList_API.Controllers
{
    [RoutePrefix("api/Access")]
    public class AccessController : ApiController
    {
        private readonly WishlistEntities _db = new WishlistEntities();

        [HttpPost]
        [Route("Register")]
        public IHttpActionResult Register([FromBody]RegisterViewModel model)
        {
            if (model == null)
                return BadRequest("An error ocured while trying to register to Wishlist, please try again later.");

            if (_db.Users.Any(u => !u.IsDeleted && u.Username.Equals(model.Username)))
                return BadRequest($"Username \"{model.Username}\" is already taken.");

            Users user = model;

            try
            {
                _db.Users.Add(user);
                _db.SaveChanges();
            }
            catch (Exception)
            {
                return BadRequest("An error ocured while trying to register to Wishlist, please try again later.");
            }

            return Ok();
        }

        [HttpPost]
        [Route("Login")]
        public IHttpActionResult Login([FromBody]LoginViewModel model)
        {
            if ((!model?.Username.IsSet() ?? true) || (!model?.Password.IsSet() ?? true))
                return BadRequest("Username and password are not provided.");

            var user = _db.Users.FirstOrDefault(u => u.Username.Equals(model.Username));
            if (user == null)
                return BadRequest($"Username or password are not correct.");

            var hash = Cryptography.GenerateHash(user.PasswordSalt, model.Password);

            if (!user.PasswordHash.Equals(hash))
                return BadRequest($"Username or password are not correct.");

            var userAuthTokens = _db.AuthenticationTokens.Where(at => at.UserId == user.Id && !at.IsDeleted).ToList();

            var newAuthToken = new AuthenticationTokens
            {
                DeviceId = model.DeviceId,
                Token = Guid.NewGuid().ToString(),
                TokenDate = DateTime.Now,
                UserId = user.Id,
                IsDeleted = false
            };

            try
            {
                foreach (var item in userAuthTokens)
                {
                    item.IsDeleted = true;
                }

                _db.AuthenticationTokens.Add(newAuthToken);
                _db.SaveChanges();

            }
            catch (Exception)
            {
                return BadRequest("An error ocured while trying to login to Wishlist, please try again later.");
            }

            return Ok(new { UserId = newAuthToken.UserId, Username = user.Username, AuthToken = newAuthToken.Token});
        }

        [HttpGet]
        [Route("CheckAccess")]
        public IHttpActionResult CheckAccess()
        {
            var authToken = Request.Headers.GetValues("authToken").FirstOrDefault()?.ToString() ?? null;
            var authTokenDb = _db.AuthenticationTokens.FirstOrDefault(at => at.Token.Equals(authToken));

            if (!authTokenDb?.IsTokenValid(authToken) ?? true)
                return Unauthorized();

            return Ok();
        }
    }
}
