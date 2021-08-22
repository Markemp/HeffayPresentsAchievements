using System.Collections.Generic;

namespace HeffayPresentsAchievements.Models
{
    public class Game : BaseEntity
    {
        public string Name { get; set; } = "";
        public bool IsDeleted { get; set; }
        public virtual ICollection<Achievement>? Achievements { get; set; }
        public virtual ICollection<User>? Users { get; set; }
    }
}
