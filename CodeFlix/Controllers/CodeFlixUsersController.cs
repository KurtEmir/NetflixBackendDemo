using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CodeFlix.Data;
using CodeFlix.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CodeFlix.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CodeFlixUsersController : ControllerBase
    {
        public struct LoginModel
        {
            public string UserName { get; set; }
            public string Password { get; set; }
        }

        private readonly CodeFlixContext _context;
        private readonly SignInManager<CodeFlixUser> _signInManager;
       
        public CodeFlixUsersController(CodeFlixContext context, SignInManager<CodeFlixUser> signInManager)
        {
            _context = context;
            _signInManager = signInManager;
        }

        // GET: api/CodeFlixUsers
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public ActionResult<List<CodeFlixUser>> GetUsers(bool includePassive = true)
        {
            IQueryable<CodeFlixUser> users = _signInManager.UserManager.Users;

            if (includePassive == false)
            {
                users = users.Where(u => u.Passive == false);
            }
            return users.AsNoTracking().ToList(); //tolist koymazsak linq çalışmaz. EF ile alakalı. amacı gerekmedikçe SQL e sorgu attırmamak
        }

        // GET: api/CodeFlixUsers/5
        [HttpGet("{id}")]
        [Authorize]
        public ActionResult<CodeFlixUser> GetCodeFlixUser(long id)
        {
            CodeFlixUser? codeFlixUser=null;

            if (User.IsInRole("Administrator")==false)
            {
                if (User.FindFirstValue(ClaimTypes.NameIdentifier) != id.ToString())
                {
                    return Unauthorized();
                }
            }

            codeFlixUser = _signInManager.UserManager.Users.Where(u => u.Id == id).AsNoTracking().FirstOrDefault();


            /*if (User.IsInRole("Administrator")==true)
            {
                codeFlixUser= _signInManager.UserManager.Users.Where(u => u.Id == id).FirstOrDefault();
            }
            else
            {
                if (User.FindFirstValue(ClaimTypes.NameIdentifier) == id.ToString())
                {
                    codeFlixUser = _signInManager.UserManager.Users.Where(u => u.Id == id).FirstOrDefault();
                }
                else
                {
                    return Unauthorized();
                }
            } yukarıdaki kod la aynı işi yapar. 
            */

            if (codeFlixUser == null)
            {
                return NotFound();
            }

            return codeFlixUser;
        }

        // PUT: api/CodeFlixUsers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        [Authorize]
        public ActionResult PutCodeFlixUser(CodeFlixUser codeFlixUser)
        {   //identity den dolayı direkt update etmeyiz. bu şekilde bir değeişkene verip daha sonra update etmeliyiz.
            CodeFlixUser? user = null;

            if (User.IsInRole("ContentAdmin") == false)
            {
                if (User.FindFirstValue(ClaimTypes.NameIdentifier) != codeFlixUser.Id.ToString())//kendisi mi sorgusu
                {
                    return Unauthorized();
                }
            }

            user = _signInManager.UserManager.Users.Where(u => u.Id == codeFlixUser.Id).FirstOrDefault(); //burada aşağıda yine lazım olacak bu yüzden AsNoTracking() olmaz.
            
            if (user == null)
            {
                return NotFound();
            }
            user.PhoneNumber = codeFlixUser.PhoneNumber;
            user.UserName = codeFlixUser.UserName;
            user.BirthDate = codeFlixUser.BirthDate;
            user.Email = codeFlixUser.Email;
            user.Name = codeFlixUser.Name;
            _signInManager.UserManager.UpdateAsync(user).Wait();
            return Ok();
        }

        // POST: api/CodeFlixUsers
        [HttpPost]
        public ActionResult<object> PostCodeFlixUser(CodeFlixUser codeFlixUser, string password) //fonksiyon dışarı ne döndürüyorsa dönüş tipi ona göre konur
        {

            IdentityResult identityResult = _signInManager.UserManager.CreateAsync(codeFlixUser, password).Result;

            if (identityResult != IdentityResult.Success)
            {
                return identityResult.Errors.FirstOrDefault()!.Description;
            }
            return Ok();
        }

        // DELETE: api/CodeFlixUsers/5
        [HttpDelete("{id}")]
        [Authorize]
        public ActionResult DeleteCodeFlixUser(long id)
        {
            CodeFlixUser? user = null;

            if (User.IsInRole("Administrator") == false)
            {
                if (User.FindFirstValue(ClaimTypes.NameIdentifier) != id.ToString())
                {
                    return Unauthorized();
                }
            }

            user = _signInManager.UserManager.Users.Where(u => u.Id == id).FirstOrDefault(); //burada aşağıda yine lazım olacak bu yüzden AsNoTracking() olmaz.
            if (user == null)
            {
                return NotFound();
            }
            
            user.Passive=true;
            _signInManager.UserManager.UpdateAsync(user).Wait();
            return Ok();

        }

        [HttpPost("Login")]
        public ActionResult<List<Media>> Login(LoginModel loginModel) //işimizi görsün diye frontend bize json gönderebilsin diye yukarıda struct olarak tanımladık.
        {
            Microsoft.AspNetCore.Identity.SignInResult signInResult;
            CodeFlixUser codeFlixUser = _signInManager.UserManager.FindByNameAsync(loginModel.UserName).Result;
            List<Media>? medias = null;
            List<UserFavorite> userFavorites;
            IGrouping<short, MediaCategory>? mediaCategories;
            IQueryable<Media> mediaQuery;
            IQueryable<int> userWatcheds;

            if (codeFlixUser == null)
            {
                return NotFound();
            }
            if (_signInManager.UserManager.IsInRoleAsync(codeFlixUser, "Administrator").Result == true || _signInManager.UserManager.IsInRoleAsync(codeFlixUser, "ContentAdmin").Result == true)
            {
                signInResult = _signInManager.PasswordSignInAsync(codeFlixUser, loginModel.Password, false, false).Result;
                if (signInResult.Succeeded==true)
                {
                    return Ok("Admin LoggedIn");
                }
            }
            else if (_context.UserPlans.Where(u => u.UserId == codeFlixUser.Id && u.EndDate >= DateTime.Today).Any() == false)
            {
                codeFlixUser.Passive = true;
                _signInManager.UserManager.UpdateAsync(codeFlixUser).Wait();
            }
            if (codeFlixUser.Passive == true)
            {
                //redirect to package sell
                return Content("Passive"); 
            }
            signInResult = _signInManager.PasswordSignInAsync(codeFlixUser, loginModel.Password, false, false).Result;
            if (signInResult.Succeeded == true)
            {
                //Kullanıcının favori olarak işaretlediği mediaları ve kategorilerini alıyoruz.
                userFavorites = _context.UserFavorites.Where(u => u.UserId == codeFlixUser.Id).Include(u => u.Media).Include(u => u.Media!.MediaCategories).ToList();
                //userFavorites içindeki media kategorilerini ayıklıyoruz (SelectMany)
                //Bunları kategori id'lerine göre grupluyoruz (GroupBy)
                //Her grupta kaç adet item olduğuna bakıp (m.Count())
                //Çoktan aza doğru sıralıyoruz (OrderByDescending)
                //En üstteki, yani en çok item'a sahip grubu seçiyoruz (FirstOrDefault)
                mediaCategories = userFavorites.SelectMany(u => u.Media!.MediaCategories!).GroupBy(m => m.CategoryId).OrderByDescending(m => m.Count()).FirstOrDefault();
                if (mediaCategories != null)
                {
                    //Kullabıcının izlediği episode'lardan media'ya ulaşıp, sadece media id'lerini alıyoruz (Select)
                    //Tekrar eden media id'leri eliyoruz (Distinct)
                    userWatcheds = _context.UserWatcheds.Where(u => u.UserId == codeFlixUser.Id).Include(u => u.Episode).Select(u => u.Episode!.MediaId).Distinct();
                    //Öneri yapmak için mediaCategories.Key'i yani kullanıcının en çok favorilediği kategori id'sini kullanıyoruz
                    //Media listesini çekerken sadece bu kategoriye ait mediaların MediaCategories listesini dolduruyoruz (Include(m => m.MediaCategories!.Where(mc => mc.CategoryId == mediaCategories.Key)))
                    //Diğer mediaların MediaCategories listeleri boş kalıyor
                    //Sonrasında MediaCategories'i boş olmayan media'ları filtreliyoruz (m.MediaCategories!.Count > 0)
                    //Ayrıca bu kategoriye giren fakat kullanıcının izlemiş olduklarını da dışarıda bırakıyoruz (userWatcheds.Contains(m.Id) == false)
                    mediaQuery = _context.Medias.Include(m => m.MediaCategories!.Where(mc => mc.CategoryId == mediaCategories.Key)).Where(m => m.MediaCategories!.Count > 0 && userWatcheds.Contains(m.Id) == false);
                    if (codeFlixUser.Restriction != null)
                    {
                        //TO DO
                        //Son olarak, kullanıcı bir restrictiona sahipse seçilen media içerisinden bunları da çıkarmamız gerekiyor.
                        mediaQuery = mediaQuery.Include(m => m.MediaRestrictions!.Where(r => r.RestrictionId <= codeFlixUser.Restriction));
                    }
                    medias = mediaQuery.ToList();
                }
                //Populate medias
            }
            return medias;
        }
    }
}
