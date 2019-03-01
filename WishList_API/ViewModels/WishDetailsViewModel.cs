using System.Collections.Generic;

namespace WishList_API.ViewModels
{
    public class WishDetailsViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public decimal AmountSaved { get; set; }
        public decimal Difference { get; set; }
        public decimal Percentage { get; set; }
    }
}