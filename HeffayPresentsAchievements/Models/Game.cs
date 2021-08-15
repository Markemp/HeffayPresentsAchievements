using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HeffayPresentsAchievements.Models
{
    public class Game : BaseEntity
    {
        [Required]
        public string Name { get; set; } = "";
        public virtual ICollection<Achievement>? Achievements { get; set; }
        public virtual ICollection<User>? Users { get; set; }
    }
}
