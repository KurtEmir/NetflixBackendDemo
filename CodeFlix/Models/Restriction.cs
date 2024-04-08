using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CodeFlix.Models
{
    public class Restriction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)] //DB buraya oto değer vermesin.ben program.cs veya başka bir yerde vereceğim.
        public byte Id { get; set; }
        [Column(TypeName = "nvarchar(50)")]
        [StringLength(50)]
        [Required]
        public string Name { get; set; } = "";

    }
}
