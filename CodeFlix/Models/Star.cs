using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CodeFlix.Models
{
    public class Star : Person
    {
        public List<MediaStar>? MediaStars { get; set; }
    }
}