using System.ComponentModel.DataAnnotations.Schema;

namespace CodeFlix.Models
{
    public class UserWatched
    {
        public long UserId { get; set; }
        public long EpisodeId { get; set; }

        [ForeignKey("UserId")]
        public CodeFlixUser? CodeFlixUser { get; set; }

        [ForeignKey("EpisodeId")]
        public Episode? Episode { get; set; }
    }
}
