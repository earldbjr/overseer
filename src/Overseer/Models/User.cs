using Overseer.Data;
using System;

namespace Overseer.Models
{
    /// <summary>
    /// This is the user as represented in the database
    /// </summary>
    public class User : IEntity
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string PasswordHash { get; set; }

        public string PasswordSalt { get; set; }
        
        public string Token { get; set; }

        public DateTime? TokenExpiration { get; set; }

        public int? SessionLifetime { get; set; }

        public AccessLevel AccessLevel { get; set; }

        public string PreauthenticatedToken { get; set; }

        public DateTime? PreauthenticatedTokenExpiration { get; set; }

        /// <summary>
        /// Helper method to quickly convert a user to a user display object
        /// </summary>
        public UserDisplay ToDisplay(bool includeToken = false)
        {
            return new UserDisplay
            {
                Id = Id,
                Username = Username,
                SessionLifetime = SessionLifetime,
                Token = includeToken ? Token : null,
                IsLoggedIn = !this.IsTokenExpired(),
                AccessLevel = AccessLevel
            };
        }
    }
}