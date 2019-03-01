using System;
using WishList_API.Models;

namespace WishList_API.Helpers
{
    public static class Extensions
    {
        public static bool IsSet(this string text)
        {
            return !(string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text));
        }

        public static bool IsTokenValid(this AuthenticationTokens tokenDb, string token)
        {
            var tokenActiveHours = (DateTime.Now - (tokenDb?.TokenDate ?? DateTime.Now.AddDays(-5))).TotalHours;

            if (!token.IsSet() || tokenDb == null || (tokenDb?.IsDeleted ?? true) || tokenActiveHours > Constants.AuthTokenValidHours)
                return false;

            return true;
        }
    }
}