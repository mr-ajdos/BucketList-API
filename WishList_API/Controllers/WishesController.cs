using System;
using System.Linq;
using System.Web.Http;

using WishList_API.Models;
using WishList_API.Helpers;
using WishList_API.ViewModels;

namespace WishList_API.Controllers
{
    [RoutePrefix("api/Wishes")]
    public class WishesController : ApiController
    {
        private readonly WishlistEntities _db = new WishlistEntities();

        [HttpGet]
        [Route("GetWishes")]
        public IHttpActionResult GetWishes([FromUri]int? userId)
        {
            if (!userId.HasValue)
                return BadRequest("An error occurred while trying to load wishes, please try again later");

            var authToken = Request.Headers.GetValues("authToken").FirstOrDefault()?.ToString() ?? null;
            var authTokenDb = _db.AuthenticationTokens.FirstOrDefault(at => at.Token.Equals(authToken));

            if (!authTokenDb?.IsTokenValid(authToken) ?? true)
                return Unauthorized();

            var user = _db.Users.FirstOrDefault(u => !u.IsDeleted && u.Id == userId.Value);
            if (user == null)
                return BadRequest("This account doesn't exist, please create a new one");

            var model = _db.Wishes.Where(w => !w.IsDeleted && w.UserId == user.Id).Select(w =>  new WishIndexViewModel
            {
                Id = w.Id,
                Name = w.Name,
                Amount = w.Amount,
                AmountSaved = _db.Payments.Where(p => !p.IsDeleted && p.WishId == w.Id).Sum(p => (decimal?)p.Amount ?? (decimal?)0) ?? 0
            }).ToList();

            return Ok(model);
        }

        [HttpGet]
        [Route("GetDetails")]
        public IHttpActionResult GetDetails([FromUri]int? wishId)
        {
            if (!wishId.HasValue)
                return BadRequest("An error occurred while trying to load wish details, please try again later");

            var authToken = Request.Headers.GetValues("authToken").FirstOrDefault()?.ToString() ?? null;
            var authTokenDb = _db.AuthenticationTokens.FirstOrDefault(at => at.Token.Equals(authToken));

            if (!authTokenDb?.IsTokenValid(authToken) ?? true)
                return Unauthorized();

            var wish = _db.Wishes.FirstOrDefault(w => !w.IsDeleted && w.Id == wishId.Value);
            if (wish == null)
                return BadRequest("This wish doesn't exist or it has been deleted.");

            var model = new WishDetailsViewModel
            {
                Id = wish.Id,
                Amount = wish.Amount,
                UserId = wish.UserId,
                Name = wish.Name,
            };

            var payments = _db.Payments.Where(p => !p.IsDeleted && p.WishId == wish.Id).OrderByDescending(p => p.Date).ToList();
           
            model.AmountSaved =payments?.Sum(p => p.Amount) ?? 0;
            model.Difference = model.Amount - model.AmountSaved;
            model.Percentage = (model.AmountSaved / model.Amount) * 100;

            return Ok(model);
        }

        [HttpPost]
        [Route("Add")]
        public IHttpActionResult Add([FromBody] WishAddViewModel model)
        {
            var authToken = Request.Headers.GetValues("authToken").FirstOrDefault()?.ToString() ?? null;
            var authTokenDb = _db.AuthenticationTokens.FirstOrDefault(at => at.Token.Equals(authToken));

            if (!authTokenDb?.IsTokenValid(authToken) ?? true)
                return Unauthorized();

            if (!ModelState.IsValid)
                return BadRequest("An error occurred while trying to add new wish, please try again later.");

            Wishes wish = model;

            try
            {
                _db.Wishes.Add(wish);
                _db.SaveChanges();
            }
            catch (Exception)
            {
                return BadRequest("An error occurred while trying to add new wish, please try again later.");
            }

            return Ok();
        }

        [HttpPost]
        [Route("Edit")]
        public IHttpActionResult Edit([FromBody] Wishes model)
        {
            if (model == null)
                return BadRequest("An error occurred while trying to edit wish, please try again later.");

            var authToken = Request.Headers.GetValues("authToken").FirstOrDefault()?.ToString() ?? null;
            var authTokenDb = _db.AuthenticationTokens.FirstOrDefault(at => at.Token.Equals(authToken));

            if (!authTokenDb?.IsTokenValid(authToken) ?? true)
                return Unauthorized();

            var wish = _db.Wishes.FirstOrDefault(w => !w.IsDeleted && w.Id == model.Id);
            if (wish == null)
                return BadRequest("This wish doesn't exist or it has been deleted.");

            try
            {
                wish.Amount = model.Amount;
                wish.Name = model.Name;

                _db.SaveChanges();
            }
            catch (Exception)
            {
                return BadRequest("An error occurred while trying to add new wish, please try again later.");
            }

            return Ok();
        }

        [HttpPost]
        [Route("Delete")]
        public IHttpActionResult Delete([FromUri]int? wishId)
        {
            if (!wishId.HasValue)
                return BadRequest("An error occurred while trying to delete wish, please try again later");

            var authToken = Request.Headers.GetValues("authToken").FirstOrDefault()?.ToString() ?? null;
            var authTokenDb = _db.AuthenticationTokens.FirstOrDefault(at => at.Token.Equals(authToken));

            if (!authTokenDb?.IsTokenValid(authToken) ?? true)
                return Unauthorized();

            var wish = _db.Wishes.FirstOrDefault(w => !w.IsDeleted && w.Id == wishId);
            if (wish == null)
                return BadRequest("This wish doesn't exist or it has been deleted.");

            try
            {
                wish.IsDeleted = true;
                _db.SaveChanges();
            }
            catch (Exception)
            {
                return BadRequest("An error occurred while trying to delete wish, please try again later");
            }

            return Ok();
        }


    }
}
