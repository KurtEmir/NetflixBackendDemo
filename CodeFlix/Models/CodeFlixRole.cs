using Microsoft.AspNetCore.Identity;

namespace CodeFlix.Models
{
    public class CodeFlixRole : IdentityRole<long>
    {
        public CodeFlixRole(string name) : base(name) { }

    }
}
