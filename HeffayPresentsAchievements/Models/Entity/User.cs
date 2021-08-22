using System.Collections.Generic;

namespace HeffayPresentsAchievements.Models
{
    public class User : BaseEntity
    {
        public string Username { get; set; } = "";

        public string Email { get; set; } = "";
        
        public byte[]? PasswordHash { get; set; }
        
        public byte[]? PasswordSalt { get; set; }
        
        public virtual ICollection<Game>? Games { get; set; }
    }
}
