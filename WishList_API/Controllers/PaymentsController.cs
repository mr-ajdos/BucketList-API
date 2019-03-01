using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WishList_API.Helpers;
using WishList_API.Models;
using WishList_API.ViewModels;

namespace WishList_API.Controllers
{
    [RoutePrefix("api/Payments")]
    public class PaymentsController : ApiController
    {
        private readonly WishlistEntities _db = new WishlistEntities();
    
        [HttpGet]
        [Route("GetPayments")]
        public IHttpActionResult GetPayments([FromUri]int? wishId)
        {
            if (!wishId.HasValue)
                return BadRequest("An error occurred while trying to load payments, please try again later");

            var authToken = Request.Headers.GetValues("authToken").FirstOrDefault()?.ToString() ?? null;
            var authTokenDb = _db.AuthenticationTokens.FirstOrDefault(at => at.Token.Equals(authToken));

            if (!authTokenDb?.IsTokenValid(authToken) ?? true)
                return Unauthorized();

            var wish = _db.Wishes.FirstOrDefault(w => !w.IsDeleted && w.Id == wishId.Value);
            if (wish == null)
                return BadRequest("This wish doesn't exist or it has been deleted.");

            var payments = _db.Payments.Where(p => !p.IsDeleted && p.WishId == wish.Id).OrderByDescending(p => p.Date).ToList();

            var model = new WishPaymetsViewModel { Payments = new List<PaymentViewModel>() };

            foreach (var item in payments)
            {
                var dateString = item.Date.ToString("dd.MM.yyyy");
                model.Payments.Add(new PaymentViewModel
                {
                    Id = item.Id,
                    Amount = item.Amount,
                    CreatedDate = dateString
                });
            }

            return Ok(model);
        }

        [HttpPost]
        [Route("Edit")]
        public IHttpActionResult Edit([FromBody]Payments model)
        {
            if (model == null)
                return BadRequest("An error occurred while trying to edit payment, please try again later");

            var authToken = Request.Headers.GetValues("authToken").FirstOrDefault()?.ToString() ?? null;
            var authTokenDb = _db.AuthenticationTokens.FirstOrDefault(at => at.Token.Equals(authToken));

            if (!authTokenDb?.IsTokenValid(authToken) ?? true)
                return Unauthorized();

            var payment = _db.Payments.FirstOrDefault(p => p.Id == model.Id && !p.IsDeleted);
            if (payment == null)
                return BadRequest("This payment doesn't exist or it has been deleted.");

            try
            {
                payment.Amount = model.Amount;
                _db.SaveChanges();
            }
            catch (Exception)
            {
                return BadRequest("An error occurred while trying to edit payment, please try again later");
            }

            return Ok();
        }

        [HttpPost]
        [Route("Add")]
        public IHttpActionResult Add([FromBody]Payments model)
        {
            if (model == null)
                return BadRequest("An error occurred while trying to edit payment, please try again later");

            var authToken = Request.Headers.GetValues("authToken").FirstOrDefault()?.ToString() ?? null;
            var authTokenDb = _db.AuthenticationTokens.FirstOrDefault(at => at.Token.Equals(authToken));

            if (!authTokenDb?.IsTokenValid(authToken) ?? true)
                return Unauthorized();

            try
            {
                _db.Payments.Add(model);
                _db.SaveChanges();

                var addedPayment = new PaymentViewModel
                {
                    Id = model.Id,
                    Amount = model.Amount,
                    CreatedDate = model.Date.ToString("dd.MM.yyyy")
                };

                return Ok(addedPayment);
            }
            catch (Exception)
            {
                return BadRequest("An error occurred while trying to edit payment, please try again later");
            }

        }
        
        [HttpPost]
        [Route("Delete")]
        public IHttpActionResult Delete([FromUri]int? paymentId)
        {
            if (!paymentId.HasValue)
                return BadRequest("An error occurred while trying to delete payment, please try again later");

            var authToken = Request.Headers.GetValues("authToken").FirstOrDefault()?.ToString() ?? null;
            var authTokenDb = _db.AuthenticationTokens.FirstOrDefault(at => at.Token.Equals(authToken));

            if (!authTokenDb?.IsTokenValid(authToken) ?? true)
                return Unauthorized();

            var payment = _db.Payments.FirstOrDefault(p => !p.IsDeleted && p.Id == paymentId.Value);
            if(payment == null)
                return BadRequest("This payment doesn't exist or it has been deleted.");

            try
            {
                payment.IsDeleted = true;
                _db.SaveChanges();
            }
            catch (Exception)
            {
                return BadRequest("An error occurred while trying to delete payment, please try again later");
            }

            return Ok();
        }

    }
}
