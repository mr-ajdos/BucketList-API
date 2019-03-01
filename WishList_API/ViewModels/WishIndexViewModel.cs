using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WishList_API.ViewModels
{
    public class WishIndexViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public decimal AmountSaved { get; set; }
    }
}