namespace Overseer.Models
{
    /// <summary>
    /// This is the user object that is send to and from the client
    /// </summary>
    public class UserDisplay
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public int? SessionLifetime { get; set; }

        public string Token { get; set; }

        public bool IsLoggedIn { get; set; }

        public AccessLevel AccessLevel { get; set; }
    }
}