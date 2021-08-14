using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HeffayPresentsAchievements.Models
{
    public class User : BaseEntity
    {
        [Required]
        public string Username { get; set; } = "";
        public virtual ICollection<Game>? Games { get; set; }
    }
}
