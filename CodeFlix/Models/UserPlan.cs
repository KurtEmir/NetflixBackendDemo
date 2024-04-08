using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace CodeFlix.Models
{
    public class UserPlan
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long PlanId {  get; set; }
        [Column(TypeName ="date")]
        public DateTime StartDate { get; set; }
        [Column(TypeName = "date")]
        public DateTime EndDate { get; set; }
        [ForeignKey("UserId")]
        public CodeFlixUser? CodeFlixUser { get; set; }
        [ForeignKey("PlanId")]
        public Plan? Plan { get; set; }
    }
}
