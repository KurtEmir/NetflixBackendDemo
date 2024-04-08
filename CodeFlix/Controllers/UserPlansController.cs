using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CodeFlix.Data;
using CodeFlix.Models;
using NuGet.Protocol;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace CodeFlix.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserPlansController : ControllerBase
    {
        private readonly CodeFlixContext _context;
        private readonly UserManager<CodeFlixUser> _userManager;


        public UserPlansController(CodeFlixContext context, UserManager<CodeFlixUser> usermanager)
        {
            _context = context;
            _userManager = usermanager;
        }

        // GET: api/UserPlans
        [HttpGet]
        public ActionResult<List<UserPlan>> GetUserPlans()
        {
            return _context.UserPlans.ToList();
        }

        // GET: api/UserPlans/5
        [HttpGet("{id}")]
        public ActionResult<UserPlan> GetUserPlan(long id)
        {
            UserPlan? userPlan = _context.UserPlans.FindAsync(id).Result;
             
            if (userPlan == null)
            {
                return NotFound();
            }

            return userPlan;
        }
        
        // POST: api/UserPlans
        [HttpPost]
        public async Task<ActionResult> PostUserPlan(string eMail, long planId)
        {
            // Kullanıcıyı bul
            var user = await _userManager.FindByEmailAsync(eMail);

            // Planı bul
            var plan = _context.Plans.Find(planId);

            //Bide buraya bir planın süresi dolduysa başka bir plan atanabilsin komutu lazım
            // Kullanıcının zaten belirtilen plana sahip olup olmadığını kontrol et
            //bool hasAnyPlan = _context.UserPlans.Any(up => up.UserId == user.Id);
            //if (hasAnyPlan)
            //{
            //    // Kullanıcı zaten herhangi bir plana sahip, hata işle
            //    return BadRequest("Kullanıcı zaten bir plana sahip.");
            //}
            var existingUserPlan = _context.UserPlans.FirstOrDefault(up => up.UserId == user.Id);
            if (existingUserPlan != null && existingUserPlan.EndDate > DateTime.Today)
            {
                // Mevcut plan hala geçerli, yeni bir plan ekleyemezsiniz
                return BadRequest("Kullanıcının mevcut planı hala geçerli.");
            }

            // Yeni kullanıcı planı oluştur
            var userPlan = new UserPlan
            {
                UserId = user.Id,
                PlanId = planId,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddMonths(1)
            };

            // Kullanıcı planını ekle ve değişiklikleri kaydet
            _context.UserPlans.Add(userPlan);
            await _context.SaveChangesAsync();
            return Ok();
        }

    }
}
