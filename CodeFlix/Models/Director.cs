using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CodeFlix.Models
{
    public class Director : Person
    {
        public List<MediaDirector>? MediaDirectors { get; set; }
    }
}
