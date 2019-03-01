using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WishList_API.Models;

namespace WishList_API.ViewModels
{
    public class WishAddViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public decimal Amount { get; set; }
        public int UserId { get; set; }

        public static implicit operator Wishes(WishAddViewModel model)
        {
            return new Wishes
            {
                Name = model.Name,
                Amount = model.Amount,
                UserId = model.UserId,
                IsDeleted = false
            };
        }
    }
}