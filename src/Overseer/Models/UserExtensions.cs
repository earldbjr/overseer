using System;

namespace Overseer.Models
{
    public static class UserExtensions
    {
        public static bool IsTokenExpired(this User user)
        {
            //if the user or token is null it's considered expired
            if (string.IsNullOrWhiteSpace(user?.Token)) return true;

            //if there is no expiration set the, with the presence of a token, the user
            //is configured for indefinite session length
            if (!user.TokenExpiration.HasValue) return false;

            //otherwise the tokens is expired if it's expiration date is less than the current UTC time
            return user.TokenExpiration.Value < DateTime.UtcNow;
        }
    }
}