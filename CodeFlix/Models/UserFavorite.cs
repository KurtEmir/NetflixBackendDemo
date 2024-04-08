using System.ComponentModel.DataAnnotations.Schema;

namespace CodeFlix.Models
{
    public class UserFavorite
    {
        public long UserId { get; set; }
        public int MediaId { get; set; }

        [ForeignKey("UserId")]
        public CodeFlixUser? CodeFlixUser { get; set; }

        [ForeignKey("MediaId")]
        public Media? Media { get; set; }
    }
}
