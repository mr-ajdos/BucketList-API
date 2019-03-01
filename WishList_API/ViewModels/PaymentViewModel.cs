using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WishList_API.ViewModels
{
    public class PaymentViewModel
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string CreatedDate { get; set; }
    }
}