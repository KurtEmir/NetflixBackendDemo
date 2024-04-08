using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CodeFlix.Models
{
    public class Plan
    {
        public long Id { get; set; }
        [Column(TypeName = "nvarchar(50)")]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; } = "";
        [Range(0,float.MaxValue)]
        public float Price {  get; set; }
        [Column(TypeName = "varchar(20)")]
        [StringLength(20, MinimumLength = 2)]
        public string Resolution { get; set; } = ""; //film kalitesi

    }
}
