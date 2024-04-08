using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace CodeFlix.Models;

// Add profile data for application users by adding properties to the CodeFlixUser class
public class CodeFlixUser : IdentityUser<long>
{
    [Column(TypeName ="date")]
    public DateTime BirthDate { get; set; }
    [Column(TypeName = "nvarchar(100)")]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = "";
    public bool Passive {  get; set; }

    //[NotMapped]//veri tabanına yazmasına gerek yok demek. migration gerektirmez.
    //[StringLength(100, MinimumLength = 8)] body de verirsek bu şekilde sınırlamalarını da burada ayarlayabiliriz.
    //public string Password { get; set; } = ""; //böyle yaparsak password body de gelecek. diğer türlü parametre olarak alacak.
    //bunu yaptıktan sonra password istersen; password yerine CreateAsync içinde CodeFlixUser.password olarak alırsın. Parametre olarak vermene gerek yok

    [NotMapped]
    public byte? Restriction
    {
        get
        {
            int age = DateTime.Today.Year - BirthDate.Year;
            
            if (age < 7)
            {
                return 7;
            }
            else
            {
                if (age < 13)
                {
                    return 13;
                }
                else
                {
                    if (age<18)
                    {
                        return 18;
                    }
                }
            }
            return null;
        }
        //set e gerek yok.
    } 
}