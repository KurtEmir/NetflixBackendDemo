using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeFlix.Models
{
    public class Episode
    {
        public long Id { get; set; } 
        public int MediaId { get; set; }
        [Range(0, byte.MaxValue)]
        public byte SeasonNumber { get; set; }
        [Range(0, 366)]
        public short EpisodeNumber { get; set; }
        public DateTime ReleaseDate { get; set; }
        [Column(TypeName = "nvarchar(200)")]
        [StringLength(200, MinimumLength = 1)]
        public string Title { get; set; } = "";
        [Column(TypeName = "nvarchar(500)")]
        [StringLength(500)]
        public string? Description { get; set; }
        public TimeSpan Duration { get; set; }//film, dizi süresi
        public long ViewCount {  get; set; } //kaç kişi izledi 
        public bool Passive { get; set; }

        [ForeignKey("MediaId")]
        public Media? Media { get; set; }


    }
}
